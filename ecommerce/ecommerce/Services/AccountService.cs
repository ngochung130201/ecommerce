

using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ecommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository<Admin> _accountAdminRepository;
        private readonly IAccountRepository<User> _accountUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly EcommerceContext _context;
        public AccountService(IConfiguration configuration, IAccountRepository<Admin> accountAdminRepository, EcommerceContext context,
        IAccountRepository<User> accountUserRepository, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _configuration = configuration;
            _accountAdminRepository = accountAdminRepository;
            _accountUserRepository = accountUserRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _context = context;

        }
        public string GenerateJwtToken(AdminDto admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, admin.AdminRole.ToString()),
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto)
        {
            var passwordHash = new byte[64];
            var passwordSalt = new byte[128];
            var isAdmin = false;
            var user = await _accountUserRepository.GetByEmailForUser(loginDto.Email);
            if (user != null)
            {
                if(user.AccountStatus == AccountStatus.Blocked)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "User is blocked",
                        Status = false
                    };
                }
                passwordHash = user.PasswordHash;
                passwordSalt = user.PasswordSalt;
                if (!VerifyPasswordHash(loginDto.Password, passwordHash, passwordSalt))
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Invalid password",
                        Status = false
                    };
                }

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email
                };
                var token = GenerateJwtToken(userDto);
                return new ApiResponse<LoginResponse>
                {
                    Data = new LoginResponse
                    {
                        Token = token,
                        User = userDto,
                        IsAdmin = false
                    },
                    Message = "User logged in successfully",
                    Status = true
                };
            }
            else
            {
                // Admin login
                var admin = await _accountAdminRepository.GetByEmailForAdmin(loginDto.Email);
                if (admin == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "User not found",
                        Status = false
                    };
                }
                if (admin.AccountStatus == AccountStatus.Blocked)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Admin is blocked",
                        Status = false
                    };
                }
                passwordHash = admin.PasswordHash;
                passwordSalt = admin.PasswordSalt;
                if (!VerifyPasswordHash(loginDto.Password, passwordHash, passwordSalt))
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Invalid password",
                        Status = false
                    };
                }
                return new ApiResponse<LoginResponse>
                {
                    Data = new LoginResponse
                    {
                        Token = GenerateJwtToken(new AdminDto
                        {
                            AdminId = admin.AdminId,
                            Username = admin.Username,
                            Email = admin.Email,
                            AdminRole = admin.Role
                        }),
                        Admin = new AdminDto
                        {
                            AdminId = admin.AdminId,
                            Username = admin.Username,
                            Email = admin.Email,
                            AdminRole = admin.Role,
                        },
                        AdminRole = admin.Role,
                        IsAdmin = true
                    },
                    Message = "Admin logged in successfully",
                    Status = true
                };

            }

        }

        public async Task<ApiResponse<string>> LogoutAsync()
        {
            // Perform logout logic here

            return new ApiResponse<string>
            {
                Data = "Logout successful",
                Message = "User logged out successfully",
                Status = true
            };
        }

        public async Task<ApiResponse<RegisterDto>> RegisterAsync(string username, string email, string password, AdminRole? adminRole = null, bool isAdmin = false)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            if (isAdmin == false)
            {
                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedAt = DateTime.UtcNow,
                    AccountStatus = AccountStatus.Active
                };
                var existingUser = await _accountUserRepository.GetByEmailForUser(email);
                if (existingUser != null)
                {
                    return new ApiResponse<RegisterDto>
                    {
                        Data = null,
                        Message = "User already exists",
                        Status = false
                    };
                }
                _accountUserRepository.Add(user);
            }
            else
            {
                var admin = new Admin
                {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Role = adminRole ?? AdminRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    AccountStatus = AccountStatus.Active
                };
                _accountAdminRepository.Add(admin);
                var existingAdmin = await _accountAdminRepository.GetByEmailForAdmin(email);
                if (existingAdmin != null)
                {
                    return new ApiResponse<RegisterDto>
                    {
                        Data = null,
                        Message = "Admin already exists",
                        Status = false
                    };
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<RegisterDto>
            {
                Data = new RegisterDto
                {
                    Email = email
                },
                Message = "User registered successfully",
                Status = true
            };
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email, AdminRole? adminRole = null, bool isAdmin = false)
        {
            var adminModel = new Admin();
            var userModel = new User();
            var token = string.Empty;
            var name = string.Empty;

            if (isAdmin)
            {
                adminModel = await _accountAdminRepository.GetByEmailForAdmin(email);
                token = GeneratePasswordResetToken(adminModel.AdminId, adminModel.Email, adminRole, isAdmin);
                name = adminModel.Username;
            }
            else
            {
                userModel = await _accountUserRepository.GetByEmailForUser(email);
                token = GeneratePasswordResetToken(userModel.UserId, userModel.Email);
                name = userModel.Username;
            }

            if (adminModel == null || userModel == null)
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = isAdmin ? "Admin not found" : "User not found",
                    Status = false
                };
            }

            var emailDto = new EmailDto
            {
                Body = "",
                Name = name,
                Subject = "Reset your password",
                To = email,
                Url = $"https://yourapplication.com/reset-password?token={token}&email={email}"
            };
            // test
            emailDto.Url = "https://www.facebook.com/";
            try
            {

                await _emailService.SendEmailAsync(emailDto);
                return new ApiResponse<string>
                {
                    Data = token,
                    Message = "Password reset email sent",
                    Status = true // Assuming status should be true when email is successfully sent
                };
            }
            catch (Exception)
            {
                return new ApiResponse<string>
                {
                    Data = token,
                    Message = "Failed to send email",
                    Status = false
                };
            }

        }

        public bool ValidateToken(string token, string email, bool isAdmin = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtKey"])),
                ValidateIssuer = false,
                ValidateAudience = false,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                if (isAdmin)
                {
                    var adminIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    if (adminIdClaim != null && adminIdClaim == email)
                    {
                        return true;
                    }
                }
                else
                {
                    var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    if (userIdClaim != null && userIdClaim == email)
                    {
                        return true;
                    }
                }

            }
            catch
            {
                // Token validation failed
                return false;
            }

            return false;
        }

        private string GeneratePasswordResetToken(int id, string email, AdminRole? adminRole = null, bool isAdmin = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, adminRole?.ToString() ?? (isAdmin ? nameof(adminRole) : "User"))
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, AdminRole? adminRole = null, bool isAdmin = false)
        {
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            if (isAdmin)
            {
                var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
                if (admin == null) return false;

                if (!ValidateToken(token, email, isAdmin))
                {
                    return false;
                }
                admin.PasswordHash = passwordHash;
                admin.PasswordSalt = passwordSalt;
                admin.UpdatedAt = DateTime.UtcNow;
                _accountAdminRepository.Update(admin);

            }
            else
            {
                var user = await _accountUserRepository.GetByEmailForUser(email);
                if (user == null) return false;

                if (!ValidateToken(token, email))
                {
                    return false;
                }
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UpdatedAt = DateTime.UtcNow;
                _accountUserRepository.Update(user);
            }
            await _unitOfWork.SaveChangesAsync();
            return true;

        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        public async Task<ApiResponse<string>> ResetPasswordEmailAsync(string email, string newPassword)
        {
            var user = await _accountUserRepository.GetByEmailForUser(email);
            if (user != null)
            {
                // Update user password
                CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.UpdatedAt = DateTime.UtcNow;
                _accountUserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Data = "Password reset successfully",
                    Message = "Password reset successfully",
                    Status = true
                };
            }
            else
            {
                var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
                if (admin != null)
                {
                    // Update admin password
                    CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    admin.PasswordHash = passwordHash;
                    admin.PasswordSalt = passwordSalt;
                    admin.UpdatedAt = DateTime.UtcNow;
                    _accountAdminRepository.Update(admin);
                    await _unitOfWork.SaveChangesAsync();
                    return new ApiResponse<string>
                    {
                        Data = "Password reset successfully",
                        Message = "Password reset successfully",
                        Status = true
                    };
                }
                else
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "User not found",
                        Status = false
                    };
                }
            }
        }

        public async Task<ApiResponse<string>> AddRoleAsync(string email, AdminRole role)
        {
            var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
            if (admin != null)
            {
                if (admin.Role == role)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Role already assigned",
                        Status = false
                    };
                }
                else
                {
                    admin.Role = role;
                    _accountAdminRepository.Update(admin);
                    await _unitOfWork.SaveChangesAsync();
                    return new ApiResponse<string>
                    {
                        Data = "Role assigned successfully",
                        Message = "Role assigned successfully",
                        Status = true
                    };
                }
            }
            else
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Admin not found",
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<string>> UpdateRoleAsync(string email, AdminRole role)
        {
            var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
            if (admin != null)
            {
                if (admin.Role == role)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Role already assigned",
                        Status = false
                    };
                }
                else
                {
                    admin.Role = role;
                    _accountAdminRepository.Update(admin);
                    await _unitOfWork.SaveChangesAsync();
                    return new ApiResponse<string>
                    {
                        Data = "Role updated successfully",
                        Message = "Role updated successfully",
                        Status = true
                    };
                }
            }
            else
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Admin not found",
                    Status = false
                };
            }

        }

        public async Task<ApiResponse<string>> DeleteRoleAsync(string email, AdminRole role)
        {
            var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
            if (admin != null)
            {
                if (admin.Role != role)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Role not assigned",
                        Status = false
                    };
                }
                else
                {
                    admin.Role = role;
                    _accountAdminRepository.Update(admin);
                    await _unitOfWork.SaveChangesAsync();
                    return new ApiResponse<string>
                    {
                        Data = "Role deleted successfully",
                        Message = "Role deleted successfully",
                        Status = true
                    };
                }
            }
            else
            {
                return new ApiResponse<string>
                {
                    Data = null,
                    Message = "Admin not found",
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<AdminDto>> GetRoleAsync(string email, AdminRole role)
        {
            var admin = await _accountAdminRepository.GetByEmailForAdmin(email);
            if (admin == null)
            {
                return new ApiResponse<AdminDto>
                {
                    Data = null,
                    Message = "Admin not found",
                    Status = false
                };
            }
            else
            {
                if (admin.Role != role)
                {
                    return new ApiResponse<AdminDto>
                    {
                        Data = null,
                        Message = "Role not assigned",
                        Status = false
                    };
                }
                else
                {
                    return new ApiResponse<AdminDto>
                    {
                        Data = new AdminDto
                        {
                            AdminId = admin.AdminId,
                            Username = admin.Username,
                            Email = admin.Email,
                            AdminRole = admin.Role,
                            Status = admin.AccountStatus
                        },
                        Message = "Role found",
                        Status = true
                    };
                }
            }

        }

        public async Task<ApiResponse<List<AdminDto>>> GetListRoleAsync(AdminRole role)
        {
            var admins = await _context.Admins.Where(a => a.Role == role).ToListAsync();
            if (admins == null)
            {
                return new ApiResponse<List<AdminDto>>
                {
                    Data = null,
                    Message = "Admins not found",
                    Status = false
                };
            }
            return new ApiResponse<List<AdminDto>>
            {
                Data = admins.Select(a => new AdminDto
                {
                    AdminId = a.AdminId,
                    Username = a.Username,
                    Email = a.Email,
                    AdminRole = a.Role,
                    Status = a.AccountStatus
                }).ToList(),
                Message = "Admins found",
                Status = true
            };
        }

        public async Task<ApiResponse<List<AdminDto>>> GetListRoleAsync(PagingForUser? paging = null)
        {
            // paging
            var adminsNotPaging = await _context.Admins.ToListAsync();
            if (paging == null)
            {
      
                if (adminsNotPaging == null)
                {
                    return new ApiResponse<List<AdminDto>>
                    {
                        Data = null,
                        Message = "Admins not found",
                        Status = false,
                        
                    };
                }
                return new ApiResponse<List<AdminDto>>
                {
                    Data = adminsNotPaging.Select(a => new AdminDto
                    {
                        AdminId = a.AdminId,
                        Username = a.Username,
                        Email = a.Email,
                        AdminRole = a.Role,
                        Status = a.AccountStatus
                    }).ToList(),
                    Message = "Admins found",
                    Status = true,
                    Total = adminsNotPaging.Count
                };
            }
           var admins =  adminsNotPaging.Where(x =>
                string.IsNullOrEmpty(paging.UserName) || x.Username.Contains(paging.UserName) || x.Email.Contains(paging.UserName)
            ).Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            if(paging.Status != null)
            {
                admins = admins.Where(x => x.AccountStatus == paging.Status).ToList();
            }
            if (admins == null)
            {
                return new ApiResponse<List<AdminDto>>
                {
                    Data = null,
                    Message = "Admins not found",
                    Status = false
                };
            }
            return new ApiResponse<List<AdminDto>>
            {
                Data = admins.Select(a => new AdminDto
                {
                    AdminId = a.AdminId,
                    Username = a.Username,
                    Email = a.Email,
                    AdminRole = a.Role,
                    Status = a.AccountStatus
                }).ToList(),
                Message = "Admins found",
                Status = true,
                Total = adminsNotPaging.Count
            };
        }

        public async Task<ApiResponse<List<UserDto>>> GetListUserAsync(PagingForUser paging)
         {
            //          (string.IsNullOrEmpty(filterDto.Name) || p.Name.Contains(filterDto.Name)) &&
            //         (filterDto.MinPrice == 0 || p.Price >= filterDto.MinPrice) &&
            //         (filterDto.MaxPrice == 0 || p.Price <= filterDto.MaxPrice) &&
            //         (filterDto.CategoryId == 0 || p.CategoryId == filterDto.CategoryId) &&
            //         (filterDto.Popular == 0 || p.Popular == filterDto.Popular) &&
            //         (filterDto.InventoryCount == 0 || p.InventoryCount >= filterDto.InventoryCount)
            //         ).Skip(itemsToSkip)
            //         .Take(filterDto.PageSize).OrderByDescending(u => u.CreatedAt).ToListAsync();
            var users = await _context.Users.ToListAsync();
            var usersDto = users.Where(x =>
                string.IsNullOrEmpty(paging.UserName) || x.Username.Contains(paging.UserName) || x.Email.Contains(paging.UserName)
            ).Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            //
            if (paging.Status != null)
            {
                usersDto = users.Where(x => x.AccountStatus == paging.Status).ToList();
            }
            if (users == null)
            {
                return new ApiResponse<List<UserDto>>
                {
                    Data = null,
                    Message = "Users not found",
                    Status = false
                };
            }
            return new ApiResponse<List<UserDto>>
            {
                Data = users.Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    Status = u.AccountStatus
                }).ToList(),
                Message = "Users found",
                Status = true,
                Total = users.Count
            };
        }

        public async Task<ApiResponse<string>> UpdateAccountAsync(UpdateAccountRequest updateAccountRequest)
        {
            if (updateAccountRequest.IsAdmin == false)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateAccountRequest.Email);
                if (user == null)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "User not found",
                        Status = false
                    };
                }
                user.Username = updateAccountRequest.Username ?? user.Username;
                user.Email = updateAccountRequest.Email ?? user.Email;
                user.AccountStatus = updateAccountRequest.Status ?? user.AccountStatus;
                if (!string.IsNullOrEmpty(updateAccountRequest.NewPassword))
                {
                    CreatePasswordHash(updateAccountRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
            }
            else {
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == updateAccountRequest.Email);
                if (admin == null)
                {
                    return new ApiResponse<string>
                    {
                        Data = null,
                        Message = "Admin not found",
                        Status = false
                    };
                }
                admin.Username = updateAccountRequest.Username ?? admin.Username;
                admin.Email = updateAccountRequest.Email ?? admin.Email;
                admin.AccountStatus = updateAccountRequest.Status ?? admin.AccountStatus;
                if (!string.IsNullOrEmpty(updateAccountRequest.NewPassword))
                {
                    CreatePasswordHash(updateAccountRequest.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    admin.PasswordHash = passwordHash;
                    admin.PasswordSalt = passwordSalt;
                }
                if (updateAccountRequest.Role != null)
                {
                    admin.Role = updateAccountRequest.Role.Value;
                }
                admin.UpdatedAt = DateTime.UtcNow;
                _context.Admins.Update(admin);
            }
            await  _unitOfWork.SaveChangesAsync();
            return new ApiResponse<string>
            {
                Data = "Account updated successfully",
                Message = "Account updated successfully",
                Status = true
            };

        }

        public async Task<ApiResponse<string>> DeleteAccountAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Data = "User deleted successfully",
                    Message = "User deleted successfully",
                    Status = true
                };
            }
            return new ApiResponse<string>
            {
                Data = null,
                Message = "User not found",
                Status = false,
                
            };
        }
        public async Task<ApiResponse<string>> DeleteAccountForAdminAsync(string email)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Email == email);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Data = "Admin deleted successfully",
                    Message = "Admin deleted successfully",
                    Status = true
                };
            }
            return new ApiResponse<string>
            {
                Data = null,
                Message = "Admin not found",
                Status = false
            };
        }
    }
}
