using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using PoliceMaps.Models.Pagination;
using System.Linq.Dynamic.Core;

namespace PoliceMaps.Repositories.Generic
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task AddOrUpdateAsync(TEntity entity);

        Task AddOrUpdateAsync(IEnumerable<TEntity> entities);

        Task AddAsync(TEntity entity);

        Task AddAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteAsync(Expression<Func<TEntity, bool>> filter);

        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool asNoTracking = false,
            params string[] includeProperties);

        Task<TEntity> GetAsync(object id, bool asNoTracking = false);

        public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties);

        Task DeleteAsync(IEnumerable<TEntity> entities);

        Task<Models.Pagination.PagedResult<TEntity>> GetPagedAsync(PaginationPropertiesModel model,
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties);
    }

    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<TEntity> _set;

        public GenericRepository(TContext context)
        {
            _context = context;
            _set = _context.Set<TEntity>();
        }

        public virtual async Task AddOrUpdateAsync(TEntity entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateAsync(IEnumerable<TEntity> entities)
        {
            _set.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            _set.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(IEnumerable<TEntity> entities)
        {
            _set.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await this.GetAsync(id);
            await this.DeleteAsync(entity);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            _set.RemoveRange(_set.Where(filter));
            await _context.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null,
            bool asNoTracking = false,
            params string[] includeProperties)
        {
            var query = this.GetQuery();

            if (includeProperties is { Length: > 0 })
            {
                query = includeProperties.Aggregate(query, (current, prop) => current.Include(prop));
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity> GetAsync(object id, bool asNoTracking = false)
        {
            var entity = await _set.FindAsync(id);

            if (entity != null && asNoTracking)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            _set.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties)
        {
            var query = this.GetQuery();

            if (includeProperties is { Length: > 0 })
            {
                query = includeProperties.Aggregate(query, (current, prop) => current.Include(prop));
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<Models.Pagination.PagedResult<TEntity>> GetPagedAsync(PaginationPropertiesModel model,
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties)
        {
            if (model == null)
            {
                model = PaginationPropertiesModel.Default;
            }

            var query = this.GetQuery();

            if (includeProperties is { Length: > 0 })
            {
                query = includeProperties.Aggregate(query, (current, prop) => current.Include(prop));
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            else if (!String.IsNullOrEmpty(model.Filter))
            {
                if (!model.Filter.StartsWith('(') || !model.Filter.EndsWith(')'))
                {
                    model.Filter = '(' + model.Filter + ')';
                }

                try
                {
                    var expression = (Expression<Func<TEntity, bool>>)DynamicExpressionParser.ParseLambda(
                        new[] { Expression.Parameter(typeof(TEntity), "x") }, null, model.Filter
                    );
                    query = query.Where(expression);
                }
                catch (Exception)
                {
                    return new Models.Pagination.PagedResult<TEntity>
                    {
                        Entities = new List<TEntity>(),
                        Skip = 0,
                        Take = 0,
                        TotalEntities = -1
                    };
                }
            }

            if (!string.IsNullOrEmpty(model.OrderByColumnId))
            {
                query = query.OrderBy(model.OrderByColumnId);

                if (model.OrderByDecreasing)
                    query.Reverse();
            }

            if (model.Take < 0)
            {
                model.Take = query.Count();
            }

            return new Models.Pagination.PagedResult<TEntity>
            {
                Entities = await query.Skip(model.Skip).Take(model.Take).ToListAsync(),
                Skip = model.Skip,
                Take = model.Take,
                TotalEntities = query.Count()
            };
        }

        protected async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected IQueryable<TEntity> GetQuery()
        {
            return _set.AsQueryable().AsNoTracking();
        }
    }
}
