using Backend_Net.Application.Common.Models;
using Backend_Net.Domain.Enums;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderInfo;

public class GetOrderInfoResponse: BaseResponse<GetOrderInfoData>
{
}

public class GetOrderInfoData
{
    public string OrderNumber { get; set; }
    public decimal Amount { get; set; }
    public string? UserId { get; set; }
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; }
    public string Currency { get; set; }
}