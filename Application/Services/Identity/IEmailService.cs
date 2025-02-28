using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IEmailService
{
    Task<ResponseWrapper> SendEmailConfirmAsync(SendEmailConfirmRequest request);
    Task<IResponseWrapper> GetEmailConfirmAsync(EmailConfirmRequest emailConfirmRequest);
}
