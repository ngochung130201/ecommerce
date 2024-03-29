using System.Globalization;
using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IRepositoryBase<RevenueReport> _revenueReportRepository;
        private readonly EcommerceContext _context;
        public RevenueReportService(IRepositoryBase<RevenueReport> revenueReportRepository, EcommerceContext context)
        {
            _revenueReportRepository = revenueReportRepository;
            _context = context;
        }

        public async Task<ApiResponse<int>> AddRevenueReportAsync(RevenueReportAddDto revenueReport)
        {
            var newRevenueReport = new RevenueReport
            {
                Date = revenueReport.Date,
                TotalRevenue = revenueReport.TotalRevenue,
                CreatedAt = DateTime.UtcNow.Date,
                Day = revenueReport.Date.Day,
                Month = revenueReport.Date.Month,
                Year = revenueReport.Date.Year
            };
            try
            {
                _revenueReportRepository.Create(newRevenueReport);
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Revenue Report added",
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

        public async Task<ApiResponse<int>> DeleteRevenueReportAsync(int id)
        {
            var revenueReport = await _revenueReportRepository.FindByIdAsync(id);
            if (revenueReport == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Revenue Report found",
                    Status = false
                };
            }
            try
            {
                _revenueReportRepository.Delete(revenueReport);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Revenue Report deleted",
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

        public async Task<ApiResponse<IEnumerable<RevenueReportDto>>> GetAllRevenueReportsAsync()
        {
            var revenueReports = await _revenueReportRepository.FindAllAsync();
            if (revenueReports == null)
            {
                return new ApiResponse<IEnumerable<RevenueReportDto>>
                {
                    Data = null,
                    Message = "No Revenue Report found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<RevenueReportDto>>
            {
                Data = revenueReports.Select(x => new RevenueReportDto
                {
                    Date = x.Date,
                    TotalRevenue = x.TotalRevenue,
                    ReportId = x.ReportId,
                }),
                Message = "Revenue Report found",
                Status = true
            };
        }

        public async Task<IEnumerable<RevenueReportDto>> GetRevenueReportByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var revenueReports = await _context.RevenueReports
                .Where(r => r.Date >= startDate && r.Date <= endDate)
                .ToListAsync();
            return revenueReports.Select(x=> new RevenueReportDto
            {
                Date = x.Date,
                TotalRevenue = x.TotalRevenue,
                ReportId = x.ReportId
            });
        }


        public async Task<ApiResponse<RevenueReportDto>> GetRevenueReportByIdAsync(int id)
        {
            var revenueReport = await _revenueReportRepository.FindByIdAsync(id);
            if (revenueReport == null)
            {
                return new ApiResponse<RevenueReportDto>
                {
                    Data = null,
                    Message = "Revenue Report not found",
                    Status = false
                };
            }
            return new ApiResponse<RevenueReportDto>
            {
                Data = new RevenueReportDto
                {
                    Date = revenueReport.Date,
                    TotalRevenue = revenueReport.TotalRevenue,
                    ReportId = revenueReport.ReportId,
                },
                Message = "Revenue Report found",
                Status = true
            };
        }

        public async Task<List<object>> GetRevenueStatsTypeAsync(RevenueStatsType type)
        {
            try
            {
                IQueryable<object> query;

                switch (type)
                {
                    case RevenueStatsType.Monthly:
                        query = _context.RevenueReports
                            .GroupBy(r => new { r.Year, r.Month })
                            .Select(g => new
                            {
                                Year = g.Key.Year,
                                Month = g.Key.Month,
                                TotalRevenue = g.Sum(r => r.TotalRevenue)
                            });
                        break;

                    case RevenueStatsType.Daily:
                        query = _context.RevenueReports
                            .GroupBy(r => new { r.Year, r.Month, r.Day })
                            .Select(g => new
                            {
                                Year = g.Key.Year,
                                Month = g.Key.Month,
                                Day = g.Key.Day,
                                TotalRevenue = g.Sum(r => r.TotalRevenue)
                            });
                        break;

                    case RevenueStatsType.Weekly:
                        query = _context.RevenueReports
                            .GroupBy(r => new { Year = r.Date.Year, Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(r.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) })
                            .Select(g => new
                            {
                                Year = g.Key.Year,
                                Week = g.Key.Week,
                                TotalRevenue = g.Sum(r => r.TotalRevenue)
                            });
                        break;

                    case RevenueStatsType.Yearly:
                        query = _context.RevenueReports
                            .GroupBy(r => r.Year)
                            .Select(g => new
                            {
                                Year = g.Key,
                                TotalRevenue = g.Sum(r => r.TotalRevenue)
                            });
                        break;

                    default:
                        return null;
                }

                var result = await query.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Task<ApiResponse<int>> UpdateRevenueReportAsync(int id, RevenueReportDto revenueReport)
        {
            var revenueReportToUpdate = new RevenueReport
            {
                Date = revenueReport.Date,
                TotalRevenue = revenueReport.TotalRevenue
            };
            try
            {
                _revenueReportRepository.Update(revenueReportToUpdate);
                return Task.FromResult(new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Revenue Report updated",
                    Status = true
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                });
            }
        }
    }
}
