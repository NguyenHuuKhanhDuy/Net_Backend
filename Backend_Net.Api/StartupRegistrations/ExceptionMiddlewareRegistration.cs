using Backend_Net.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Backend_Net.Api.StartupRegistrations;

public static class ExceptionMiddlewareRegistration
{
    public static WebApplication UseExceptionLayer(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}