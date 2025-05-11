namespace AuthService.Application.Events;

public class UserRegisteredEvent
{
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
