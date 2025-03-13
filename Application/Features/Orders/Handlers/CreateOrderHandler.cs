using Application.Features.Orders.Commands;
using Application.Services;
using Common.Responses.Wrappers;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, IResponseWrapper>
{
    private readonly IOrderService _orderService;

    public CreateOrderHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IResponseWrapper> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrderAsync(request.Request);
    }
}
