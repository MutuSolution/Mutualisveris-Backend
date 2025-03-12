using MediatR;
using Common.Responses.Wrappers;
using Common.Request.Cart;
using Application.Pipelines;

namespace Application.Features.Cart.Commands;

public class AddToCartCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public AddToCartRequest Request { get; set; }
}
