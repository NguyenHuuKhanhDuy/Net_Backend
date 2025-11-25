using Backend_Net.Application.Common.Models;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderResponse : BaseResponse<CreateOrderData>
{
}

public class CreateOrderData
{
    public Guid Id { get; set; }
    public string PaymentUrl { get; set; }
    public decimal Fee { get; set; }
}