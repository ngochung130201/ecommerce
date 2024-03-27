

using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
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
        public AccountService(IConfiguration configuration, IAccountRepository<Admin> accountAdminRepository,
            IAccountRepository<User> accountUserRepository, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _configuration = configuration;
            _accountAdminRepository = accountAdminRepository;
            _accountUserRepository = accountUserRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;

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
                            AdminRole= admin.Role,
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
                    PasswordSalt = passwordSalt
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
                    Role = adminRole ?? AdminRole.Admin
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
    }
}
