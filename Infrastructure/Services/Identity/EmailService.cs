using Application.Services.Identity;
using AutoMapper;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;
using Domain;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services.Identity;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public EmailService(IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _configuration = configuration;
        _userManager = userManager;
        _mapper = mapper;
    }

    // The code appears to be mostly correct, but there are a few improvements and error handling that can be added:

    public async Task<ResponseWrapper> SendEmailConfirmAsync(SendEmailConfirmRequest request)
    {
        if (request.Email is null)
            return (ResponseWrapper)await ResponseWrapper.FailAsync("[ML91] Email required.");

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return (ResponseWrapper)await ResponseWrapper.FailAsync("[ML92] User does not exist.");

        if (user.EmailConfirmed)
            return (ResponseWrapper)await ResponseWrapper.FailAsync("[ML93] User already confirmed.");

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("confirm@mutuapp.com"));
        email.To.Add(MailboxAddress.Parse(request.Email));
        email.Subject = $"Mutuapp confirm code: {code}";
        email.Body = new BodyBuilder
        {
            HtmlBody = $"<p>Welcome to Mutuapp, that is your confirmation code: <b>{code}</b></p>",
            TextBody = $"Welcome to Mutuapp, your confirmation code is: {code}"
        }.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            // Bağlantı
            await client.ConnectAsync(_configuration.GetSection("Emailhost").Value, 465, SecureSocketOptions.SslOnConnect);

            // Doğrulama
            await client.AuthenticateAsync(
                _configuration.GetSection("EmailUserName").Value,
                _configuration.GetSection("EmailPassword").Value
            );

            // E-posta gönderimi
            await client.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Loglama ve hata iletimi
            // Log ex.Message ile kaydedilebilir
            return (ResponseWrapper)await ResponseWrapper
                .FailAsync($"[ML90] Email sending failed: {ex.Message}");
        }
        finally
        {
            // Bağlantıyı kes ve kaynağı temizle
            await client.DisconnectAsync(true);
        }

        return await ResponseWrapper<string>.SuccessAsync("[ML94] Verification code has been sent.");
    }


    public async Task<IResponseWrapper> GetEmailConfirmAsync(EmailConfirmRequest emailConfirmRequest)
    {
        if (emailConfirmRequest.Code is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML85] Code required.");
        if (emailConfirmRequest.Email is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML86] Email required.");

        var user = await _userManager.FindByEmailAsync(emailConfirmRequest.Email);
        if (user is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML87] User not found.");

        var isVerified = await _userManager.ConfirmEmailAsync(user, emailConfirmRequest.Code);
        if (!isVerified.Succeeded)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML88] Email not confirmed.");

        return await ResponseWrapper<TokenResponse>.SuccessAsync("[ML89] Email confirmed.");
    }
}
