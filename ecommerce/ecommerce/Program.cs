using ecommerce.Context;
using ecommerce.Models;
using ecommerce.Repository;
using ecommerce.Repository.Base;
using ecommerce.Repository.Interface;
using ecommerce.Services;
using ecommerce.Services.Interface;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<EcommerceContext>(
    );


// Register repositories
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

builder.Services.AddScoped<IRepositoryBase<OrderItem>, RepositoryBase<OrderItem>>();
builder.Services.AddScoped<IRepositoryBase<Product>, RepositoryBase<Product>>();
builder.Services.AddScoped<IRepositoryBase<Order>, RepositoryBase<Order>>();
builder.Services.AddScoped<IRepositoryBase<Category>, RepositoryBase<Category>>();
builder.Services.AddScoped<IRepositoryBase<ProductReview>, RepositoryBase<ProductReview>>();
builder.Services.AddScoped<IRepositoryBase<Admin>, RepositoryBase<Admin>>();
builder.Services.AddScoped<IRepositoryBase<Payment>, RepositoryBase<Payment>>();
builder.Services.AddScoped<IRepositoryBase<CartItem>, RepositoryBase<CartItem>>();
builder.Services.AddScoped<IRepositoryBase<Cart>, RepositoryBase<Cart>>();
builder.Services.AddScoped<IRepositoryBase<Wishlist>, RepositoryBase<Wishlist>>();
builder.Services.AddScoped<IRepositoryBase<RevenueReport>, RepositoryBase<RevenueReport>>();
builder.Services.AddScoped<IRepositoryBase<History>, RepositoryBase<History>>();

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishListRepository, WishListRepository>();
builder.Services.AddScoped<IRevenueReportRepository, RevenueReportRepository>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();


// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IRevenueReportService, RevenueReportService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
