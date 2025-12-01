using Backend_Net.Job.Options;

namespace Backend_Net.Job.JobRegistry;

public interface IJobFactory
{
    IEnumerable<JobRegistration> CreateJobs(JobScheduleOptions options);
}