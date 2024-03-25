using ecommerce.Repository;
using ecommerce.Repository.Interface;

namespace ecommerce.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        ICategoryRepository Categories { get; }
        IProductReviewRepository ProductReviews { get; }
        IAccountRepository Accounts { get; }
        IPaymentRepository Payments { get; }
        ICartItemRepository CartItems { get; }
        ICartRepository Carts { get; }
        IWishListRepository WishLists { get; }
        IRevenueReportRepository RevenueReports { get; }
        IHistoryRepository Histories { get; }

        int Commit();
        Task<int> CommitAsync();
    }
}
