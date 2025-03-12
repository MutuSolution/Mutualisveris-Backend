using MediatR;
using Common.Responses.Wrappers;
using Common.Request.Cart;
using Application.Pipelines;

namespace Application.Features.Cart.Commands;

public class RemoveFromCartCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public RemoveFromCartRequest Request { get; set; }
}
