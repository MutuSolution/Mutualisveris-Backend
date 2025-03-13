using Application.Pipelines;
using Common.Requests.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Commands;

public class RemoveFromCartCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public RemoveFromCartRequest Request { get; set; }
}
