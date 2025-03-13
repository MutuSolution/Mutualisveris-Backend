using Common.Requests.Orders;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands;

public class CreateOrderCommand : IRequest<IResponseWrapper>
{
    public CreateOrderRequest Request { get; set; }
}
