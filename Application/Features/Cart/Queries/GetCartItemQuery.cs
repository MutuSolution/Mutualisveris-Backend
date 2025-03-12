using Common.Responses.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Queries;

public class GetCartItemQuery : IRequest<IResponseWrapper<CartItemResponse>>
{
    public string UserId { get; set; }
    public int ProductId { get; set; }
}
