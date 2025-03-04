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
        .NotNull().WithMessage("[ML1] Mail boş olamaz.")
        .NotEmpty().WithMessage("[ML2] Mail boş olamaz.")
        .EmailAddress().WithMessage("[ML3] Mail yanlış formatta.")
        .Must(email => !bannedDomains.Any(domain => email.EndsWith($"@{domain}")))
        .WithMessage("[ML4] Mail adresine izin verilmedi.")
        .MaximumLength(200).WithMessage("[ML5] Mail 200 karakterden fazla olamaz.")
        .MustAsync(async (email, cancellation) => await userService
        .GetUserByEmailAsync(email) is not UserResponse existing)
        .WithMessage("[ML6] Mail zaten alınmış.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(200);


        RuleFor(x => x.Password)
         .MinimumLength(8).WithMessage("[ML215] Şifreniz en az 8 karakter olmalı.")
         .Matches(@"[A-Z]").WithMessage("[ML213] Şifreniz en az 1 Büyük harf içermeli.")
         .Matches(@"[a-z]").WithMessage("[ML212] Şifreniz en az 1 küçük harf içermeli.")
         .Matches(@"[0-9]").WithMessage("[ML211] Şifreniz en az 1 rakam içermeli.")
         //.Matches(@"[\W]").WithMessage("Password must contain at least one special character.")
         .NotEmpty().WithMessage("[ML214] Şifre boş olamaz");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("[ML17] Şifreler eşleşmedi.");
    }
}
