namespace Backend_Net.Domain.Entities;

public class WebhookDelivery
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public int Attempt { get; set; }
    public string Status { get; set; } = null!; // PENDING / SUCCESS / FAILED

    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public int? HttpStatus { get; set; }
    public string? ErrorMessage { get; set; }

    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public WebhookEvent Event { get; set; } = null!;
}