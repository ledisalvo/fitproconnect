using AuthService.Application.Common;
using AuthService.Application.Dtos;
using AuthService.Application.Events;
using AuthService.Application.Interfaces;
using AuthService.Domain.Constants;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _config;
    private readonly IEventBus _eventBus;

    public AuthService(AuthDbContext context, IConfiguration config, IEventBus eventBus)
    {
        _context = context;
        _config = config;
        _eventBus = eventBus;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {

        if (!Roles.All.Contains(request.Role ?? Roles.User))
            throw new Exception($"Rol inválido. Los roles válidos son: {string.Join(", ", Roles.All)}");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("El email ya está registrado");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = request.Role ?? Roles.User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _eventBus.Publish(new UserRegisteredEvent
        {
            Email = user.Email,
            Role = user.Role
        });

        return new AuthResponseDto
        {
            Email = user.Email,
            Token = GenerateJwt(user)
        };


    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Credenciales inválidas");

        return new AuthResponseDto
        {
            Email = user.Email,
            Token = GenerateJwt(user)
        };
    }

    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
