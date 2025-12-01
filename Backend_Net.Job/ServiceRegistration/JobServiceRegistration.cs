using Backend_Net.Job.CronJobs;
using Backend_Net.Job.CronJobs.Executors;
using Backend_Net.Job.JobRegistry;
using Backend_Net.Job.Options;
using Backend_Net.Job.Services.BackOffice;

namespace Backend_Net.Job.ServiceRegistration;

public static class JobServiceRegistration
{
    public static IServiceCollection AddScheduledJobs(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JobScheduleOptions>(config.GetSection(JobScheduleOptions.OptionName));

        services.AddScoped<IBackOfficeService, BackOfficeService>();
        services.AddScoped<IJobExecutor, SendWebhookJobExecutor>();
        services.AddSingleton<IJobFactory, JobFactory>();

        services.AddHostedService<CronJobManagerHostedService>();

        return services;
    }
}