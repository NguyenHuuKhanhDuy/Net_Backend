using MediatR;

namespace Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;

public class GetAvailablePaymentMethodQuery : IRequest<GetAvailablePaymentMethodResponse>
{
    public GetAvailablePaymentMethodRequest Payload { get; }

    public GetAvailablePaymentMethodQuery(GetAvailablePaymentMethodRequest payload)
    {
        Payload = payload;
    }
}