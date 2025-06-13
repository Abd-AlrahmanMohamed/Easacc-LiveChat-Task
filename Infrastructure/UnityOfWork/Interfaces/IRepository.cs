using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllDataAsync();
        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T> FindWithIncludeAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null);

        Task<T> FindAsync(Expression<Func<T, bool>> predicate);

    }
}
