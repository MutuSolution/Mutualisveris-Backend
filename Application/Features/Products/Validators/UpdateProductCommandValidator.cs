using Application.Features.Products.Commands;
using Application.Services;
using FluentValidation;

namespace Application.Features.Products.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IProductService productService)
    {
        RuleFor(x => x.UpdateProductRequest)
            .SetValidator(new UpdateProductRequestValidator(productService));
    }
}
