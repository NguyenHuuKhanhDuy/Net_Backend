namespace Backend_Net.Infrastructure.Options;

public class AzureServiceBusOptions
{
    public const string OptionName = "AzureServiceBus";
    public string ConnectionString { get; set; } = "";
    public int MaxConcurrentCalls { get; set; }
    public int MaxDeliveryCount { get; set; }
    public int DefaultMessageTimeToLive { get; set; }
    public int MaxSizeInMegabytes { get; set; }
}