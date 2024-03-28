using ecommerce.Context;
using ecommerce.Models;
using ecommerce.Repository;
using ecommerce.Repository.Base;
using ecommerce.Repository.Interface;
using ecommerce.Services;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<EcommerceContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Ecommerce_SQLServer")));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtKey"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
// Register repositories
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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
builder.Services.AddScoped<IAccountRepository<User>, AccountRepository>();
builder.Services.AddScoped<IAccountRepository<Admin>, AccountRepository>();
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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUploadFilesService, UploadFilesService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.WithOrigins("*")
           .AllowAnyMethod()
           .AllowAnyHeader();

// U Can Filter Here
}));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("MyPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
