using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly IRepositoryBase<History> _repositoryBase;
        private readonly EcommerceContext _context;
        public HistoryRepository(IRepositoryBase<History> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }
        public async Task AddHistoriesAsync(List<History> histories)
        {
            try
            {
                _repositoryBase.CreateRange(histories);

            }
            catch (Exception e)
            {
                throw new CustomException("Histories not added", 500);
            }
        }

        public async Task AddHistoryAsync(History history)
        {
            try
            {
                _repositoryBase.Create(history);

            }
            catch (Exception e)
            {
                throw new CustomException("History not added", 500);
            }
        }

        public async Task DeleteHistoryAsync(int id)
        {
            var history = await _repositoryBase.FindByIdAsync(id);
            if (history == null)
            {
                throw new CustomException("No History found", 404);
            }
            try
            {
                _repositoryBase.Delete(history);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<History>> GetAllHistoriesAsync()
        {
            var histories = await _context.Histories.Include(u => u.Payment).ThenInclude(u => u.Order).ThenInclude(u => u.User).ToListAsync();
            if (histories == null)
            {
                throw new CustomException("No History found", 404);
            }
            return histories;
        }

        public async Task<IEnumerable<History>> GetHistoriesByUserIdAsync(int userId)
        {
            var histories = await _context.Histories.Include(u => u.Payment).ThenInclude(u => u.Order).ThenInclude(u => u.User).Where(u => u.Payment.Order.UserId == userId).ToListAsync();
            if (histories == null)
            {
                throw new CustomException("No History found", 404);
            }
            return histories;
        }

        public async Task<History> GetHistoryByIdAsync(int id)
        {
            var history = await _context.Histories.Include(u => u.Payment).ThenInclude(u => u.Order).ThenInclude(u => u.User).FirstOrDefaultAsync(u => u.HistoryId == id);
            return history;
        }

        public async Task UpdateHistoryAsync(int id, History history)
        {
            var historyToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (historyToUpdate == null)
            {
                throw new CustomException("History not found", 404);
            }
            try
            {
                _repositoryBase.Update(historyToUpdate);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
