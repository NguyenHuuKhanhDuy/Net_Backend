using Backend_Net.Application.Common.Models;
using Backend_Net.Application.Models.Dtos;

namespace Backend_Net.Application.Services.PaymentToken;

public interface IPaymentTokenService
{
    string GeneratePaymentTokenAsync(Guid transactionId, DateTime expireIn, CancellationToken cancellationToken = default);
    BaseResponse<PaymentTokenDto> ExtractPaymentTokenAsync(string token, CancellationToken cancellationToken = default);
}