using System.Globalization;
using Backend_Net.Application.Common.Helpers;
using Shared.Helpers;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string ReferenceId { get; set; }
    public object? CallbackData { get; set; }
    public string ReturnUrl { get; set; }
    public string CallbackUrl { get; set; }
    public string? UserId { get; set; }
    
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { "amount", Amount.ToString(CultureInfo.InvariantCulture) },
            { "currency", Currency },
            { "referenceId", ReferenceId },
            { "returnUrl", ReturnUrl },
            { "callbackUrl", CallbackUrl },
            { "callbackData", CallbackData is null ? string.Empty : JsonHelper.Serialize(CallbackData) },
            { "userId", UserId ?? string.Empty },
        };

        return dict;
    }
}