using Backend_Net.Api.Middlewares;

namespace Backend_Net.Api.StartupRegistrations;

public static class RequestLogRegistration
{
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        return app;
    }
}