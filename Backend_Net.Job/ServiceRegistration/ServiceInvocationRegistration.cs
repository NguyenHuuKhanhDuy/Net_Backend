using Backend_Net.Job.Options;
using Shared.Extensions;

namespace Backend_Net.Job.ServiceRegistration;

public static class ServiceInvocationRegistration
{
    public static IServiceCollection AddServiceInvocation(this IServiceCollection services, IConfiguration configuration)
    {
        var apiOptions = configuration.GetOptions<ApiOptions>(ApiOptions.OptionName);
        services.AddHttpClient(nameof(ApiOptions), c =>
        {
            c.BaseAddress = new Uri(apiOptions.Url);
        });
        
        return services;
    }
}