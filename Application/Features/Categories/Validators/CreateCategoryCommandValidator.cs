using Application.Features.Categories.Commands;
using FluentValidation;

namespace Application.Features.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.CreateCategory)
            .SetValidator(new CreateCategoryRequestValidator());
    }
}
