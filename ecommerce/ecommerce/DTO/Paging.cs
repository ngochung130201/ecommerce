using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class Paging
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        // date
        public bool SortByDate { get; set; } = false; // sap xep theo ngay cu nhat hoac moi nhat
    }
    public class PagingForUser : Paging
    {
        public string? UserName { get; set; } = null;
        public AccountStatus? Status = null;
    }
    public class PagingForCart : Paging
    {
        // min TotalPrice and max TotalPrice
        public decimal MinTotalPrice { get; set; } = 0;
        public decimal MaxTotalPrice { get; set; } = 0;
        public string? UserName { get; set; } = null;
    }
    public class PagingForHistory : Paging
    {
        public HistoryStatus? HistoryStatus { get; set; } = null;
        public string? UserName { get; set; } = null;
    }
    // order paging 
    public class PagingForOrder : Paging
    {
        public OrderStatus? OrderStatus { get; set; } = null;
        // min TotalPrice and max TotalPrice
        public decimal MinTotalPrice { get; set; } = 0;
        public decimal MaxTotalPrice { get; set; } = 0;
        public string? UserName { get; set; } = null;
        // user name

    }
    // paging payment
    public class PagingForPayment : Paging
    {
        public PaymentStatus? PaymentStatus { get; set; } = null;
        public decimal MinTotalPrice { get; set; } = 0;
        public decimal MaxTotalPrice { get; set; } = 0;
        public PaymentMethod? PaymentMethod { get; set; } = null;
        public string? UserName { get; set; } = null;
    }
    // paging product review
    public class PagingForProductReview : Paging
    {
        public int MinRating { get; set; } = 0;
        public int MaxRating { get; set; } = 0;
        public int Rating { get; set; } = 0;
        public string? UserName { get; set; } = null;
        public string? ProductName { get; set; } = null;
    }
    //  paging wishlist
    public class PagingForWishlist : Paging
    {
        public string? UserName { get; set; } = null;
        public string? ProductName { get; set; } = null;
    }
    // blog category
    public class PagingForBlogCategory : Paging
    {
        public string? Name { get; set; } = null;
    }
}
