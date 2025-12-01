namespace Backend_Net.Infrastructure.Options;

public class RabbitMQOptions
{
    public const string OptionName = "RabbitMQ";
    public string Host { get; set; } = string.Empty;
    public ushort Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}