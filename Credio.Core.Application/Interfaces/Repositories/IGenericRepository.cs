using Credio.Core.Application.Dtos.Common;
using System.Linq.Expressions;

namespace Credio.Core.Application.Interfaces.Repositories
{
	public interface IGenericRepository<Entity> where Entity : class
	{
		Task<Entity> AddAsync(Entity entity);
		Task<List<Entity>> AddManyAsync(List<Entity> entities);
		Task UpdateAsync(Entity entity);
		Task UpdateManyAsync(List<Entity> entities);
        Task DeleteAsync(Entity entity);
		Task DeleteManyAsync(List<Entity> entities);
		Task<List<Entity>> GetAllAsync();
		Task<Entity> GetByIdAsync(string id);
		Task<List<Entity>> GetAllWithIncludeAsync(List<Expression<Func<Entity, object>>> properties);
		Task<Entity> GetByIdWithIncludeAsync(Expression<Func<Entity, bool>> predicate, List<Expression<Func<Entity, object>>> properties);
		Task<Entity> GetByPropertyAsync(Expression<Func<Entity, bool>> predicate);
		Task<Entity> GetByPropertyWithIncludeAsync(Expression<Func<Entity, bool>> predicate,
			List<Expression<Func<Entity, object>>> properties);
		Task<List<Entity>> GetAllByPropertyAsync(Expression<Func<Entity, bool>> predicate);
        Task<List<Entity>> GetAllByPropertyWithIncludeAsync(Expression<Func<Entity, bool>> predicate,
            List<Expression<Func<Entity, object>>> properties);
        
        Task<PagedResult<Entity>> GetPagedAsync(int pageNumber, int pageSize, List<Expression<Func<Entity, object>>>? properties,
			Expression<Func<Entity, bool>>? predicate = null);
        }
}
