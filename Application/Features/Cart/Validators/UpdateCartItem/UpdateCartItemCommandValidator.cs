using Application.Features.Cart.Commands;
using FluentValidation;

namespace Application.Features.Cart.Validators.UpdateCartItem;

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new UpdateCartItemRequestValidator());
    }
}
