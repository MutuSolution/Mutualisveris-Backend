using MediatR;
using Common.Responses.Wrappers;
using Common.Request.Cart;
using Application.Pipelines;

namespace Application.Features.Cart.Commands;

public class UpdateCartItemCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateCartItemRequest Request { get; set; }
}
