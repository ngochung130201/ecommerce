using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class AccountRepository : IAccountRepository<Admin>, IAccountRepository<User>
    {
        private readonly IRepositoryBase<Admin> _repositoryBaseAdmin;
        private readonly IRepositoryBase<User> _repositoryBaseUser;
        private readonly EcommerceContext _context;
        public AccountRepository(IRepositoryBase<Admin> repositoryBaseAdmin, IRepositoryBase<User> repositoryBaseUser,EcommerceContext context)
        {
            _repositoryBaseAdmin = repositoryBaseAdmin;
            _repositoryBaseUser = repositoryBaseUser;
            _context = context;
        }

        public void Add(Admin admin)
        {
            try
            {
                _repositoryBaseAdmin.Create(admin);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }

            // Additional code logic here...

        }

        public void Add(User user)
        {
            try
            {
                _repositoryBaseUser.Create(user);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public void Delete(Admin? admin)
        {
            try
            {
                if (admin == null)
                {
                    throw new CustomException("Admin not found", 404);
                }
                _repositoryBaseAdmin.Delete(admin);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public void Delete(User? user)
        {
            try
            {
                if (user == null)
                {
                    throw new CustomException("User not found", 404);
                }
                _repositoryBaseUser.Delete(user);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<Admin>> GetAllAdmin()
        {
            try
            {
                return await _repositoryBaseAdmin.FindAllAsync();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }


        public void Update(Admin admin)
        {
            try
            {
                _repositoryBaseAdmin.Update(admin);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public void Update(User user)
        {
            try
            {
                _repositoryBaseUser.Update(user);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            try
            {
                return await _repositoryBaseUser.FindAllAsync();
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<Admin> GetByIdAdmin(int id)
        {
            var admin = await _repositoryBaseAdmin.FindByIdAsync(id);
            if (admin == null)
            {
                throw new CustomException("Admin not found", 404);
            }
            return admin;
        }

        public async Task<User> GetByIdForUser(int id)
        {
            var user = await _context.Users.Include(u=>u.Carts).ThenInclude(u=>u.CartItems).FirstOrDefaultAsync(x=>x.UserId == id);
            if (user == null)
            {
                throw new CustomException("User not found", 404);
            }
            return user;
        }
        public async Task<User> GetByIdUser(int id)
        {
            var user = _context.Users.Include(u => u.Carts).FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                throw new CustomException("User not found", 404);
            }
            return user;
        }

        public async Task<User> GetByEmailForUser(string email)
        {
            var user = await _repositoryBaseUser.FindByConditionAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            return user.FirstOrDefault();
        }

        public async Task<Admin> GetByEmailForAdmin(string email)
        {
            var admin = await _repositoryBaseAdmin.FindByConditionAsync(a => a.Email == email);
            if (admin == null)
            {
                return null;
            }
            return admin.FirstOrDefault();
        }
    }
}
