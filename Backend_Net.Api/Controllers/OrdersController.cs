using Backend_Net.Api.Attributes;
using Backend_Net.Api.Helpers;
using Backend_Net.Application.Features.Orders.Commands.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.Controllers;

[Route("api/[controller]")]
public class OrdersController : Controller
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Create")]
    [EnableRequestLogging]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var headers = Request.Headers;
        var apiKey = headers["X-API-KEY"].FirstOrDefault();
        var signature = headers["X-Signature"].FirstOrDefault();
        
        var response = await _mediator.Send(new CreateOrderCommand(request, apiKey, signature));
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}