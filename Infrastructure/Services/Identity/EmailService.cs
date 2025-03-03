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

namespace Infrastructure.Services.Identity
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        // Opsiyonel: ILogger<EmailService> _logger; // Hata loglama için

        public EmailService(IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ResponseWrapper> SendEmailConfirmAsync(SendEmailConfirmRequest request)
        {
            // Email alanı kontrolü: null, boş veya sadece boşluk karakterleri kontrol ediliyor.
            if (string.IsNullOrWhiteSpace(request.Email))
                return (ResponseWrapper)await
                    ResponseWrapper.FailAsync("[ML91] Email required.");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return (ResponseWrapper)await
                    ResponseWrapper.FailAsync("[ML92] User does not exist.");

            if (user.EmailConfirmed)
                return (ResponseWrapper)await
                    ResponseWrapper.FailAsync("[ML93] User already confirmed.");

            // Email onay token'ı oluşturuluyor.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // (Opsiyonel) Token'ı URL dostu hale getirmek için encode edebilirsiniz.

            var emailMessage = CreateEmailMessage(request.Email, token);

            using var client = new SmtpClient();
            try
            {
                // Konfigürasyon üzerinden değerler okunuyor.
                var host = _configuration["EmailHost"];
                var portString = _configuration["EmailPort"];
                if (!int.TryParse(portString, out int port))
                {
                    port = 465; // Varsayılan port
                }
                var emailUserName = _configuration["EmailUserName"];
                var emailPassword = _configuration["EmailPassword"];

                // Sunucuya bağlanma (SSL ile)
                await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);

                // Kimlik doğrulama
                await client.AuthenticateAsync(emailUserName, emailPassword);

                // E-posta gönderimi
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                // Hata loglama yapılabilir: _logger.LogError(ex, "Email gönderim hatası");
                return (ResponseWrapper)await ResponseWrapper
                    .FailAsync($"[ML90] Email sending failed: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

            return await ResponseWrapper<string>.SuccessAsync("[ML94] Verification code has been sent.");
        }

        // Email mesajı oluşturma işlemi ayrı metodda toplanarak kod tekrarı azaltıldı.
        private MimeMessage CreateEmailMessage(string recipientEmail, string token)
        {
            var message = new MimeMessage();

            // Gönderici adresi konfigürasyon üzerinden alınabilir. (Varsayılan değeri belirleniyor.)
            var fromAddress = _configuration["EmailFromAddress"] ?? "confirm@mutuapp.com";

            message.From.Add(MailboxAddress.Parse(fromAddress));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = "Mutualisveris Email Confirmation";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<p>Welcome to Mutualisveris, your confirmation code is: <strong>{token}</strong></p>",
                TextBody = $"Welcome to Mutualisveris, your confirmation code is: {token}"
            };

            message.Body = bodyBuilder.ToMessageBody();
            return message;
        }

        public async Task<IResponseWrapper> GetEmailConfirmAsync(EmailConfirmRequest emailConfirmRequest)
        {
            if (string.IsNullOrWhiteSpace(emailConfirmRequest.Code))
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML85] Code required.");
            if (string.IsNullOrWhiteSpace(emailConfirmRequest.Email))
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML86] Email required.");

            var user = await _userManager.FindByEmailAsync(emailConfirmRequest.Email);
            if (user is null)
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML87] User not found.");

            var confirmationResult = await _userManager.ConfirmEmailAsync(user, emailConfirmRequest.Code);
            if (!confirmationResult.Succeeded)
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML88] Email not confirmed.");

            return await ResponseWrapper<TokenResponse>.SuccessAsync("[ML89] Email confirmed.");
        }
    }
}
