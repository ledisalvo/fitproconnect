using AuthService.Application.Dtos;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new { Email = email, Message = "Token válido 💪" });
    }

    [HttpGet("emails")]
    public async Task<IActionResult> GetAllEmails([FromServices] AuthDbContext db)
    {
        var emails = await db.Users
            .Select(u => u.Email)
            .ToListAsync();

        return Ok(emails);
    }

    [HttpGet("detail/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email, [FromServices] AuthDbContext db)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
        if (user is null)
            return NotFound();

        return Ok(new
        {
            user.Email,
            user.Role,
            user.RegisteredAt
        });
    }

}
