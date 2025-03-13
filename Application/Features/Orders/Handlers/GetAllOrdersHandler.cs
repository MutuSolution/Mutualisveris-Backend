using Application.Features.Orders.Queries;
using Application.Services;
using Common.Responses.Orders;
using Common.Responses.Wrappers;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IResponseWrapper<List<OrderResponse>>>
{
    private readonly IOrderService _orderService;

    public GetAllOrdersHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper<List<OrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetAllOrdersAsync();
    }
}
