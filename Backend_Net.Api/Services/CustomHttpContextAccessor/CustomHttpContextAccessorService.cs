using Backend_Net.Application.Common.Interfaces;

namespace Backend_Net.Api.Services.CustomHttpContextAccessor;

public class CustomHttpContextAccessorService : ICustomHttpContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CustomHttpContextAccessorService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetApiKey()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return string.Empty;
        }
        
        var apiKey = context.Request.Headers["X-API-KEY"].ToString();
        return string.IsNullOrEmpty(apiKey) ? string.Empty : apiKey;
    }
}