using Application.Features.Cart.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Handlers;
public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, IResponseWrapper>
{
    private readonly ICartService _cartService;

    public UpdateCartItemCommandHandler(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IResponseWrapper> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        return await _cartService.UpdateCartItemAsync(request.Request);
    }
}