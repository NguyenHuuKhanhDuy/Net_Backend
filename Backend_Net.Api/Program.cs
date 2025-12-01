using Backend_Net.Api.StartupRegistrations;
using Backend_Net.Application;
using Backend_Net.Application.ServiceRegistration;
using Backend_Net.Infrastructure;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.Host.UseLogging();

// 2. Register services
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddLocalizationLayer()
    .AddCorsLayer(builder.Configuration)
    .AddSwaggerLayer()
    .AddControllersLayer()
    .AddCustomHttpContextAccessor();

var app = builder.Build();

// 3. Middleware pipeline
app.UseRequestLocalizationLayer();
app.UseSwaggerLayer();
app.UseSerilogRequestLogging();
app.UseCors("_allowSpecificOrigins");
app.UseRequestLogging();
app.UseExceptionLayer();

// 4. Init localization service
app.InitLocalization();

// 5. Map endpoints
app.MapControllers();

app.Run();