using Backend_Net.Application.Common.Models;

namespace Backend_Net.Application.Features.Country.Queries.GetCountries;

public class GetCountriesResponse : BaseResponse
{
    public List<GetCountriesData> Data { get; set; } = new();
}

public class GetCountriesData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DialCode { get; set; }
    public string Currency { get; set; }
    public string FlagUrl { get; set; }
}