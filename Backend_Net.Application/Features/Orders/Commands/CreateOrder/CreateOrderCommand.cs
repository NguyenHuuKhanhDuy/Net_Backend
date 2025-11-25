using MediatR;

namespace Backend_Net.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public CreateOrderRequest Payload { get; set; }
    public string? ApiKey { get; set; }
    public string? Signature { get; set; }
    
    public CreateOrderCommand(CreateOrderRequest payload, string? apiKey, string? signature)
    {
        Payload = payload;
        ApiKey = apiKey;
        Signature = signature;
    }
}