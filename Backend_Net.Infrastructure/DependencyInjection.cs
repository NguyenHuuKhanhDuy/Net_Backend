using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Infrastructure.Localization;
using Backend_Net.Infrastructure.Options;
using Backend_Net.Infrastructure.Options.Logging;
using Backend_Net.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ILocalizationService, LocalizationService>();
        return services;
    }
}