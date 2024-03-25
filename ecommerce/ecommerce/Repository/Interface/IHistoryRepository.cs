using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IHistoryRepository
    {
        Task<IEnumerable<History>> GetAllHistoriesAsync();
        Task<History> GetHistoryByIdAsync(int id);
        Task<IEnumerable<History>> GetHistoriesByUserIdAsync(int userId);
        Task AddHistoryAsync(History history);
        Task AddHistoriesAsync(List<History> histories);
        Task UpdateHistoryAsync(int id, History history);
        Task DeleteHistoryAsync(int id);
    }
}
