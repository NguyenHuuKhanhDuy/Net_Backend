using Backend_Net.Job.CronJobs;
using Backend_Net.Job.Options;

namespace Backend_Net.Job.JobRegistry;

public class JobFactory : IJobFactory
{
    private readonly IEnumerable<IJobExecutor> _executors;

    public JobFactory(IEnumerable<IJobExecutor> executors)
    {
        _executors = executors;
    }

    public IEnumerable<JobRegistration> CreateJobs(JobScheduleOptions options)
    {
        foreach (var (name, cfg) in options)
        {
            if (!cfg.Enabled)
                continue;

            var executor = _executors.FirstOrDefault(x => x.Name == name);
            if (executor == null)
                continue;

            yield return new JobRegistration(name, cfg.Cron, cfg.Endpoint, executor);
        }
    }
}