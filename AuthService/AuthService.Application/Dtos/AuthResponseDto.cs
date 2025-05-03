namespace AuthService.Application.Dtos;

public class AuthResponseDto
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
