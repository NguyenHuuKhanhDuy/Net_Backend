using System.Globalization;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Backend_Net.Api.StartupRegistrations;

public static class LocalizationRegistration
{
    public static IServiceCollection AddLocalizationLayer(this IServiceCollection services)
    {
        services.AddLocalization(o => o.ResourcesPath = "Resources");

        services
            .AddControllers()
            .AddDataAnnotationsLocalization()
            .AddViewLocalization();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("vi")
            };

            options.DefaultRequestCulture = new RequestCulture("en");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });

        return services;
    }

    public static IApplicationBuilder UseRequestLocalizationLayer(this IApplicationBuilder app)
    {
        var opts = app.ApplicationServices.GetRequiredService<
            IOptions<RequestLocalizationOptions>>();

        app.UseRequestLocalization(opts.Value);
        return app;
    }

    public static void InitLocalization(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<ILocalizationService>();
        LocalizationAccessor.Configure(localizer);
    }
}