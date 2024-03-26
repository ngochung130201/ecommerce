using ecommerce.Repository;
using ecommerce.Repository.Interface;

namespace ecommerce.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
