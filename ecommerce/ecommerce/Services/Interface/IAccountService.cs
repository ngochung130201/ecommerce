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
        // Manage Account Admin role
        Task<ApiResponse<string>> AddRoleAsync(string email, AdminRole role);
        // Edit Account Admin role
        Task<ApiResponse<string>> UpdateRoleAsync(string email, AdminRole role);
        // Delete Account Admin role
        Task<ApiResponse<string>> DeleteRoleAsync(string email, AdminRole role);
        // Get Account Admin role
        Task<ApiResponse<AdminDto>> GetRoleAsync(string email, AdminRole role);

        // Get List of Account Admin for role
        Task<ApiResponse<List<AdminDto>>> GetListRoleAsync(AdminRole role);

        // Get Full List of Account Admin
        Task<ApiResponse<List<AdminDto>>> GetListRoleAsync();

    }
}
