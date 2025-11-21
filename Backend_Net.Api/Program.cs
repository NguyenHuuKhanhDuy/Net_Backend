using System.Globalization;
using Backend_Net.Api.Middlewares;
using Backend_Net.Api.Options;
using Backend_Net.Application;
using Backend_Net.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
// Localization
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

builder.Services
    .AddControllers()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
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

// Options Pattern
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddCors(o =>
{
    o.AddPolicy("Default", p =>
    {
        p.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRequestLocalization(
    app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Default");

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();