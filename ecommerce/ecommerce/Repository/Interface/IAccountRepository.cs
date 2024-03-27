using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IAccountRepository<T>
    {
        Task<IEnumerable<User>> GetAllUser();
        Task<IEnumerable<Admin>> GetAllAdmin();
        Task<Admin> GetByIdAdmin(int id);
        Task<User> GetByIdForUser(int id);
        Task<User> GetByIdUser(int id);
        // email
        Task<User> GetByEmailForUser(string email);
        Task<Admin> GetByEmailForAdmin(string email);
        void Add(T admin);
        void Delete(T? admin);
        void Update(T admin);
    }
}
