using Infrastructure.Interfaces;
using System.Collections;

namespace Infrastructure.Implementations
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly ApplicationDbContext _context;
        private Hashtable _Repositories { get; set; }
        public UnityOfWork(ApplicationDbContext context)
        {
            _context = context;

        }


        public async Task<int> Complete()
        {
            return  await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IRepository<TEntity>? Repository<TEntity>() where TEntity : class
        {
            if (_Repositories is null) _Repositories = new Hashtable();
            var type = typeof(TEntity);
            if (!_Repositories.ContainsKey(type))
            {
                var repo = new Repository<TEntity>(_context);
                _Repositories.Add(type, repo);
            }
            return _Repositories[type] as IRepository<TEntity>;
        }
    }
}
