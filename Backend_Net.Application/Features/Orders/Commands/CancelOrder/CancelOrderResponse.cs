using Backend_Net.Application.Common.Models;
using Backend_Net.Application.Models.Dtos;

namespace Backend_Net.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderResponse : BaseResponse
{
    internal Guid PaymentTransactionId { get; set; }
}