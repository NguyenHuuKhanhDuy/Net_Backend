namespace Backend_Net.Application.Options;

public class AppOptions
{
    public static readonly string OptionName = "App";
    public string Name { get; set; }
    public string HostingUrl { get; set; }
    public string ClientSecret { get; set; }
    public int TokenExpirationInMinutes { get; set; }
}