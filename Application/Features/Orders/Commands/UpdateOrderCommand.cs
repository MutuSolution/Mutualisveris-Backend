using Common.Requests.Orders;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands;

public class UpdateOrderCommand : IRequest<IResponseWrapper>
{
    public UpdateOrderRequest Request { get; set; }
}
