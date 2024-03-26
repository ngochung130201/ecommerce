using ecommerce.Context;

namespace ecommerce.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceContext _context;

        public UnitOfWork(EcommerceContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            var result = _context.SaveChanges();
            return result;
        }

        public async Task<int> SaveChangesAsync()
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
