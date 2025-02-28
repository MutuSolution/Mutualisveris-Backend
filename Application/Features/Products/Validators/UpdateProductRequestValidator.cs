using Application.Services;
using Common.Requests.Products;
using Common.Responses.Products;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator(IProductService productService)
    {

        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellation) => await productService
            .GetProductByIdAsync(id) is ProductResponse existing)
            .WithMessage("[ML27] Product does not exit.");

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