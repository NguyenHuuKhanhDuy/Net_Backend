namespace Backend_Net.Job.Services.BackOffice;

public interface IBackOfficeService 
{
    Task JobSendWebhookAsync(string path, CancellationToken cancellationToken);
}