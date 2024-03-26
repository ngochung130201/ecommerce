using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Services.Interface;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;



namespace ecommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var host = _configuration["EmailSettings:Host"];
                var port = int.Parse(_configuration["EmailSettings:Port"]);
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(emailDto.Name, emailDto.To));
                message.Subject = emailDto.Subject;

                // link  will user to page reset password
                var htmlContent = $"<a href='{emailDto.Url}'>Click here để đổi mật khẩu </a>";
                message.Body = new TextPart(TextFormat.Html) { Text = htmlContent };
                using (var client = new SmtpClient())
                {
                    client.Connect(host, port);
                    client.Authenticate(username, password);

                    await client.SendAsync(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
