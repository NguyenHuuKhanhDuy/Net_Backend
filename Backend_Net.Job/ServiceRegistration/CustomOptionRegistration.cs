using Backend_Net.Job.Options;

namespace Backend_Net.Job.ServiceRegistration;

public static class CustomOptionRegistration
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ApiOptions>(config.GetSection(ApiOptions.OptionName));
        return services;
    }
}