using Application.Features.Cart.Queries;
using Application.Services;
using Common.Responses.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Handlers
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, IResponseWrapper<CartResponse>>
    {
        private readonly ICartService _cartService;

        public GetCartQueryHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper<CartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            return await _cartService.GetCartAsync(request.UserId);
        }
    }
}
