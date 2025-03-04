using Application.Features.Identity.Users.Commands;
using Application.Services.Identity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Users.Validators;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator(IUserService userService)
    {
        RuleFor(x => x.ChangePassword)
           .SetValidator(new ChangeUserPasswordRequestValidator(userService));
    }
}
