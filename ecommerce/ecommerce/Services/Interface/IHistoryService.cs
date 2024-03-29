using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Services.Interface
{
    public interface IHistoryService
    {
        Task<ApiResponse<IEnumerable<HistoryDto>>> GetAllHistoriesAsync();
        Task<ApiResponse<HistoryDto>> GetHistoryByIdAsync(int id);
        Task<ApiResponse<int>> AddHistoryAsync(History history);
        Task<ApiResponse<int>> DeleteHistoryAsync(int id);
        Task<ApiResponse<int>> UpdateHistoryAsync(int id, HistoryDto history);
        // history for paymentId
        Task<ApiResponse<IEnumerable<HistoryDto>>> GetHistoriesByPaymentIdAsync(int paymentId);

    }
}
