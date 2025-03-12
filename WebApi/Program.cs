using Application;
using AspNetCoreRateLimit;
using Infrastructure;
using Microsoft.Extensions.Options;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

/*** 1) Servis Konfigürasyonu ***/
var configuration = builder.Configuration;
var environment = builder.Environment;

// Güvenlik
builder.Services.AddCors(o => o.AddPolicy("DevelopmentCors", policy =>
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Localization
builder.Services.AddCustomLocalization();

// Database & Identity
builder.Services.AddDatabase(configuration);
builder.Services.AddIdentityServices();
builder.Services.AddIdentitySettings();

// Uygulama Katmaný
builder.Services.AddApplicationServices();
builder.Services.AddProductService();
builder.Services.AddProductImageService();
builder.Services.AddCategoryService();
builder.Services.AddCartService();
builder.Services.AddAddressService();


// Altyapý Servisleri
builder.Services.AddInfrastructureDependencies();
builder.Services.AddEmailService();

// Kimlik Doðrulama
var appSettings = builder.Services.GetApplicationSettings(configuration);
builder.Services.AddJwtAuthentication(appSettings);

// API Konfigürasyonu
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger (Sadece Development)
if (environment.IsDevelopment())
{
    builder.Services.RegisterSwagger();
}

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();

// Yardýmcý Servisler
builder.Services.AddHttpContextAccessor();

/*** 2) Middleware Pipeline ***/
var app = builder.Build();

// Veritabaný Initialization
app.SeedDatabase();

// Global Hata Yönetimi (Ýlk middleware olarak)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Geliþtirme Araçlarý
if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentCors");
}
else // Production Ayarlarý
{
    app.UseHsts();
    app.UseHttpsRedirection();

    // Production CORS Politikasý (Örnek - Kendi domainlerinizle güncelleyin)
    app.UseCors(policy => policy
        .WithOrigins("" +
        "https://mutualisveris.com",
        "https://api.mutualisveris.com",
        "http://localhost:3000",
        "https://localhost:3000"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
}
// Statik dosya servisini aç

// Localization
app.UseRequestLocalization(app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

// Rate Limiting
app.UseIpRateLimiting();

// Auth Pipeline
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Routing
app.MapControllers();

app.Run();