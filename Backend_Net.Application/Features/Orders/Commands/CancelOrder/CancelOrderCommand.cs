using MediatR;

namespace Backend_Net.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<CancelOrderResponse>
{
    public CancelOrderRequest Payload { get; set; }

    public CancelOrderCommand(CancelOrderRequest payload)
    {
        Payload = payload;
    }
}