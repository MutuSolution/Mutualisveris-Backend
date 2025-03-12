using MediatR;
using Common.Responses.Wrappers;
using Common.Responses.Cart;

namespace Application.Features.Cart.Queries;

public class GetCartQuery : IRequest<IResponseWrapper<CartResponse>>
{
    public string UserId { get; set; }
}
