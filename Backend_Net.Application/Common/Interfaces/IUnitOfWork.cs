using Microsoft.EntityFrameworkCore.Storage;

namespace Backend_Net.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUserRepository User { get; }
    
    Task SaveAsync();
    Task<IDbContextTransaction> OpenTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}