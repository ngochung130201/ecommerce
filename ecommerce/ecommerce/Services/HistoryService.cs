using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;
        public HistoryService(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }
        public async Task<ApiResponse<int>> AddHistoryAsync(HistoryDto history)
        {
            var historyModel = new History
            {
                Message = history.Message,
                PaymentId = history.PaymentId,
                Status = history.Status,
                StatusMessage = history.StatusMessage,
            };
            try
            {
                await _historyRepository.AddHistoryAsync(historyModel);
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "History added",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<int>> DeleteHistoryAsync(int id)
        {
            var history = await _historyRepository.GetHistoryByIdAsync(id);
            if (history == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No History found",
                    Status = false
                };
            }
            try
            {
                await _historyRepository.DeleteHistoryAsync(id);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "History deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<HistoryDto>>> GetAllHistoriesAsync()
        {
            var histories = await _historyRepository.GetAllHistoriesAsync();
            if (histories == null)
            {
                return new ApiResponse<IEnumerable<HistoryDto>>
                {
                    Data = null,
                    Message = "No History found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<HistoryDto>>
            {
                Data = new List<HistoryDto>(histories.Select(x => new HistoryDto
                {
                    Message = x.Message,
                    PaymentId = x.PaymentId,
                    Status = x.Status,
                    StatusMessage = x.StatusMessage,
                })),
                Message = "Histories found",
                Status = true
            };
        }

        public async Task<ApiResponse<HistoryDto>> GetHistoryByIdAsync(int id)
        {
            var history = await _historyRepository.GetHistoryByIdAsync(id);
            if (history == null)
            {
                return new ApiResponse<HistoryDto>
                {
                    Data = null,
                    Message = "History not found",
                    Status = false
                };
            }
            return new ApiResponse<HistoryDto>
            {
                Data = new HistoryDto
                {
                    Message = history.Message,
                    PaymentId = history.PaymentId,
                    Status = history.Status,
                    StatusMessage = history.StatusMessage,
                },
                Message = "History found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateHistoryAsync(int id, HistoryDto history)
        {
            var historyModel = new History
            {
                Message = history.Message,
                PaymentId = history.PaymentId,
                Status = history.Status,
                StatusMessage = history.StatusMessage,
            };
            try
            {
                await _historyRepository.UpdateHistoryAsync(id, historyModel);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "History updated",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }
    }
}
