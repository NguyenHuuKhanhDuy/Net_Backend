using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class UserRepository(AppDbContext context, ILogger logger) : Repository<User>(context, logger), IUserRepository
{
    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _db
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }
}