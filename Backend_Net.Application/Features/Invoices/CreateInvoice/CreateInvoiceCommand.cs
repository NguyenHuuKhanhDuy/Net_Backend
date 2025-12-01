using MediatR;

namespace Backend_Net.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceCommand : IRequest<CreateInvoiceResponse>
{
    public CreateInvoiceRequest Payload { get; set; }

    public CreateInvoiceCommand(CreateInvoiceRequest payload)
    {
        Payload = payload;
    }
}