using Application.Pipelines;
using Common.Request.Cart;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Cart.Commands;

public class AddToCartCommand : IRequest<IResponseWrapper>, IValidateMe
{
    public AddToCartRequest Request { get; set; }
}
