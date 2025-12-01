using Backend_Net.Job.CronJobs;

namespace Backend_Net.Job.JobRegistry;

public record JobRegistration(
    string Name,
    string Cron,
    string Endpoint,
    IJobExecutor Executor
);