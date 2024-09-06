using System.Linq.Expressions;

namespace AEBackendProject.Repositories
{
    public interface IRepository
    {
        public interface IRepository<T, TKey> where T : class
        {
            Task<T> GetByIdAsync(TKey id);
            Task<IEnumerable<T>> GetAllAsync();
            Task<IEnumerable<T>> GetAsync(
                Expression<Func<T, bool>> predicate = null,
                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                Func<IQueryable<T>, IQueryable<T>> selector = null,
                Expression<Func<T, object>>[] includes = null,
                int? skip = null,
                int? take = null
            );
            Task AddAsync(T entity);
            Task UpdateAsync(T entity);
            Task DeleteAsync(T entity);
        }
    }
}
