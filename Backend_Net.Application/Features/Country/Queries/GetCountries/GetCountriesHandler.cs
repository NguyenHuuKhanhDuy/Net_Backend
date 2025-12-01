using System.Net;
using Backend_Net.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Application.Features.Country.Queries.GetCountries;

public class GetCountriesHandler : IRequestHandler<GetCountriesQuery, GetCountriesResponse>
{
    private readonly ILogger<GetCountriesHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetCountriesHandler(ILogger<GetCountriesHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetCountriesQuery, GetCountriesResponse>

    public async Task<GetCountriesResponse> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetCountriesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetCountriesResponse { Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var countries = await _unitOfWork.Country
                .GetAll()
                .AsNoTracking()
                .Select(x => new GetCountriesData
                {
                    Id = x.Id,
                    Name = x.Name,
                    DialCode = x.DialCode,
                    Currency = x.CurrencyCode,
                    FlagUrl = x.FlagUrl
                })
                .ToListAsync(cancellationToken);

            response.Data = countries;
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return response;
    }

    #endregion
}