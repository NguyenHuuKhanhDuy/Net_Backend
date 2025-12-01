namespace Backend_Net.Job.Options;

public class ApiOptions
{
    public static readonly string OptionName = "Api";
    public string Url { get; set; } = string.Empty;
    public int HttpClientTimeout { get; set;  }
}