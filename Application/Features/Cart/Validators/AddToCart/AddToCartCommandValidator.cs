using Application.Features.Cart.Commands;
using FluentValidation;

namespace Application.Features.Cart.Validators.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new AddToCartRequestValidator());
    }
}
