using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Orders.Commands;

public class RemoveOrderCommand : IRequest<IResponseWrapper>
{
    public int OrderId { get; set; }
}
