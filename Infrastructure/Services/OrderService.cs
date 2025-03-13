using Application.Services;
using AutoMapper;
using Common.Requests.Orders;
using Common.Responses.Addresses;
using Common.Responses.Orders;
using Common.Responses.Payments;
using Common.Responses.Wrappers;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResponseWrapper<OrderResponse>> CreateOrderAsync(CreateOrderRequest request)
    {
        try
        {
            // ✅ Kullanıcının mevcut sepetini getir
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null || cart.Items.Count == 0)
                return ResponseWrapper<OrderResponse>.Fail("Sepet bulunamadı veya boş.");

            // ✅ Kullanıcı bilgilerini çekelim
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                return ResponseWrapper<OrderResponse>.Fail("Kullanıcı bulunamadı.");

            // ✅ Adresleri veritabanından çekelim
            var shippingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.ShippingAddressId);
            var billingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.BillingAddressId);

            // ✅ Sipariş oluştur
            var order = new Order
            {
                UserId = request.UserId,
                OrderDate = DateTime.UtcNow,
                ShippingAddressId = request.ShippingAddressId,
                BillingAddressId = request.BillingAddressId,
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                Status = OrderStatus.Pending,
                OrderItems = cart.Items.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList(),
                TotalAmount = cart.Items.Sum(ci => ci.Quantity * ci.UnitPrice),
                Payment = new Payment
                {
                    Amount = cart.Items.Sum(ci => ci.Quantity * ci.UnitPrice),
                    Method = PaymentMethod.CreditCard,
                    Status = PaymentStatus.Pending
                }
            };

            _context.Orders.Add(order);
            cart.Items.Clear();
            await _context.SaveChangesAsync();

            // ✅ **Siparişi tekrar çekiyoruz, `User` bilgisini de dahil ediyoruz**
            var createdOrder = await _context.Orders
                .Include(o => o.User) // 🔥 Kullanıcıyı ekledik
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderResponse = _mapper.Map<OrderResponse>(createdOrder);
            return ResponseWrapper<OrderResponse>.Success(orderResponse, "Sipariş başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<OrderResponse>.Fail($"Sipariş oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<OrderResponse>> UpdateOrderAsync(UpdateOrderRequest request)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.User) // 🔥 Kullanıcı bilgisi yüklendi
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // 🔥 Ürün bilgisi eklendi
                    .ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order == null)
                return ResponseWrapper<OrderResponse>.Fail("Sipariş bulunamadı.");

            // ❗ **Sipariş durumu "Delivered" ise güncellenemez**
            if (order.Status == OrderStatus.Delivered)
                return ResponseWrapper<OrderResponse>.Fail("Bu sipariş zaten teslim edildi ve değiştirilemez.");

            // ✅ **Sipariş Durumu Güncelleniyor**
            if (request.Status.HasValue)
            {
                if (!Enum.IsDefined(typeof(OrderStatus), request.Status.Value))
                    return ResponseWrapper<OrderResponse>.Fail("Geçersiz sipariş durumu.");

                order.Status = request.Status.Value;
            }

            // ✅ **Adres Güncelleme**
            if (request.ShippingAddressId.HasValue)
            {
                var shippingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.ShippingAddressId.Value);
                if (shippingAddress == null)
                    return ResponseWrapper<OrderResponse>.Fail("Geçersiz teslimat adresi.");
                order.ShippingAddressId = request.ShippingAddressId;
                order.ShippingAddress = shippingAddress; // ✅ Adresi güncelliyoruz
            }

            if (request.BillingAddressId.HasValue)
            {
                var billingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == request.BillingAddressId.Value);
                if (billingAddress == null)
                    return ResponseWrapper<OrderResponse>.Fail("Geçersiz fatura adresi.");
                order.BillingAddressId = request.BillingAddressId;
                order.BillingAddress = billingAddress; // ✅ Adresi güncelliyoruz
            }

            // ✅ **Ürün Güncelleme ve Stok Kontrolü**
            if (request.OrderItems != null && request.OrderItems.Count != 0)
            {
                // 🔥 Önce stok kontrolü yap
                foreach (var item in request.OrderItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product == null)
                        return ResponseWrapper<OrderResponse>.Fail($"Ürün ({item.ProductId}) bulunamadı.");

                    if (item.Quantity > product.StockQuantity)
                        return ResponseWrapper<OrderResponse>.Fail($"Ürün ({product.Name}) için yeterli stok bulunmamaktadır.");
                }

                // 🔥 Eski sipariş ürünlerini temizle ve yeni ürünleri ekle
                order.OrderItems.Clear();
                order.OrderItems = request.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = _context.Products.Where(p => p.Id == item.ProductId).Select(p => p.Price).FirstOrDefault()
                }).ToList();
            }

            await _context.SaveChangesAsync();

            // ✅ **Kayıt ettikten sonra tekrar veritabanından çekiyoruz**
            var updatedOrder = await _context.Orders
                .Include(o => o.User) // 🔥 Kullanıcı bilgisi tekrar çekildi
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            var orderResponse = _mapper.Map<OrderResponse>(updatedOrder);
            return ResponseWrapper<OrderResponse>.Success(orderResponse, "Sipariş başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<OrderResponse>.Fail($"Sipariş güncellenirken hata oluştu: {ex.Message}");
        }
    }

    // ❌ **Sipariş Silme**
    public async Task<IResponseWrapper> RemoveOrderAsync(int orderId)
    {
        try
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return ResponseWrapper.Fail("Sipariş bulunamadı.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return ResponseWrapper.Success("Sipariş başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper.Fail($"Sipariş silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<OrderResponse>> GetOrderByIdAsync(int orderId)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.User) // 🔥 Kullanıcıyı ekledik
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return ResponseWrapper<OrderResponse>.Fail("Sipariş bulunamadı.");

            var orderResponse = _mapper.Map<OrderResponse>(order);
            return ResponseWrapper<OrderResponse>.Success(orderResponse, "Sipariş başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<OrderResponse>.Fail($"Sipariş getirilirken hata oluştu: {ex.Message}");
        }
    }


    public async Task<IResponseWrapper<List<OrderResponse>>> GetUserOrdersAsync(string userId)
    {
        try
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User) // 🔥 Kullanıcı bilgisi eklendi
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .ToListAsync();

            if (!orders.Any())
                return ResponseWrapper<List<OrderResponse>>.Fail("Kullanıcının siparişi bulunamadı.");

            var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            return ResponseWrapper<List<OrderResponse>>.Success(orderResponses, "Siparişler başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<List<OrderResponse>>.Fail($"Siparişler getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<List<OrderResponse>>> GetAllOrdersAsync()
    {
        try
        {
            var orders = await _context.Orders
                .Include(o => o.User) // 🔥 Kullanıcı bilgisi eklendi
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.Images)
                .Include(o => o.Payment)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .ToListAsync();

            if (!orders.Any())
                return ResponseWrapper<List<OrderResponse>>.Fail("Sistemde sipariş bulunamadı.");

            var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            return ResponseWrapper<List<OrderResponse>>.Success(orderResponses, "Tüm siparişler başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<List<OrderResponse>>.Fail($"Siparişler getirilirken hata oluştu: {ex.Message}");
        }
    }


}
