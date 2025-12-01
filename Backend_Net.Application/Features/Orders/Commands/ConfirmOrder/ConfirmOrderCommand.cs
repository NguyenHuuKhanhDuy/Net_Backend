using MediatR;

namespace Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderCommand : IRequest<ConfirmOrderResponse>
{
    public ConfirmOrderRequest Payload { get; set; }

    public ConfirmOrderCommand(ConfirmOrderRequest payload)
    {
        Payload = payload;
    }
}