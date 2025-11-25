namespace Backend_Net.Application.Features.RequestLog.Commands.CreateRequestLog;

public class CreateRequestLogRequest
{
    public Guid RequestId {get; set; }
    public string Path {get; set; }
    public string Method {get; set; }
    public string Headers {get; set; }
    public string? Body {get; set; }
    public string? ResponseBody {get; set; }
    public int ResponseStatus {get; set; }
    public int DurationMs {get; set; }
    public string? IpAddress {get; set; }
    public string? UserAgent {get; set; }
    public string? ErrorMessage {get; set; }
    public string? QueryParams { get; set; }
}