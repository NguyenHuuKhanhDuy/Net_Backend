using Backend_Net.Api.Services.CustomHttpContextAccessor;
using Backend_Net.Application.Common.Interfaces;

namespace Backend_Net.Api.StartupRegistrations;

public static class HttpContextAccessorExtensions
{
    public static IServiceCollection AddCustomHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddSingleton<ICustomHttpContextAccessor, CustomHttpContextAccessorService>();
        return services;
    } 
}