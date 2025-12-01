using Backend_Net.Api.Attributes;
using Backend_Net.Api.Helpers;
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
    
    [HttpGet("Info")]
    public async Task<IActionResult> GetOrderInfo([FromQuery] string token)
    {
        var response = await _mediator.Send(new GetOrderInfoQuery(token));
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("Result")]
    public async Task<IActionResult> GetOrderResult([FromQuery] string token)
    {
        var response = await _mediator.Send(new GetOrderResultQuery(token));
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("Confirm")]
    public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderRequest request)
    {
        var response = await _mediator.Send(new ConfirmOrderCommand(request));
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("Cancel")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
    {
        var response = await _mediator.Send(new CancelOrderCommand(request));
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
}