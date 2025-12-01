using Backend_Net.Job.JobRegistry;
using Backend_Net.Job.Options;
using Cronos;
using Microsoft.Extensions.Options;

namespace Backend_Net.Job.CronJobs;

public class CronJobManagerHostedService : BackgroundService
{
    private readonly IJobFactory _jobFactory;
    private readonly JobScheduleOptions _options;
    private readonly IServiceProvider _provider;

    public CronJobManagerHostedService(
        IJobFactory jobFactory,
        IOptions<JobScheduleOptions> options,
        IServiceProvider provider)
    {
        _jobFactory = jobFactory;
        _options = options.Value;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobs = _jobFactory.CreateJobs(_options);

        var tasks = jobs.Select(job =>
            RunCronJob(job, stoppingToken)
        );

        await Task.WhenAll(tasks);
    }

    private async Task RunCronJob(JobRegistration job, CancellationToken ct)
    {
        var cron = CronExpression.Parse(job.Cron, CronFormat.IncludeSeconds);

        while (!ct.IsCancellationRequested)
        {
            var next = cron.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            if (next == null)
                return;

            var delay = next.Value - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, ct);

            using var scope = _provider.CreateScope();
            var executor = scope.ServiceProvider.GetRequiredService<IJobExecutor>();

            await executor.ExecuteAsync(job.Endpoint, ct);
        }
    }
}