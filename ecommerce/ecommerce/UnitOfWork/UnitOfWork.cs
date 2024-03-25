using ecommerce.Context;
using ecommerce.Repository;
using ecommerce.Repository.Interface;

namespace ecommerce.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceContext _context;

        public UnitOfWork(EcommerceContext context)
        {
            _context = context;
        }

        public int Commit()
        {
            var result = _context.SaveChanges();
            return result;
        }

        public async Task<int> CommitAsync()
        {
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public void Dispose()
        {
           _context.Dispose();
        }
    }
}
