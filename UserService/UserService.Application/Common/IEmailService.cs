namespace UserService.Application.Common;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
