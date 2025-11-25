namespace Backend_Net.Domain.Entities;

public class RequestLog
{
    public Guid Id { get; set; }
    public Guid? TenantCredentialId { get; set; }
    public string Path { get; set; } = null!;
    public string Method { get; set; } = null!;

    public string? Headers { get; set; }
    public string? QueryParams { get; set; }
    public string? Body { get; set; }
    public int? ResponseStatus { get; set; }
    public string? ResponseBody { get; set; }
    public int? DurationMs { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public TenantCredential? TenantCredential { get; set; }
}