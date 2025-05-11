namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string Role { get; set; } = "User";
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
