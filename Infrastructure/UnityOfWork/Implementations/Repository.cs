
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entity;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }
        public Task AddAsync(T entity) => _entity.AddAsync(entity).AsTask();
        public Task<T> GetByIdAsync(Guid id) => _entity.FindAsync(id).AsTask();
        public async Task<IEnumerable<T>> GetAllDataAsync() =>  await _entity.ToListAsync();
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entity.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task<T> FindWithIncludeAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }


        public Task<T> FindAsync(Expression<Func<T, bool>> predicate) => _entity.FirstOrDefaultAsync(predicate);
        
    }

}
