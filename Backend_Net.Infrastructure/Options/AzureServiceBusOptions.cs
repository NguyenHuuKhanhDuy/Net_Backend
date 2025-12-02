namespace Backend_Net.Infrastructure.Options;

public class AzureOptions
{
    public const string OptionName = "AzureConfiguration";
    public AzureServiceBusSettings ServiceBus { get; set; }
    public KeyVaultSettings KeyVault { get; set; }
}

public class AzureServiceBusSettings
{
    public string ConnectionString { get; set; } = "";
    public int MaxConcurrentCalls { get; set; }
    public int MaxDeliveryCount { get; set; }
    public int DefaultMessageTimeToLive { get; set; }
    public int MaxSizeInMegabytes { get; set; }
}

public class KeyVaultSettings
{
    public string Url { get; set; }
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}