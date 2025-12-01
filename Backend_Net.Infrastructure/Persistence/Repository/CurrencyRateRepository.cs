using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class CurrencyRateRepository(AppDbContext context, ILogger logger)
    : Repository<CurrencyRate>(context, logger), ICurrencyRateRepository
{
}