namespace Backend_Net.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceRequest
{
    public string Token { get; set; }
    public Guid MethodId { get; set; }
    public int CountryId { get; set; }
}