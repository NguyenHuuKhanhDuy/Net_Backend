using Backend_Net.Job.Services.BackOffice;

namespace Backend_Net.Job.CronJobs.Executors;

public class SendWebhookJobExecutor : IJobExecutor
{
    private readonly IBackOfficeService _backOffice;
    public string Name => "SendWebhook";

    public SendWebhookJobExecutor(IBackOfficeService backOffice)
    {
        _backOffice = backOffice;
    }

    public async Task ExecuteAsync(string path, CancellationToken token)
    {
        try
        {
            await _backOffice.JobSendWebhookAsync(path, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{Name}] ERROR: {ex.Message}");
        }
    }
}