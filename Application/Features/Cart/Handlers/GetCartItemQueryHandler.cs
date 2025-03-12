using Application.Features.Cart.Queries;
using Application.Services;
using Common.Responses.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Handlers
{
    public class GetCartItemQueryHandler : IRequestHandler<GetCartItemQuery, IResponseWrapper<CartItemResponse>>
    {
        private readonly ICartService _cartService;

        public GetCartItemQueryHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper<CartItemResponse>> Handle(GetCartItemQuery request, CancellationToken cancellationToken)
        {
            return await _cartService.GetCartItemAsync(request.UserId, request.ProductId);
        }
    }
}
