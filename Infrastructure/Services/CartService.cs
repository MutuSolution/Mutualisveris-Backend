using Application.Services;
using AutoMapper;
using Common.Request.Cart;
using Common.Responses.Cart;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
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
                // ❗ Kullanıcının var olup olmadığını kontrol et
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                    return ResponseWrapper<CartResponse>.Fail("Kullanıcı bulunamadı.");

                // ❗ Geçersiz ürün ID kontrolü
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                    return ResponseWrapper<CartResponse>.Fail("Ürün bulunamadı.");
                if (product.StockQuantity < request.Quantity)
                    return ResponseWrapper<CartResponse>.Fail("Yeterli stok yok.");

                // Kullanıcının aktif sepeti var mı kontrol et, yoksa oluştur
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.IsActive);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = request.UserId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Ürün zaten sepette mi?
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
                    cartItem.Quantity += request.Quantity;
                }

                await _context.SaveChangesAsync();
                return await GetCartAsync(request.UserId); // ✅ Güncellenmiş sepeti döndür!
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
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.IsActive);

                if (cart == null)
                    return ResponseWrapper<CartResponse>.Fail("Aktif sepet bulunamadı.");

                var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
                if (cartItem == null)
                    return ResponseWrapper<CartResponse>.Fail("Ürün sepette bulunamadı.");

                if (request.NewQuantity <= 0)
                {
                    cart.Items.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = request.NewQuantity;
                }

                await _context.SaveChangesAsync();
                return await GetCartAsync(request.UserId); // ✅ Güncellenmiş sepeti döndür!
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
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.IsActive);

                if (cart == null)
                    return ResponseWrapper<CartResponse>.Fail("Aktif sepet bulunamadı.");

                var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
                if (cartItem == null)
                    return ResponseWrapper<CartResponse>.Fail("Ürün sepette bulunamadı.");

                cart.Items.Remove(cartItem);
                await _context.SaveChangesAsync();
                return await GetCartAsync(request.UserId); // ✅ Güncellenmiş sepeti döndür!
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
                    .ThenInclude(p => p.Images) // 🔥 Ürün resimlerini de dahil edelim!
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

                if (cart == null)
                    return ResponseWrapper<CartResponse>.Fail("Aktif sepet bulunamadı.");

                var cartDto = new CartResponse
                {
                    CartId = cart.Id,
                    CreatedAt = cart.CreatedAt,
                    IsActive = cart.IsActive,
                    Items = cart.Items?.Select(ci => new CartItemResponse
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.Name ?? "Bilinmeyen Ürün",
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        Stock = ci.Product?.StockQuantity ?? 0,
                        SKU = ci.Product?.SKU ?? "N/A",
                        ImageUrl = ci.Product?.Images.FirstOrDefault()?.ImageUrl ?? string.Empty // 🔥 Ana ürün resmini al
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
                    .ThenInclude(p => p.Images) // 🔥 Ürün resimlerini de dahil et
                    .FirstOrDefaultAsync(ci => ci.Cart.UserId == userId && ci.ProductId == productId);

                if (cartItemEntity == null)
                    return ResponseWrapper<CartItemResponse>.Fail("Sepette bu ürün bulunamadı.");

                // 🔥 LINQ içinde hesaplama yapmıyoruz, burada hesaplıyoruz!
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
}
