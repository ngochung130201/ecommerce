using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto emailDto);
    }
}
