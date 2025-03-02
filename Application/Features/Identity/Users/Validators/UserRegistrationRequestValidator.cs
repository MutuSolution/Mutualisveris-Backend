using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators;

public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
{
    public UserRegistrationRequestValidator(IUserService userService)
    {
        var forbiddenWords = new List<string>
{
    "admin", "root", "superuser", "administrator", "guest", "support", "test",
    "developer", "manager", "staff", "owner", "system", "user", "info",
    "webmaster", "moderator", "staff", "security", "service", "account",
    "backup", "poweruser", "access", "console", "privilege", "configuration",
    "service", "login", "password", "command", "super", "control", "maintenance"
};

        var bannedDomains = new List<string>
{
    "spam.com", "fake.com", "example.com", "test.com", "malicious.com", "phishing.com",
    "trashmail.com", "temporary.com", "tempmail.com", "mailinator.com", "guerrillamail.com",
    "yopmail.com", "disposable.com", "sharklasers.com", "dodgit.com", "instantemail.net",
    "throwawaymail.com", "fastmail.com", "privatemail.com", "mytemp.email", "maildrop.cc",
    "jetable.org", "trashmail.net", "fakeemail.com", "notreal.com", "nomail.com"
};


        RuleFor(x => x.Email)
        .NotNull().WithMessage("[ML1] Email cannot be null.")
        .NotEmpty().WithMessage("[ML2] Email cannot be empty.")
        .EmailAddress().WithMessage("[ML3] Invalid email format.")
        .Must(email => !bannedDomains.Any(domain => email.EndsWith($"@{domain}")))
        .WithMessage("[ML4] Email domain is not allowed.")
        .MaximumLength(200).WithMessage("[ML5] Email cannot exceed 200 characters.")
        .MustAsync(async (email, cancellation) => await userService
        .GetUserByEmailAsync(email) is not UserResponse existing)
        .WithMessage("[ML6] Email is already taken.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(200);


        RuleFor(x => x.Password)
         .MinimumLength(8).WithMessage("[ML15] Password must be at least 8 characters long.")
         //.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
         //.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
         //.Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
         //.Matches(@"[\W]").WithMessage("Password must contain at least one special character.")
         .NotEmpty().WithMessage("[ML16] Password cannot be empty.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("[ML17] Password confirmation does not match.");
    }
}
