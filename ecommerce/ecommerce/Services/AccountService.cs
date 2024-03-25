using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using Microsoft.IdentityModel.Tokens;

namespace ecommerce.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository<Admin> _accountAdminRepository;
        private readonly IAccountRepository<User> _accountUserRepository;
        public AccountService(IConfiguration configuration, IAccountRepository<Admin> accountAdminRepository, IAccountRepository<User> accountUserRepository)
        {
            _configuration = configuration;
            _accountAdminRepository = accountAdminRepository;
            _accountUserRepository = accountUserRepository;
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
                    new Claim(ClaimTypes.Role, admin.AdminRole.ToString())
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
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginDto loginDto, AdminRole? adminRole = null, bool isAdmin = false)
        {
            if (isAdmin == false)
            {
                var user = await _accountUserRepository.GetByEmailForUser(loginDto.Email);
                if (user == null) return null;

                if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
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
                var admin = await _accountAdminRepository.GetByEmailForAdmin(loginDto.Email);
                if (admin == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Admin not found",
                        Status = false
                    };
                }

                if (!VerifyPasswordHash(loginDto.Password, admin.PasswordHash, admin.PasswordSalt))
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Data = null,
                        Message = "Invalid password",
                        Status = false
                    };
                }
                var adminDto = new AdminDto
                {
                    AdminId = admin.AdminId,
                    Username = admin.Username,
                    Email = admin.Email,
                    AdminRole = admin.Role
                };
                var token = GenerateJwtToken(adminDto);
                return new ApiResponse<LoginResponse>
                {
                    Data = new LoginResponse
                    {
                        Token = token,
                        User = null,
                        Admin = adminDto,
                        IsAdmin = true,
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
            }
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

        // Generate a constructor for the AccountService class
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
