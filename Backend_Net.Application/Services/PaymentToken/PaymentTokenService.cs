using System.Net;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Models;
using Backend_Net.Application.Models.Dtos;
using Backend_Net.Application.Options;
using Backend_Net.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Helpers;

namespace Backend_Net.Application.Services.PaymentToken;

public class PaymentTokenService : IPaymentTokenService
{
    private readonly AppOptions _appOptions;
    private readonly ILogger<PaymentTokenService> _logger;

    public PaymentTokenService
    (
        ILogger<PaymentTokenService> logger,
        IOptions<AppOptions> appOptions
    )
    {
        _logger = logger;
        _appOptions = appOptions.Value;
    }

    public string GeneratePaymentTokenAsync(Guid transactionId, DateTime expireAt, CancellationToken cancellationToken = default)
    {
        var paymentToken = new PaymentTokenDto
        {
            PaymentTransactionId = transactionId,
            ExpireAt = expireAt
        };

        return CryptographyHelper.Encrypt(JsonHelper.Serialize(paymentToken), _appOptions.ClientSecret)
            .ToBase64Encode();
    }

    public BaseResponse<PaymentTokenDto> ExtractPaymentTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var functionName = $"{nameof(PaymentTokenService)} - {nameof(ExtractPaymentTokenAsync)} Token={token} =>";
        var tokenDecrypted = CryptographyHelper.Decrypt(token.ToBase64Decode(), _appOptions.ClientSecret);
        var response = new BaseResponse<PaymentTokenDto> { StatusCode = HttpStatusCode.InternalServerError, Success = false };
        if (string.IsNullOrEmpty(tokenDecrypted))
        {
            _logger.LogError($"{functionName} can not decrypt token");
            response.WithMessage(ErrorCode.INV_ERR_001);
            return response;
        }

        var paymentToken = JsonHelper.Deserialize<PaymentTokenDto>(tokenDecrypted);
        response
            .WithData(paymentToken)
            .WithSuccess(true)
            .WithStatus(HttpStatusCode.OK);
        return response;
    }
}