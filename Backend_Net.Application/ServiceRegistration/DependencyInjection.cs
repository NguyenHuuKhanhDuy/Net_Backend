using System.Reflection;
using Backend_Net.Application.Options;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Application.Services.Rate;
using Backend_Net.Application.Services.Signature;
using Backend_Net.Application.Services.Webhook;
using Backend_Net.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend_Net.Application.ServiceRegistration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
        
        services.AddValidatorsFromAssembly(assembly);
        
        // Register services
        services.AddScoped<ISignatureService, SignatureService>();
        services.AddScoped<IPaymentTokenService, PaymentTokenService>();
        services.AddSingleton<IRateService, RateService>();
        services.AddHostedService<RateLoaderHostedService>();
        services.AddScoped<IWebhookService, WebhookService>();
        
        // Add custom options
        services.AddCustomOptions(configuration);
        
        // Add service invocation clients
        services.AddServiceInvocation(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AppOptions>(config.GetSection(AppOptions.OptionName));
        services.Configure<LoggingOptions>(config.GetSection(LoggingOptions.OptionName));
        services.Configure<PaymentSettingsOptions>(config.GetSection(PaymentSettingsOptions.OptionName));
        return services;
    }

    private static IServiceCollection AddServiceInvocation(this IServiceCollection services, IConfiguration config)
    {
        return services;
    }
}