using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Application.Features.PaymentProvider.Stripe.Command.CreatePaymentSession;

public class CreatePaymentSessionHandler : IRequestHandler<CreatePaymentSessionCommand, CreatePaymentSessionResponse>
{
    private readonly ILogger<CreatePaymentSessionHandler> _logger;

    public CreatePaymentSessionHandler(ILogger<CreatePaymentSessionHandler> logger)
    {
        _logger = logger;
    }

    #region Implementation of IRequestHandler<in CreatePaymentSessionCommand, CreatePaymentSessionResponse>

    public async Task<CreatePaymentSessionResponse> Handle(CreatePaymentSessionCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(CreatePaymentSessionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreatePaymentSessionResponse();

        return response;
    }

    #endregion
}