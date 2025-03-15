using Application.Services;
using AutoMapper;
using Common.Requests.Cart;
using Common.Responses.Cart;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CartService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper<CartResponse>> AddToCartAsync(AddToCartRequest request)
    {
        try
        {
            // ✅ Kullanıcının mevcut sepetini getir
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null) // ✅ Kullanıcının hiç sepeti yoksa oluştur
            {
                cart = new Cart
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // ❗ Ürünü kontrol et
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
                return ResponseWrapper<CartResponse>.Fail("Ürün bulunamadı.");

            if (product.StockQuantity < request.Quantity)
                return ResponseWrapper<CartResponse>.Fail("Yeterli stok bulunmamaktadır.");

            // ❗ Ürün zaten sepette mi?
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem == null)
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price
                });
            }
            else
            {
                // ❗ Eğer stok yetersizse ekleme yapılmasın
                if (cartItem.Quantity + request.Quantity > product.StockQuantity)
                    return ResponseWrapper<CartResponse>.Fail("Yeterli stok bulunmamaktadır.");

                cartItem.Quantity += request.Quantity;
            }

            await _context.SaveChangesAsync();
            return ResponseWrapper<CartResponse>.Success("Ürün sepete eklendi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<CartResponse>.Fail($"Hata: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
                return ResponseWrapper<CartResponse>.Fail("Sepet bulunamadı.");

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem == null)
                return ResponseWrapper<CartResponse>.Fail("Ürün sepette bulunamadı.");

            // ❗ Güncellenirken stok kontrolü yapılmalı
            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
                return ResponseWrapper<CartResponse>.Fail("Ürün bulunamadı.");

            if (request.NewQuantity > product.StockQuantity)
                return ResponseWrapper<CartResponse>.Fail("Yeterli stok bulunmamaktadır.");

            if (request.NewQuantity <= 0)
            {
                cart.Items.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = request.NewQuantity;
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(request.UserId);
        }
        catch (Exception ex)
        {
            return ResponseWrapper<CartResponse>.Fail($"Hata: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<CartResponse>> RemoveFromCartAsync(RemoveFromCartRequest request)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId);

            if (cart == null)
                return ResponseWrapper<CartResponse>.Fail("Sepet bulunamadı.");

            // 🔥 Eğer null ise, boş liste olarak ata
            cart.Items ??= new List<CartItem>();

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (cartItem == null)
                return ResponseWrapper<CartResponse>.Fail("Ürün sepette bulunamadı.");

            cart.Items.Remove(cartItem);

            // 🔥 Sepette hiç ürün kalmadıysa (ve sistemde boş sepet tutulmuyorsa), sepeti de sil
            if (!cart.Items.Any())
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(request.UserId);
        }
        catch (Exception ex)
        {
            return ResponseWrapper<CartResponse>.Fail($"Hata: {ex.Message}");
        }
    }


    public async Task<IResponseWrapper<CartResponse>> GetCartAsync(string userId)
    {
        try
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return ResponseWrapper<CartResponse>.Fail("Sepet bulunamadı.");

            var cartDto = new CartResponse
            {
                CartId = cart.Id,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items?.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Bilinmeyen Ürün",
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    Stock = ci.Product?.StockQuantity ?? 0,
                    SKU = ci.Product?.SKU ?? "N/A",
                    ImageUrl = ci.Product?.Images.FirstOrDefault()?.ImageUrl ?? string.Empty
                }).ToList() ?? new List<CartItemResponse>(),

                TotalAmount = cart.Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0
            };

            return ResponseWrapper<CartResponse>.Success(cartDto, "Sepet başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<CartResponse>.Fail($"Hata: {ex.Message}");
        }
    }

    public async Task<IResponseWrapper<CartItemResponse>> GetCartItemAsync(string userId, int productId)
    {
        try
        {
            var cartItemEntity = await _context.CartItems
                .Include(ci => ci.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

            if (cartItemEntity == null)
                return ResponseWrapper<CartItemResponse>.Fail("Sepette bu ürün bulunamadı.");

            var cartItemResponse = new CartItemResponse
            {
                ProductId = cartItemEntity.ProductId,
                ProductName = cartItemEntity.Product?.Name ?? "Bilinmeyen Ürün",
                Quantity = cartItemEntity.Quantity,
                UnitPrice = cartItemEntity.UnitPrice,
                Stock = cartItemEntity.Product?.StockQuantity ?? 0,
                SKU = cartItemEntity.Product?.SKU ?? "N/A",
                ImageUrl = cartItemEntity.Product?.Images.FirstOrDefault()?.ImageUrl ?? string.Empty
            };

            return ResponseWrapper<CartItemResponse>.Success(cartItemResponse, "Ürün başarıyla getirildi.");
        }
        catch (Exception ex)
        {
            return ResponseWrapper<CartItemResponse>.Fail($"Hata: {ex.Message}");
        }
    }
}
