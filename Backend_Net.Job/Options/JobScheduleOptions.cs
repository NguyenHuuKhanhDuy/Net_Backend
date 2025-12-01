namespace Backend_Net.Job.Options;

public class JobScheduleOptions : Dictionary<string, JobOption>
{
    public static readonly string OptionName = "Jobs";
}

public class JobOption
{
    public bool Enabled { get; set; }
    public string Cron { get; set; } = default!;
    public string Endpoint { get; set; } = default!;
}