namespace Backend_Net.Infrastructure.Options;

public class AppOptions
{
    public static readonly string OptionName = "App";
    public string Name { get; set; }
    public string HostingUrl { get; set; }
    public string ClientSecret { get; set; }
}