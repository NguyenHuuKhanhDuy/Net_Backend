using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class PaymentTransactionRepository(AppDbContext context, ILogger logger)
    : Repository<PaymentTransaction>(context, logger), IPaymentTransactionRepository
{
}