using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly EcommerceContext _context;
        public HistoryService(IHistoryRepository historyRepository, EcommerceContext context)
        {
            _historyRepository = historyRepository;
            _context = context;
        }
        public async Task<ApiResponse<int>> AddHistoryAsync(History history)
        {
            try
            {
                await _historyRepository.AddHistoryAsync(history);
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

        public async Task<ApiResponse<IEnumerable<HistoryDto>>> GetAllHistoriesAsync(PagingForHistory? pagingForHistory = null)
        {
            var histories = _context.Histories.Include(x => x.Payment).ThenInclude(k => k.Order).ThenInclude(k => k.User).AsQueryable();
            var total = await _context.Histories.CountAsync();
            if (pagingForHistory != null)
            {
                if (!string.IsNullOrEmpty(pagingForHistory.UserName))
                {
                    histories = histories.Where(c => c.Payment.Order.User.Username.Contains(pagingForHistory.UserName) || c.Payment.Order.User.Email.Contains(pagingForHistory.UserName));
                }
                if (pagingForHistory.SortByDate)
                {
                    histories = histories.OrderBy(c => c.CreateAt);
                }
                else
                {
                    histories = histories.OrderByDescending(c => c.CreateAt);
                }
                if (pagingForHistory.HistoryStatus != null)
                {
                    histories = histories.Where(c => c.Status == pagingForHistory.HistoryStatus);
                }
                histories = histories.Skip((pagingForHistory.Page - 1) * pagingForHistory.PageSize).Take(pagingForHistory.PageSize);
                var totalPage = (int)Math.Ceiling(total / (double)pagingForHistory.PageSize);
                if(totalPage <1){
                    totalPage = 1;
                }
                return new ApiResponse<IEnumerable<HistoryDto>>
                {
                    Data = new List<HistoryDto>(histories.Select(x => new HistoryDto
                    {
                        Message = x.Message,
                        PaymentId = x.PaymentId,
                        HistoryId = x.HistoryId,
                        Status = x.Status,
                        UserId = x.Payment.Order.UserId,
                        StatusMessage = x.StatusMessage,
                        User = new UserDto
                        {
                            Email = x.Payment.Order.User.Email,
                            Username = x.Payment.Order.User.Username,
                            UserId = x.Payment.Order.UserId

                        },
                        Payment = new PaymentDto
                        {
                            OrderId = x.Payment.OrderId,
                            PaymentId = x.PaymentId,
                            PaymentMethod = x.Payment.PaymentMethod,
                            PaymentStatus = x.Payment.PaymentStatus,
                            UserName = x.Payment.Order.User.Username,
                            CreatedAt = x.Payment.Order.CreatedAt,
                            Amount = x.Payment.Amount,
                            UpdatedAt = x.Payment.Order.UpdatedAt
                        }
                    })),
                    Message = "Histories found",
                    Status = true,
                    Total =  histories.Count(),
                    TotalPage = totalPage,
                    Page = pagingForHistory.Page,
                    PageSize = pagingForHistory.PageSize
                };
            }
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
                    HistoryId = x.HistoryId,
                    UserId = x.Payment.Order.UserId,
                    StatusMessage = x.StatusMessage,
                    User = new UserDto
                    {
                        Email = x.Payment.Order.User.Email,
                        Username = x.Payment.Order.User.Username,
                        UserId = x.Payment.Order.UserId

                    },
                    Payment = new PaymentDto
                    {
                        OrderId = x.Payment.OrderId,
                        PaymentId = x.PaymentId,
                        PaymentMethod = x.Payment.PaymentMethod,
                        PaymentStatus = x.Payment.PaymentStatus,
                        UserName = x.Payment.Order.User.Username,
                        CreatedAt = x.Payment.Order.CreatedAt,
                        Amount = x.Payment.Amount,
                        UpdatedAt = x.Payment.Order.UpdatedAt
                    }
                })),
                Message = "Histories found",
                Status = true,
                Total = histories.Count()
            };
        }

        public async Task<ApiResponse<IEnumerable<HistoryDto>>> GetHistoriesByPaymentIdAsync(int paymentId)
        {
            var histories = await _context.Histories.Where(x => x.PaymentId == paymentId).Include(u => u.Payment).ThenInclude(u => u.Order).ThenInclude(k => k.User).ToListAsync();
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
                    HistoryId = x.HistoryId,
                    UserId = x.Payment.Order.UserId,
                    StatusMessage = x.StatusMessage,
                    User = new UserDto
                    {
                        Email = x.Payment.Order.User.Email,
                        Username = x.Payment.Order.User.Username,
                        UserId = x.Payment.Order.UserId

                    },
                    Payment = new PaymentDto
                    {
                        OrderId = x.Payment.OrderId,
                        PaymentId = x.PaymentId,
                        PaymentMethod = x.Payment.PaymentMethod,
                        PaymentStatus = x.Payment.PaymentStatus,
                        UserName = x.Payment.Order.User.Username,
                        CreatedAt = x.Payment.Order.CreatedAt,
                        Amount = x.Payment.Amount,
                        UpdatedAt = x.Payment.Order.UpdatedAt
                    }
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
                    HistoryId = history.HistoryId,
                    UserId = history.Payment.Order.UserId,
                    StatusMessage = history.StatusMessage,
                    User = new UserDto
                    {
                        Email = history.Payment.Order.User.Email,
                        Username = history.Payment.Order.User.Username,
                        UserId = history.Payment.Order.UserId

                    },
                    Payment = new PaymentDto
                    {
                        OrderId = history.Payment.OrderId,
                        PaymentId = history.PaymentId,
                        PaymentMethod = history.Payment.PaymentMethod,
                        PaymentStatus = history.Payment.PaymentStatus,
                        UserName = history.Payment.Order.User.Username,
                        CreatedAt = history.Payment.Order.CreatedAt,
                        Amount = history.Payment.Amount,
                        UpdatedAt = history.Payment.Order.UpdatedAt

                    },
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
