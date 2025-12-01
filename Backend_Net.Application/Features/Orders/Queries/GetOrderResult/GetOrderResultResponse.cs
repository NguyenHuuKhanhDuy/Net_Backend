using Backend_Net.Application.Common.Models;
using Backend_Net.Domain.Enums;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderResult;

public class GetOrderResultResponse : BaseResponse<GetOrderResultData>
{

}

public class GetOrderResultData
{
    public string OrderNumber { get; set; }
    public decimal Amount { get; set; }
    public string? UserId { get; set; }
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; }
    public string Currency { get; set; }
    public string? RedirectUrl { get; set; }
}