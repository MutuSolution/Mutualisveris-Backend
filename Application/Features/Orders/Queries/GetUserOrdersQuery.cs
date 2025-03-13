using Common.Responses.Orders;
using Common.Responses.Wrappers;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Orders.Queries;

public class GetUserOrdersQuery : IRequest<IResponseWrapper<List<OrderResponse>>>
{
    public string UserId { get; set; }
}
