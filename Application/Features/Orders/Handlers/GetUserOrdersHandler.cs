using Application.Features.Orders.Queries;
using Application.Services;
using Common.Responses.Orders;
using Common.Responses.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;
 
public class GetUserOrdersHandler : IRequestHandler<GetUserOrdersQuery, IResponseWrapper<List<OrderResponse>>>
{
    private readonly IOrderService _orderService;

    public GetUserOrdersHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper<List<OrderResponse>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetUserOrdersAsync(request.UserId);
    }
}