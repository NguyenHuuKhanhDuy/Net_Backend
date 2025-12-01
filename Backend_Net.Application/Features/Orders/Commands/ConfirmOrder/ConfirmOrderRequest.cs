namespace Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderRequest
{
    public string Token { get; set; }
    public Guid MethodId { get; set; }
    public int CountryId { get; set; }
}