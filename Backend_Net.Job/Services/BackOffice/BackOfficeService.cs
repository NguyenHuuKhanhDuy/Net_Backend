using System.Text;
using Backend_Net.Job.Options;

namespace Backend_Net.Job.Services.BackOffice;

public class BackOfficeService : IBackOfficeService
{
    private readonly ILogger<BackOfficeService> _logger;
    private readonly HttpClient _httpClient;
    
    public BackOfficeService(
        ILogger<BackOfficeService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(ApiOptions));
    }


    public async Task JobSendWebhookAsync(string path, CancellationToken cancellationToken)
    {
        const string functionName = $"{nameof(BackOfficeService)} - {nameof(JobSendWebhookAsync)} =>";
        _logger.LogInformation(functionName);

        try
        {
            await _httpClient.PostAsync(path, new StringContent("", Encoding.UTF8, "application/json"), cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError($"{functionName} Has error: {ex.Message}");
        }
    }
}