using Common.Responses.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Queries;

public class GetCartQuery : IRequest<IResponseWrapper<CartResponse>>
{
    public string UserId { get; set; }
}
