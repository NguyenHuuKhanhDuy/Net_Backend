using Backend_Net.Application.Common.Models;
using FluentValidation;

namespace Backend_Net.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(
                BaseResponse.Fail(ex.Message, "VALIDATION_ERROR")
            );
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(
                BaseResponse.Fail(ex.Message, "UNEXPECTED_ERROR")
            );
        }
    }
}