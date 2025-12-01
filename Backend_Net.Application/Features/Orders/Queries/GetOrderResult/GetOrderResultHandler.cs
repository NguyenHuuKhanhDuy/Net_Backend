using System.Net;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Services.PaymentToken;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Backend_Net.Application.Features.Orders.Queries.GetOrderResult;

public class GetOrderResultHandler : IRequestHandler<GetOrderResultQuery, GetOrderResultResponse>
{
    private readonly ILogger<GetOrderResultHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentTokenService _paymentTokenService;

    public GetOrderResultHandler
    (
        ILogger<GetOrderResultHandler> logger,
        IUnitOfWork unitOfWork,
        IPaymentTokenService paymentTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentTokenService = paymentTokenService;
    }

    #region Implementation of IRequestHandler<in GetOrderResultQuery, GetOrderResultResponse>

    public async Task<GetOrderResultResponse> Handle(GetOrderResultQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetOrderResultHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetOrderResultResponse {Success = false, StatusCode = HttpStatusCode.InternalServerError };

        try
        {
            var extractedToken = _paymentTokenService.ExtractPaymentTokenAsync(request.Token, cancellationToken);
            if (!extractedToken.Success)
            {
                _logger.LogWarning("{FunctionName} failed to extract payment token: {ErrorMessage}", functionName, extractedToken.ErrorMessage);
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
                    RedirectUrl = x.ReturnUrl
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (paymentTransaction == null)
            {
                _logger.LogError($"{functionName} payment transaction not found for id: {paymentToken.PaymentTransactionId}");
                response.WithMessage(ErrorCode.PMT_ERR_001);
                return response;
            }
            
            response.Data = new GetOrderResultData
            {
                OrderNumber = paymentTransaction.OrderId,
                Amount = paymentTransaction.Amount,
                Description = paymentTransaction.Description,
                UserId = paymentTransaction.UserId,
                Status = paymentTransaction.Status,
                Currency = paymentTransaction.Currency,
                RedirectUrl = paymentTransaction.RedirectUrl
            };
            response.WithSuccess(true).WithStatus(HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            e.LogError(_logger, functionName);
        }
        
        return response;
    }

    #endregion
}