using Application;
using AspNetCoreRateLimit;
using Infrastructure;
using Microsoft.Extensions.Options;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1) Servis konfigurasyonu ve IoC kayýtlarý
builder.Services.AddCors(o =>
    o.AddPolicy("Mutuapp cors", builder =>
    {
        builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    }
    ));
builder.Services.AddCustomLocalization();
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentitySettings();
builder.Services.AddApplicationServices();

// Kimlik doðrulama servislerini ekliyoruz (JWT).
builder.Services.AddJwtAuthentication(
    builder.Services.GetApplicationSettings(builder.Configuration)
);

// Identity, Infrastructure vb. kayýtlar
builder.Services.AddIdentityServices();
builder.Services.AddProductService();
builder.Services.AddCategoryService();
builder.Services.AddInfrastructureDependencies();
builder.Services.AddEmailService();

// Swagger ve Rate Limit
builder.Services.AddEndpointsApiExplorer();
builder.Services.RegisterSwagger();
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();

// HttpContextAccessor ve diðer küçük servisler
builder.Services.AddHttpContextAccessor();

// 2) Middleware pipeline'ý oluþturma
var app = builder.Build();

// Örnek: Veritabaný seed iþlemi
app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS yönlendirme (isteðe baðlý)
app.UseHttpsRedirection();

// CORS
app.UseCors("Mutuapp cors");

// Yerelleþtirme (Localizasyon)
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value
);

// Rate limiting
app.UseIpRateLimiting();

// !!! Kimlik doðrulama iþlemlerinin Authorization’dan önce devreye girmesi için
app.UseAuthentication();

// Rol bazlý/Politika bazlý yetkilendirme
app.UseAuthorization();

// Global hata yakalama (Custom middleware)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Controller'larý eþle
app.MapControllers();

// Uygulamayý çalýþtýr
app.Run();
