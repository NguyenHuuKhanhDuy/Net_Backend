using System.Net;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Options;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderInfo;

public class GetOrderInfoHandler : IRequestHandler<GetOrderInfoQuery, GetOrderInfoResponse>
{
    private readonly ILogger<GetOrderInfoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppOptions _appOptions;
    private readonly IPaymentTokenService _paymentTokenService;

    public GetOrderInfoHandler
    (
        ILogger<GetOrderInfoHandler> logger,
        IUnitOfWork unitOfWork,
        IOptions<AppOptions> appOptions,
        IPaymentTokenService paymentTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _appOptions = appOptions.Value;
        _paymentTokenService = paymentTokenService;
    }

    #region Implementation of IRequestHandler<in GetOrderInfoQuery, GetOrderInfoResponse>

    public async Task<GetOrderInfoResponse> Handle(GetOrderInfoQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetOrderInfoHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetOrderInfoResponse { Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var extractedToken = _paymentTokenService.ExtractPaymentTokenAsync(request.Token, cancellationToken);
            if (!extractedToken.Success)
            {
                _logger.LogWarning("{FunctionName} failed to extract payment token: {ErrorMessage}", functionName, extractedToken.ErrorMessage);
                response.ErrorMessage = extractedToken.ErrorMessage;
                response.ErrorMessageCode = extractedToken.ErrorMessageCode;
                response.WithStatus(HttpStatusCode.BadRequest);
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
            
            var paymentTransaction = await _unitOfWork.PaymentTransaction
                .Where(x => x.Id == paymentToken.PaymentTransactionId)
                .Select(x => new
                {
                    OrderId = x.OrderId, 
                    Description = x.Description, 
                    Amount = x.Amount, 
                    UserId = x.UserId,
                    Status = x.Status,
                    Currency = x.TenantCurrencyCode,
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (paymentTransaction == null)
            {
                _logger.LogError("{functionName:l} payment transaction not found for id: {paymentTransactionId:l}", functionName, paymentToken.PaymentTransactionId);
                response
                    .WithStatus(HttpStatusCode.NotFound)
                    .WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }

            if (paymentTransaction.Status != PaymentStatus.Created)
            {
                _logger.LogInformation("{FunctionName:l} payment transaction status is not valid: {Status:l}", functionName, paymentTransaction.Status);
                response
                    .WithStatus(HttpStatusCode.BadRequest)
                    .WithMessage(ErrorCode.PMT_ERR_002);
                return response;
            }

            response.Data = new GetOrderInfoData
            {
                OrderNumber = paymentTransaction.OrderId,
                Amount = paymentTransaction.Amount,
                Description = paymentTransaction.Description,
                UserId = paymentTransaction.UserId,
                Status = paymentTransaction.Status,
                Currency = paymentTransaction.Currency
            };
            response.WithSuccess(true).WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            ex.LogError(_logger, functionName);
        }

        return response;
    }

    #endregion
}