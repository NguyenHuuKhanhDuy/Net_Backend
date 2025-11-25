using Microsoft.Extensions.Logging;

namespace Backend_Net.Application.Common.Extensions;

public static class ExceptionExtensions
{
    public static void LogError(this Exception ex, ILogger logger, string logContext)
    {
        logger.LogError("{logContext:l} Has Error. Message: {message:l}", logContext, ex.Message);
    }
}