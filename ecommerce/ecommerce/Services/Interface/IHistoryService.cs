using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IHistoryService
    {
        Task<ApiResponse<IEnumerable<HistoryDto>>> GetAllHistoriesAsync();
        Task<ApiResponse<HistoryDto>> GetHistoryByIdAsync(int id);
        Task<ApiResponse<int>> AddHistoryAsync(HistoryDto history);
        Task<ApiResponse<int>> DeleteHistoryAsync(int id);
        Task<ApiResponse<int>> UpdateHistoryAsync(int id, HistoryDto history);
    }
}
