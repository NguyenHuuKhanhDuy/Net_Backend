using Backend_Net.Application.Common.Models;
using Backend_Net.Domain.Enums;

namespace Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderResponse : BaseResponse<ConfirmOrderData>
{
}

public class ConfirmOrderData
{
    public PaymentMethodEnum PaymentMethodType { get; set; }
    public string OrderNumber { get; set; }
    public decimal Amount { get; set; }
    public string? UserId { get; set; }
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; }
    public string TenantCurrency { get; set; }
    public string UserCurrency { get; set; }
    public decimal EquivalentAmount { get; set; }
}