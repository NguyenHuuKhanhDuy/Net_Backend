using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend_Net.Application.Common.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetAll();
    Task<bool> Add(T entity);
    Task<bool> AddRange(List<T> entity);
    bool Delete(T entity);
    bool DeleteRange(List<T> entities);
    Task DeleteRangeAsync(Expression<Func<T, bool>> expression);
    IQueryable<T> Where(Expression<Func<T, bool>> expression);
    EntityEntry<T> Update(T entity);
}