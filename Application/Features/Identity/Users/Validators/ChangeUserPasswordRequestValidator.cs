using Application.Services.Identity;
using Common.Requests.Identity;
using FluentValidation;

namespace Application.Features.Identity.Users.Validators;

public class ChangeUserPasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangeUserPasswordRequestValidator(IUserService userService)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("[ML310] User ID boş olamaz");

        RuleFor(x => x.CurrentPassword)
        .MinimumLength(8).WithMessage("[ML316] Şifreniz en az 8 karakter olmalı.")
        .Matches(@"[A-Z]").WithMessage("[ML317] Şifreniz en az 1 Büyük harf içermeli.")
        .Matches(@"[a-z]").WithMessage("[ML318] Şifreniz en az 1 küçük harf içermeli.")
        .Matches(@"[0-9]").WithMessage("[ML319] Şifreniz en az 1 rakam içermeli.")
        .NotEmpty().WithMessage("[ML214] Şifre boş olamaz");

        RuleFor(x => x.NewPassword)
         .MinimumLength(8).WithMessage("[ML315] Yeni Şifreniz en az 8 karakter olmalı.")
         .Matches(@"[A-Z]").WithMessage("[ML313] Yeni Şifreniz en az 1 Büyük harf içermeli.")
         .Matches(@"[a-z]").WithMessage("[ML312] Yeni Şifreniz en az 1 küçük harf içermeli.")
         .Matches(@"[0-9]").WithMessage("[ML311] Yeni Şifreniz en az 1 rakam içermeli.")
         .NotEmpty().WithMessage("[ML214] Yeni Şifre boş olamaz");

        RuleFor(x => x.NewPassword)
            .Equal(x => x.ConfirmedNewPassword).WithMessage("[ML320] Şifreler eşleşmedi.");

    }
}
