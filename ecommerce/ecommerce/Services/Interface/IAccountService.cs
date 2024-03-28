using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;

namespace ecommerce.Services.Interface
{
    public interface IAccountService
    {
        Task<ApiResponse<RegisterDto>> RegisterAsync(string username, string email, string password, AdminRole? adminRole = null, bool isAdmin = false);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<string>> LogoutAsync();
        string GenerateJwtToken(AdminDto user);
        string GenerateJwtToken(UserDto user);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword, AdminRole? adminRole = null, bool isAdmin = false);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email, AdminRole? adminRole = null, bool isAdmin = false);
        Task<ApiResponse<string>> ResetPasswordEmailAsync(string email, string newPassword);
    }
}
