using System.Linq.Expressions;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Infrastructure.Persistence.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _db;
    protected readonly ILogger _logger;

    protected Repository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        _db = _context.Set<T>();
    }

    public virtual IQueryable<T> GetAll()
    {
        return _db;
    }

    public virtual async Task<bool> Add(T entity)
    {
        await _db.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> AddRange(List<T> entities)
    {
        await _db.AddRangeAsync(entities);
        return true;
    }

    public virtual bool Delete(T entity)
    {
        _db.Remove(entity);
        return true;
    }

    public virtual bool DeleteRange(List<T> entities)
    {
        _db.RemoveRange(entities);
        return true;
    }

    public virtual async Task DeleteRangeAsync(Expression<Func<T, bool>> expression)
    {
        var queryable = _db.Where(expression);
        _db.RemoveRange(queryable);
        await _context.SaveChangesAsync();
    }

    public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
    {
        return _db.Where(expression);
    }

    public EntityEntry<T> Update(T entity)
    {
        return _db.Update(entity);
    }
}