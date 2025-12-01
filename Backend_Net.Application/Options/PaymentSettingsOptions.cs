namespace Backend_Net.Application.Options;

public class PaymentSettingsOptions
{
    public static readonly string OptionName = "PaymentSettings";
    public StripeSettings Stripe { get; set; }
}

public class StripeSettings
{
    public string ApiKey { get; set; }
    public string WebhookSecret { get; set; }
}