using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin> GetAdminByIdAsync(int id);
        Task AddAdminAsync(Admin admin);
        Task DeleteAdminAsync(int id);
        Task UpdateAdminAsync(int id, Admin admin);
    }
}
