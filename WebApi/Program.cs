using Application;
using AspNetCoreRateLimit;
using Infrastructure;
using Microsoft.Extensions.Options;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

/*** 1) Servis Konfig�rasyonu ***/
var configuration = builder.Configuration;
var environment = builder.Environment;

// G�venlik
builder.Services.AddCors(o => o.AddPolicy("DevelopmentCors", policy =>
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Localization
builder.Services.AddCustomLocalization();

// Database & Identity
builder.Services.AddDatabase(configuration);
builder.Services.AddIdentityServices();
builder.Services.AddIdentitySettings();

// Uygulama Katman�
builder.Services.AddApplicationServices();
builder.Services.AddProductService();
builder.Services.AddProductImageService();
builder.Services.AddCategoryService();
builder.Services.AddCartService();
builder.Services.AddAddressService();


// Altyap� Servisleri
builder.Services.AddInfrastructureDependencies();
builder.Services.AddEmailService();

// Kimlik Do�rulama
var appSettings = builder.Services.GetApplicationSettings(configuration);
builder.Services.AddJwtAuthentication(appSettings);

// API Konfig�rasyonu
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

// Yard�mc� Servisler
builder.Services.AddHttpContextAccessor();

/*** 2) Middleware Pipeline ***/
var app = builder.Build();

// Veritaban� Initialization
app.SeedDatabase();

// Global Hata Y�netimi (�lk middleware olarak)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Geli�tirme Ara�lar�
if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentCors");
}
else // Production Ayarlar�
{
    app.UseHsts();
    app.UseHttpsRedirection();

    // Production CORS Politikas� (�rnek - Kendi domainlerinizle g�ncelleyin)
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
// Statik dosya servisini a�

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