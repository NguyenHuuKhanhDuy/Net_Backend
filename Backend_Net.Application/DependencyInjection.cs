using System.Reflection;
using Backend_Net.Application.Services.Signature;
using Backend_Net.Infrastructure.Options;
using Backend_Net.Infrastructure.Options.Logging;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend_Net.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        
        // Register services
        services.AddScoped<ISignatureService, SignatureService>();
        
        // Add custom options
        services.AddCustomOptions(configuration);
        return services;
    }
    
    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppOptions>(config.GetSection(AppOptions.OptionName));
        services.Configure<LoggingOptions>(config.GetSection(LoggingOptions.OptionName));
        return services;
    }
}