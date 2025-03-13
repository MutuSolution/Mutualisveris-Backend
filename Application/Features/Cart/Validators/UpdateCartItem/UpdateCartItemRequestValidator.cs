using Common.Requests.Cart;
using FluentValidation;

namespace Application.Features.Cart.Validators.UpdateCartItem;

public class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.NewQuantity).GreaterThan(-1);
    }
}
