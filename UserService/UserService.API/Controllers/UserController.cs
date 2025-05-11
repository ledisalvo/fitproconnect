using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserDbContext _db;

    public UserController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new
        {
            Message = "Bienvenido a UserService 💼",
            Email = email
        });
    }

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdminArea()
    {
        var user = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Ok(new { Message = $"Área exclusiva de administradores, bienvenido {user} 😎" });
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _db.Users.ToListAsync();
        return Ok(users);
    }

}


