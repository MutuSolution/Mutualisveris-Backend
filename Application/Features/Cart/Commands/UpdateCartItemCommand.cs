using Application.Pipelines;
using Common.Request.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Commands;

public class UpdateCartItemCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public UpdateCartItemRequest Request { get; set; }
}
