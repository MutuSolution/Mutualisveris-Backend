using Application.Features.Identity.Users.Commands;
using Application.Services.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator(IUserService userService)
    {
        RuleFor(x => x.ChangePassword)
           .SetValidator(new ChangeUserPasswordRequestValidator(userService));
    }
}
