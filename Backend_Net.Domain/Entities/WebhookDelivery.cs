using Backend_Net.Domain.Enums;

namespace Backend_Net.Domain.Entities;

public class WebhookDelivery
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int Attempt { get; set; }
    public int MaxAttempt { get; set; } = 10;
    public WebhookDeliveryStatus Status { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int? HttpStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RetryAt { get; set; }
    public WebhookEvent Event { get; set; } = null!;
    
    public TimeSpan GetBackoffDelay(int attempt)
    {
        return attempt switch
        {
            1 => TimeSpan.Zero,
            2 => TimeSpan.FromSeconds(10),
            3 => TimeSpan.FromSeconds(30),
            4 => TimeSpan.FromMinutes(2),
            5 => TimeSpan.FromMinutes(10),
            6 => TimeSpan.FromHours(1),
            7 => TimeSpan.FromHours(3),
            8 => TimeSpan.FromHours(6),
            9 => TimeSpan.FromHours(12),
            _ => TimeSpan.FromHours(24)
        };
    }
}