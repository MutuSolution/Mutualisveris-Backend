using Common.Requests.Products;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
      .NotEmpty();

        RuleFor(x => x.Description)
        .NotEmpty();

        RuleFor(x => x.Price)
        .NotEmpty();

        RuleFor(x => x.CategoryId)
        .NotEmpty();

        RuleFor(x => x.SKU)
      .NotEmpty();

    }
}
