using CareSync.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CareSync.ApplicationLayer.Repository;

public sealed class Repository<T>(CareSyncDbContext context, ILogger<Repository<T>> logger) : IRepository<T> where T : class
{
    public async Task<List<T>> GetAllAsync()
    {
        logger.LogInformation("Executing: GetAllAsync");
        return await context.Set<T>().ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        logger.LogInformation("Executing: GetAllAsync");
        return await context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<int> GetCountAsync(Expression<Func<T, bool>> predicate)
    {
        logger.LogInformation("Executing: GetCountAsync");
        return await context.Set<T>().CountAsync(predicate);
    }

    public async Task<List<T>> GetAllActiveAsync()
    {
        logger.LogInformation("Executing: GetAllActiveAsync");

        var property = typeof(T).GetProperty("IsActive");
        if (property != null)
            return await context.Set<T>()
            .Where(e => EF.Property<bool>(e, "IsActive"))
            .ToListAsync();

        logger.LogInformation($"{typeof(T).Name} does not have IsActive property");
        return [];
    }

    public async Task<List<T>> GetAllInactiveAsync()
    {
        logger.LogInformation("Executing: GetAllInactiveAsync");

        var property = typeof(T).GetProperty("IsActive");
        if (property != null)
            return await context.Set<T>()
            .Where(e => EF.Property<bool>(e, "IsActive") == false)
            .ToListAsync();

        logger.LogInformation($"{typeof(T).Name} does not have IsActive property");
        return [];
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task UpdateAsync(T entity)
    {
        logger.LogInformation("Executing: UpdateAsync");
        context.Set<T>().Update(entity);
    }

    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        logger.LogInformation("Executing: DeleteAsync");
        context.Set<T>().Remove(entity);
    }
}
