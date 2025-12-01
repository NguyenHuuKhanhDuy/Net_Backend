using System.Reflection;
using Backend_Net.Application.Common.Interfaces.MassTransit;
using Backend_Net.Domain.Enums;
using Backend_Net.Infrastructure.Options;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;

namespace Backend_Net.Infrastructure.MassTransit;

public static class MassTransitRegistration
{
    public static IServiceCollection AddMassTransitRegistration
    (
        this IServiceCollection services, 
        IConfiguration configuration, 
        Assembly? entryAssembly = null, 
        Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator>? registrationConfigure = null,
        Func<Type, bool>? customConsumerFilter = null
    )
    {
        var massTransitBroker = configuration.GetValue<MassTransitBroker>("MassTransitBroker");
        var azureServiceBusOptions = configuration.GetOptions<AzureServiceBusOptions>(AzureServiceBusOptions.OptionName);
        var rabbitMqOptions = configuration.GetOptions<RabbitMQOptions>(RabbitMQOptions.OptionName);
        
        services.AddMassTransit(x =>
        {
            if (entryAssembly is not null)
            {
                x.AddConsumers(customConsumerFilter, entryAssembly);
            }
            
            x.SetKebabCaseEndpointNameFormatter();
            
            if (massTransitBroker is MassTransitBroker.AzureServiceBus)
            {
                x.AddConfigureEndpointsCallback((_, cfg) =>
                {
                    if(cfg is IServiceBusEndpointConfigurator sb)
                    {
                        sb.DefaultMessageTimeToLive = TimeSpan.FromDays(azureServiceBusOptions.DefaultMessageTimeToLive);
                        sb.AutoDeleteOnIdle =  TimeSpan.FromDays(azureServiceBusOptions.DefaultMessageTimeToLive + 1);
                        sb.LockDuration = TimeSpan.FromMinutes(1);
                        sb.EnableDeadLetteringOnMessageExpiration = false;
                        sb.MaxDeliveryCount = azureServiceBusOptions.MaxDeliveryCount;
                    }
                });

                x.UsingAzureServiceBus((ctx, cfg) =>
                {
                    cfg.UseRawJsonSerializer();
                    cfg.Host(azureServiceBusOptions.ConnectionString);
                    registrationConfigure?.Invoke(ctx, cfg);
                    cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(false));
                });
            }
            else
            {
                x.UsingRabbitMq((ctx,cfg) =>
                {
                    cfg.UseRawJsonSerializer();
                    cfg.Host(rabbitMqOptions.Host, rabbitMqOptions.Port,"/", h => 
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });
                    cfg.ConfigureEndpoints(ctx);
                });
            }
        });

        services.AddScoped<IMessageSender, SendEndpointCustomProvider>();
        services.AddHostedService<MassTransitHostedService>();

        return services;
    }
}