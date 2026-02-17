using Credio.Core.Application.Interfaces.Repositories;
using Credio.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Credio.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        private readonly IDbContextFactory<ApplicationContext> _dbContextFactory;

        public GenericRepository(IDbContextFactory<ApplicationContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public virtual async Task<Entity> AddAsync(Entity entity)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Set<Entity>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<List<Entity>> AddManyAsync(List<Entity> entities)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            await dbContext.Set<Entity>().AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();
            return entities;
        }

        public virtual async Task UpdateAsync(Entity entity)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Set<Entity>().Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateManyAsync(List<Entity> entities)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Set<Entity>().UpdateRange(entities);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Set<Entity>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteManyAsync(List<Entity> entities)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            dbContext.Set<Entity>().RemoveRange(entities);
            await dbContext.SaveChangesAsync();
        }

        public virtual async Task<List<Entity>> GetAllAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Set<Entity>().ToListAsync();//Deferred execution
        }

        public virtual async Task<List<Entity>> GetAllWithIncludeAsync(List<Expression<Func<Entity, object>>> properties)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<Entity> GetByIdAsync(string id)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            return await dbContext.Set<Entity>().FindAsync(id);
        }

        public virtual async Task<Entity> GetByIdWithIncludeAsync(Expression<Func<Entity, bool>> predicate, List<Expression<Func<Entity, object>>> properties)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<Entity> GetByPropertyAsync(Expression<Func<Entity, bool>> predicate)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<Entity> GetByPropertyWithIncludeAsync(Expression<Func<Entity, bool>> predicate,
            List<Expression<Func<Entity, object>>> properties)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<Entity>> GetAllByPropertyAsync(Expression<Func<Entity, bool>> predicate)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            return await query.Where(predicate).ToListAsync();
        }

        public async Task<List<Entity>> GetAllByPropertyWithIncludeAsync(Expression<Func<Entity, bool>> predicate,
            List<Expression<Func<Entity, object>>> properties)
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var query = dbContext.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.Where(predicate).ToListAsync();
        }
    }
}
