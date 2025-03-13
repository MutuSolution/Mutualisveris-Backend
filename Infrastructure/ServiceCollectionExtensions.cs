using Application.Services;
using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<ApplicationDbSeeeder>();
        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services
                .AddTransient<ITokenService, TokenService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IRoleService, RoleService>()
                .AddHttpContextAccessor()
                .AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    public static IServiceCollection AddProductService(this IServiceCollection services)
    {
        services
            .AddTransient<IProductService, ProductService>();
        return services;
    }
    public static IServiceCollection AddProductImageService(this IServiceCollection services)
    {
        services
            .AddTransient<IProductImageService, ProductImageService>();
        return services;
    }
    public static IServiceCollection AddOrderService(this IServiceCollection services)
    {
        services
            .AddTransient<IOrderService, OrderService>();
        return services;
    }
    public static IServiceCollection AddCategoryService(this IServiceCollection services)
    {
        services
            .AddTransient<ICategoryService, CategoryService>();
        return services;
    }
    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        services
            .AddTransient<IEmailService, EmailService>();
        return services;
    }
    public static IServiceCollection AddCartService(this IServiceCollection services)
    {
        services
            .AddTransient<ICartService, CartService>();
        return services;
    }
    public static IServiceCollection AddAddressService(this IServiceCollection services)
    {
        services
            .AddTransient<IAddressService, AddressService>();
        return services;
    }
    public static void AddInfrastructureDependencies(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}
