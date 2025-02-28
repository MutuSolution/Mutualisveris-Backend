using Common.Requests.Products;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Title)
      .NotEmpty();

        RuleFor(x => x.Url)
        .NotEmpty();

        RuleFor(x => x.UserName)
        .NotEmpty();

        RuleFor(x => x.Description)
        .NotEmpty();

    }
}
