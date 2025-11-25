using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class PaymentMethodCurrencyRepository(AppDbContext context, ILogger logger)
    : Repository<PaymentMethodCurrency>(context, logger), IPaymentMethodCurrencyRepository
{
}