using System.Linq.Expressions;

namespace CareSync.ApplicationLayer.Repository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<int> GetCountAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAllActiveAsync();
    Task<List<T>> GetAllInactiveAsync();
    Task<T?> GetByIdAsync(object id);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
