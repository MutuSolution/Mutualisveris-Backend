using Application.Features.Identity.Token.Queries;
using FluentValidation;

public class GetTokenQueryValidator : AbstractValidator<GetTokenQuery>
{
    public GetTokenQueryValidator()
    {
        RuleFor(x => x.TokenRequest)
            .NotNull().WithMessage("TokenRequest boş olamaz.")
            .SetValidator(new TokenRequestValidator());
    }
}
