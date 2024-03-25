using ecommerce.Context;
using ecommerce.Repository;
using ecommerce.Repository.Interface;

namespace ecommerce.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceContext _context;

        public UnitOfWork(EcommerceContext context)
        {
            _context = context;
        }


        public IOrderRepository Orders => throw new NotImplementedException();

        public IOrderItemRepository OrderItems => throw new NotImplementedException();

        public ICategoryRepository Categories => throw new NotImplementedException();

        public IProductReviewRepository ProductReviews => throw new NotImplementedException();

        public IAccountRepository Accounts => throw new NotImplementedException();

        public IPaymentRepository Payments => throw new NotImplementedException();

        public ICartItemRepository CartItems => throw new NotImplementedException();

        public ICartRepository Carts => throw new NotImplementedException();

        public IWishListRepository WishLists => throw new NotImplementedException();

        public IRevenueReportRepository RevenueReports => throw new NotImplementedException();

        public IHistoryRepository Histories => throw new NotImplementedException();

        public IProductRepository Products => throw new NotImplementedException();

        public int Commit()
        {
            throw new NotImplementedException();
        }

        public Task<int> CommitAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
