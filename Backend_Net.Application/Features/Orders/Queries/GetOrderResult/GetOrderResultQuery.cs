using MediatR;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderResult;

public class GetOrderResultQuery : IRequest<GetOrderResultResponse>
{
    public string Token { get; set; }

    public GetOrderResultQuery(string token)
    {
        Token = token;
    }
}