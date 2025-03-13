using Common.Requests.Cart;
using Common.Responses.Cart;
using Common.Responses.Wrappers;

namespace Application.Services;

public interface ICartService
{
    Task<IResponseWrapper<CartResponse>> AddToCartAsync(AddToCartRequest request);
    Task<IResponseWrapper<CartResponse>> UpdateCartItemAsync(UpdateCartItemRequest request);
    Task<IResponseWrapper<CartResponse>> RemoveFromCartAsync(RemoveFromCartRequest request);
    Task<IResponseWrapper<CartResponse>> GetCartAsync(string userId);
    Task<IResponseWrapper<CartItemResponse>> GetCartItemAsync(string userId, int productId);
}
