namespace Backend_Net.Job.CronJobs;

public interface IJobExecutor
{
    string Name { get; }
    Task ExecuteAsync(string endpoint, CancellationToken token);
}