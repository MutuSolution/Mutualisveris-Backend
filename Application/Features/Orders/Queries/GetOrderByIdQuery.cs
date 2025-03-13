using Common.Responses.Orders;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Orders.Queries;

public class GetOrderByIdQuery : IRequest<IResponseWrapper<OrderResponse>>
{
    public int OrderId { get; set; }
}
