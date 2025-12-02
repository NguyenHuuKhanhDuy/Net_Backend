using Backend_Net.Api.Helpers;
using Backend_Net.Application.Features.Country.Queries.GetCountries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Net.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : Controller
{
    private readonly IMediator _mediator;
    
    public CountriesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetCountriesQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}