using System.Globalization;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Domain.Enums;
using Shared.Helpers;

namespace Backend_Net.Application.Models.Dtos;

public class WebhookDto
{
    public Guid Id { get; set; }
    public string ReferenceId { get; set; }
    public string Amount { get; set; }
    public string Currency { get; set; }
    public string? UserId { get; set; }
    public string? CallbackData { get; set; }
    public string Status { get; set; }
    public string ReturnUrl { get; set; }
    public string CallbackUrl { get; set; }
    public string CreatedAt { get; set; }
    
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { "id", Id.ToString() },
            { "amount", Amount.ToString(CultureInfo.InvariantCulture) },
            { "currency", Currency },
            { "referenceId", ReferenceId },
            { "returnUrl", ReturnUrl },
            { "callbackUrl", CallbackUrl },
            { "callbackData", CallbackData is null ? string.Empty : JsonHelper.Serialize(CallbackData) },
            { "userId", UserId ?? string.Empty },
            { "status", Status},
            { "createdAt", CreatedAt },
        };

        return dict;
    }
}