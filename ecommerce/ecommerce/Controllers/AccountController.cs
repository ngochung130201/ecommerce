using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [HttpPost]
        public async Task<IActionResult> GetAllAccountsAsync([FromQuery] bool isAdmin = false, [FromBody] PagingForUser? paging = null)
        {
            if (isAdmin)
            {
                var response = await _accountService.GetListRoleAsync(paging);
                if (response.Status)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
            else
            {
                var response = await _accountService.GetListUserAsync(paging);
                if (response.Status)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }

        }
        // GET: api/account/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
        {
            var response = await _accountService.LoginAsync(login);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
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
            var responseToken = await _accountService.GetRoleAsync(register.EmailForSupperAdmin, AdminRole.SuperAdmin);
            if (!responseToken.Status)
            {
               return Unauthorized("You are not authorized to access this endpoint");
            }
            if (register.Password != register.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match");
            }
            var response = await _accountService.RegisterAsync(username: register.Name, email: register.Email, password: register.Password, adminRole: register.Role, isAdmin: true);
            return Ok(response);
        }
        // Api test register-admin
        [HttpPost("register-admin-test")]
        public async Task<IActionResult> RegisterAdminTestAsync([FromBody] RegisterAdminDto register)
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
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Reset password with token
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest resetPassword)
        {
            var response = await _accountService.ResetPasswordAsync(resetPassword.Email, resetPassword.Token, resetPassword.NewPassword);
            if (response)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // reset password with email
        [HttpPost("reset-password-email")]
        public async Task<IActionResult> ResetPasswordEmailAsync([FromBody] ResetPasswordEmailRequest resetPassword)
        {
            var response = await _accountService.ResetPasswordEmailAsync(resetPassword.Email, resetPassword.NewPassword);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Get Account Role
        [HttpGet("role")]
        public async Task<IActionResult> GetRoleAsync(string email, AdminRole role)
        {
            var response = await _accountService.GetRoleAsync(email, role);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Get List of Account Role
        [HttpGet("role-list")]
        public async Task<IActionResult> GetListRoleAsync(AdminRole role)
        {
            var response = await _accountService.GetListRoleAsync(role);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // // Get Full List of Account Role
        // [HttpGet("role-list-all")]
        // public async Task<IActionResult> GetListRoleAsync(Paging paging)
        // {
        //     var response = await _accountService.GetListRoleAsync(paging);
        //     if (response.Status)
        //     {
        //         return Ok(response);
        //     }
        //     return BadRequest(response);
        // }
        // Add Account Role
        [HttpPost("role-add")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleRequest addRole)
        {
            var response = await _accountService.AddRoleAsync(addRole.Email, addRole.Role);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Update Account Role
        [HttpPut("role-update")]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] AddRoleRequest addRole)
        {
            var response = await _accountService.UpdateRoleAsync(addRole.Email, addRole.Role);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Delete Account Role
        [HttpDelete("role-delete")]
        public async Task<IActionResult> DeleteRoleAsync([FromBody] AddRoleRequest addRole)
        {
            var response = await _accountService.DeleteRoleAsync(addRole.Email, addRole.Role);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


    }
}
