using MediatR;
using Common.Responses.Wrappers;
using Common.Responses.Cart;

namespace Application.Features.Cart.Queries;

public class GetCartItemQuery : IRequest<IResponseWrapper<CartItemResponse>>
{
    public string UserId { get; set; }
    public int ProductId { get; set; }
}
