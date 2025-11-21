using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
    private readonly ILogger _logger;

    public UnitOfWork(AppDbContext context, ILoggerFactory logger)
    {
        _context = context;
        _logger = logger.CreateLogger("logs");

        User = new UserRepository(_context, _logger);
    }

    public IUserRepository User { get; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task<IDbContextTransaction> OpenTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }
    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}