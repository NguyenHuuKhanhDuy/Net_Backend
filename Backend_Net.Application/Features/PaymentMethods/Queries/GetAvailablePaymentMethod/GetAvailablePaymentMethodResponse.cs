using Backend_Net.Application.Common.Models;
using Backend_Net.Domain.Entities;

namespace Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;

public class GetAvailablePaymentMethodResponse : BaseResponse
{
    public List<GetAvailablePaymentMethodData> Data { get; set; } = new();
}

public class GetAvailablePaymentMethodData
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? BackgroundUrl { get; set; }
}