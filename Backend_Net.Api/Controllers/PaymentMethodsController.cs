using Backend_Net.Api.Helpers;
using Backend_Net.Application.Features.PaymentMethods.Queries.GetAvailablePaymentMethod;
using MediatR;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodsController : Controller
{
    private readonly IMediator _mediator;
    public PaymentMethodsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("Available")]
    public async Task<IActionResult> GetPaymentMethod([FromQuery] GetAvailablePaymentMethodRequest payload)
    {
        var response = await _mediator.Send(new GetAvailablePaymentMethodQuery(payload));
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}