using Application.Features.Orders.Queries;
using Application.Services;
using Common.Responses.Orders;
using Common.Responses.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, IResponseWrapper<OrderResponse>>
{
    private readonly IOrderService _orderService;

    public GetOrderByIdHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _orderService.GetOrderByIdAsync(request.OrderId);
    }
}
