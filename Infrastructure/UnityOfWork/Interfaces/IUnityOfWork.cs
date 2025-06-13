namespace Infrastructure.Interfaces
{
    public interface IUnityOfWork : IDisposable
    {
        public IRepository<TEntity>? Repository<TEntity>() where TEntity : class;
        public Task <int> Complete();
    }
}
