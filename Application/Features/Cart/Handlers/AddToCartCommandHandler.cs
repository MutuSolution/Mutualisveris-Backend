using Application.Features.Cart.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Handlers
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, IResponseWrapper>
    {
        private readonly ICartService _cartService;

        public AddToCartCommandHandler(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IResponseWrapper> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            return await _cartService.AddToCartAsync(request.Request);
        }
    }
}
