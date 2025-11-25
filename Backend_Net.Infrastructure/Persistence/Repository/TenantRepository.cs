using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class TenantRepository(AppDbContext context, ILogger logger)
    : Repository<Tenant>(context, logger), ITenantRepository
{
}