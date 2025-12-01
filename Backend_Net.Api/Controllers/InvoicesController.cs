using Backend_Net.Api.Attributes;
using Backend_Net.Api.Helpers;
using Backend_Net.Application.Features.Invoices.CreateInvoice;
using Backend_Net.Application.Features.Orders.Commands.CancelOrder;
using Backend_Net.Application.Features.Orders.Commands.ConfirmOrder;
using Backend_Net.Application.Features.Orders.Commands.CreateOrder;
using Backend_Net.Application.Features.Orders.Queries.GetOrderInfo;
using Backend_Net.Application.Features.Orders.Queries.GetOrderResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : Controller
{
    private readonly IMediator _mediator;
    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Create")]
    [EnableRequestLogging]
    public async Task<IActionResult> CreateOrder([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateInvoiceCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}