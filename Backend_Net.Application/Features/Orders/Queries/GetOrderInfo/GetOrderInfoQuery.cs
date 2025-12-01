using MediatR;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderInfo;

public class GetOrderInfoQuery : IRequest<GetOrderInfoResponse>
{
    public string Token { get; set; }
    public GetOrderInfoQuery(string token)
    {
        Token = token;
    }
}