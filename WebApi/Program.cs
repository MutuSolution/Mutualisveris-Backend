using Application;
using AspNetCoreRateLimit;
using Infrastructure;
using Microsoft.Extensions.Options;
using WebApi;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1) Servis konfigurasyonu ve IoC kay�tlar�
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

// Kimlik do�rulama servislerini ekliyoruz (JWT).
builder.Services.AddJwtAuthentication(
    builder.Services.GetApplicationSettings(builder.Configuration)
);

// Identity, Infrastructure vb. kay�tlar
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

// HttpContextAccessor ve di�er k���k servisler
builder.Services.AddHttpContextAccessor();

// 2) Middleware pipeline'� olu�turma
var app = builder.Build();

// �rnek: Veritaban� seed i�lemi
app.SeedDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS y�nlendirme (iste�e ba�l�)
app.UseHttpsRedirection();

// CORS
app.UseCors("Mutuapp cors");

// Yerelle�tirme (Localizasyon)
app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value
);

// Rate limiting
app.UseIpRateLimiting();

// !!! Kimlik do�rulama i�lemlerinin Authorization�dan �nce devreye girmesi i�in
app.UseAuthentication();

// Rol bazl�/Politika bazl� yetkilendirme
app.UseAuthorization();

// Global hata yakalama (Custom middleware)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Controller'lar� e�le
app.MapControllers();

// Uygulamay� �al��t�r
app.Run();
