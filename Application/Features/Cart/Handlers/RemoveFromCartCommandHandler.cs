using MediatR;
using Application.Services;
using Common.Responses.Wrappers;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Cart.Commands;

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
