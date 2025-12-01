using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Constants;
using Backend_Net.Domain.Entities;
using Backend_Net.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class PaymentMethodRepository(AppDbContext context, ILogger logger)
    : Repository<PaymentMethod>(context, logger), IPaymentMethodRepository
{
}