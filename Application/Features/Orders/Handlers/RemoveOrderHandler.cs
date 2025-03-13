using Application.Features.Orders.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;

public class RemoveOrderHandler : IRequestHandler<RemoveOrderCommand, IResponseWrapper>
{
    private readonly IOrderService _orderService;

    public RemoveOrderHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper> Handle(RemoveOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.RemoveOrderAsync(request.OrderId);
    }
}
