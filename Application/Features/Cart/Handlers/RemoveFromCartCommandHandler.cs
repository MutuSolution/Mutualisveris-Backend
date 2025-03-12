using Application.Features.Cart.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Handlers;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, IResponseWrapper>
{
    private readonly ICartService _cartService;

    public RemoveFromCartCommandHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IResponseWrapper> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        return await _cartService.RemoveFromCartAsync(request.Request);
    }
}
