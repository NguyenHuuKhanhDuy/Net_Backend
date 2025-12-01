using System.Reflection;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.PaymentProvider;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Services.MessageBus;
using Backend_Net.Infrastructure.Localization;
using Backend_Net.Infrastructure.MassTransit;
using Backend_Net.Infrastructure.MassTransit.Consumers;
using Backend_Net.Infrastructure.Options;
using Backend_Net.Infrastructure.Persistence;
using Backend_Net.Infrastructure.Services.PaymentProvider;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.MassTransit.Contracts.Queues;

namespace Backend_Net.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddCustomServices();
        
        services.AddCustomMassTransitRegistration(configuration, Assembly.GetExecutingAssembly());
        return services;
    }
    
    private static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMessageBusService, MessageBusService>();
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddScoped<IStripeService, StripeService>();
        
        return services;
    }
    
    private static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<AzureServiceBusOptions>(config.GetSection(AzureServiceBusOptions.OptionName));
        services.Configure<RabbitMQOptions>(config.GetSection(RabbitMQOptions.OptionName));
        return services;
    }
    
    private static IServiceCollection AddCustomMassTransitRegistration(
        this IServiceCollection services, 
        IConfiguration config, 
        Assembly? assemblies)
    {
        services.AddMassTransitRegistration(config, assemblies,  (ctx, cfg) =>
        {
            var kebabFormatter =  new KebabCaseEndpointNameFormatter(false);
            cfg.ReceiveEndpoint(
                queueName: kebabFormatter.SanitizeName(nameof(SendWebhook)), 
                ep  => { 
                    ep.MaxDeliveryCount = 1;
                    ep.DefaultMessageTimeToLive = TimeSpan.FromMinutes(1); 
                    ep.LockDuration = TimeSpan.FromMinutes(1);
                    ep.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                    ep.MaxSizeInMegabytes = 1024;
                    ep.PublishFaults = false;
                    ep.EnableDeadLetteringOnMessageExpiration = false;
                    ep.ConfigureConsumer<SendWebhookConsumer>(ctx);
                });
        });
        return services;
    }
}