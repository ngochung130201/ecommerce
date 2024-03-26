using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            return Ok();
        }
        // GET: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
        {
            var response = await _accountService.LoginAsync(login);
            return Ok(response);
        }
        // POST: api/account/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto register)
        {
            if (register.Password != register.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match");
            }
            var response = await _accountService.RegisterAsync(username: register.Name, email: register.Email, password: register.Password);
            return Ok(response);
        }
        // POST: api/account/register-admin only admin can access
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdminAsync([FromBody] RegisterAdminDto register)
        {
            if (register.Password != register.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match");
            }
            var response = await _accountService.RegisterAsync(username: register.Name, email: register.Email, password: register.Password, adminRole: register.Role, isAdmin: true);
            return Ok(response);
        }
        // POST: api/account/forgot-password
        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            var response = await _accountService.ForgotPasswordAsync(email);
            return Ok(response);
        }
        // Reset password with token
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPassword)
        {
            var response = await _accountService.ResetPasswordAsync(resetPassword.Email, resetPassword.Token, resetPassword.NewPassword);
            return Ok(response);
        }

    }
}
