namespace DemoProject.Infrastructure.Modules.EmailService;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}