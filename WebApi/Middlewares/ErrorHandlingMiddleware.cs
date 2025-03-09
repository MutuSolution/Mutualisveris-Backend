using Application.Exceptions;
using Common.Responses.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen bir hata oluştu.");

            // ❗ Yanıt zaten başladıysa, middleware yeni yanıt yazamaz.
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("Yanıt zaten başlatıldığı için hata middleware'i devre dışı bırakıldı.");
                return;
            }

            // 📌 HTTP Yanıt başlıklarını ayarla
            var response = httpContext.Response;
            response.ContentType = "application/json";

            // 🔥 Hata türüne göre durum kodu belirle
            response.StatusCode = ex switch
            {
                CustomValidationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // 🔥 Yanıtı JSON formatında oluştur
            var responseWrapper = await ResponseWrapper.FailAsync(ex.Message);
            var result = JsonSerializer.Serialize(responseWrapper);

            // 🔥 Yanıtı gönder
            await response.WriteAsync(result);
        }
    }
}
