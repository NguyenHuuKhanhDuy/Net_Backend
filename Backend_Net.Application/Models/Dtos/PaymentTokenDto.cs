namespace Backend_Net.Application.Models.Dtos;

public class PaymentTokenDto
{
    public Guid PaymentTransactionId { get; set; }
    public DateTime ExpireAt { get; set; }
}