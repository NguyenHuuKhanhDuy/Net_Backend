using FluentValidation;

namespace Backend_Net.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceValidator()
    {
        // Add validation rules here if needed
    }
}