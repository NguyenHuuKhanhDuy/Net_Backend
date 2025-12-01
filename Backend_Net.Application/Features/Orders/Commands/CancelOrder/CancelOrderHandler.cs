using System.Net;
using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Models.Dtos;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Application.Services.Webhook;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, CancelOrderResponse>
{
    private readonly ILogger<CancelOrderHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentTokenService _paymentTokenService;

    public CancelOrderHandler
    (
        ILogger<CancelOrderHandler> logger,
        IUnitOfWork unitOfWork,
        IPaymentTokenService paymentTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentTokenService = paymentTokenService;
    }

    #region Implementation of IRequestHandler<in CancelOrderCommand, CancelOrderResponse>

    public async Task<CancelOrderResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CancelOrderHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CancelOrderResponse { Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var extractedToken = _paymentTokenService.ExtractPaymentTokenAsync(payload.Token, cancellationToken);
            if (!extractedToken.Success)
            {
                _logger.LogWarning("{FunctionName} failed to extract payment token: {ErrorMessage}", functionName,
                    extractedToken.ErrorMessage);
                response.ErrorMessage = extractedToken.ErrorMessage;
                response.ErrorMessageCode = extractedToken.ErrorMessageCode;
                response.WithStatus(extractedToken.StatusCode);
                return response;
            }

            var paymentToken = extractedToken.Data;
            if (paymentToken is null || paymentToken.ExpireAt < DateTime.UtcNow)
            {
                _logger.LogWarning("{Fn:l} Token is expired", functionName);
                response
                    .WithMessage(ErrorCode.INV_ERR_001)
                    .WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }

            var transaction = await _unitOfWork.PaymentTransaction
                .Where(x => x.Id == paymentToken.PaymentTransactionId)
                .FirstOrDefaultAsync(cancellationToken);
            if (transaction == null)
            {
                _logger.LogError(
                    $"{functionName} payment transaction not found for id: {paymentToken.PaymentTransactionId}");
                response.WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }

            transaction.Status = PaymentStatus.Canceled;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveAsync(cancellationToken);

            response.PaymentTransactionId = transaction.Id;
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
        }

        return response;
    }

    #endregion
}