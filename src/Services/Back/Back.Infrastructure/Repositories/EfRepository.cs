using Back.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repositories;

internal class EfRepository<TEntity, TContext> : IRepository<TEntity> where TEntity : class
    where TContext : DbContext
{
    private readonly TContext _dbContext;

    protected EfRepository(TContext dbContext) =>
        _dbContext = dbContext;

    public IQueryable<TEntity> GetAll() => _dbContext.Set<TEntity>();

    public Task CreateAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }
}