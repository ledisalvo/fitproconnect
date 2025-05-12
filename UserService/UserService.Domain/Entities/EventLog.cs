namespace UserService.Domain.Entities;

public class EventLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string EventType { get; set; } = "UserRegistered";
    public string Status { get; set; } = "consumido"; // o "reprocesado"
    public string Source { get; set; } = "evento";    // o "sync"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
