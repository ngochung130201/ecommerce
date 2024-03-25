using ecommerce.Repository;
using ecommerce.Repository.Interface;

namespace ecommerce.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        Task<int> CommitAsync();
    }
}
