using Backend_Net.Application.Common.Models;
namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.CreatePaymentSession;

public class CreatePaymentSessionResponse: BaseResponse<CreatePaymentSessionData>
{
    
}

public class CreatePaymentSessionData
{
    public string PaymentUrl { get; set; }
}