using ecommerce.Context;
using ecommerce.Models;
using ecommerce.Repository.Base;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class AccountRepository : RepositoryBase<Admin>, IAccountRepository
    {
        private readonly IRepositoryBase<Admin> _repositoryBase;
        public AccountRepository(EcommerceContext context) : base(context)
        {
            _repositoryBase = new RepositoryBase<Admin>(context);
        }
        public Task AddAdminAsync(Admin admin)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAdminAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Admin> GetAdminByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAdminAsync(int id, Admin admin)
        {
            throw new NotImplementedException();
        }
    }
}
