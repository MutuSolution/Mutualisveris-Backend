using Application.Features.Orders.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, IResponseWrapper>
{
    private readonly IOrderService _orderService;

    public UpdateOrderHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.UpdateOrderAsync(request.Request);
    }
}
