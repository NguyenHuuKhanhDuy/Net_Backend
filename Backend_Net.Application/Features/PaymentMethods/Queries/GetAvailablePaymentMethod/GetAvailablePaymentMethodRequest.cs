namespace Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;

public class GetAvailablePaymentMethodRequest
{
    public string Token { get; set; }
    public int CountryId { get; set; }
}