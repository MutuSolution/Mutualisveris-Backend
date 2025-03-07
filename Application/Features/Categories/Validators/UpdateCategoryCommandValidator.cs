using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.UpdateCategory)
            .SetValidator(new UpdateCategoryRequestValidator());
    }
}
