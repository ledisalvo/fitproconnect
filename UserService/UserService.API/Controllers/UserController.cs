using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Services;
using UserService.Application.Common;

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

    [HttpGet("{email}")]
    public async Task<ActionResult<User>> GetByEmail(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user is null)
            return NotFound(new { message = "User not found." });

        return Ok(user);
    }

    [HttpGet("emails")]
    public async Task<IActionResult> GetAllEmails()
    {
        var emails = await _db.Users
            .Select(u => u.Email)
            .ToListAsync();

        return Ok(emails);
    }

    [HttpGet("missing")]
    public async Task<IActionResult> GetMissingFromUserService(
    [FromServices] AuthServiceClient authService,
    [FromServices] UserDbContext db)
    {
        var authEmails = await authService.GetAllEmailsAsync();
        var userEmails = await db.Users.Select(u => u.Email).ToListAsync();

        var missing = authEmails.Except(userEmails).ToList();

        return Ok(missing);
    }

    [HttpPost("sync-missing")]
    public async Task<IActionResult> SyncMissing(
    [FromServices] AuthServiceClient auth,
    [FromServices] UserDbContext db,
    [FromServices] IEventBus bus)
    {
        var authEmails = await auth.GetAllEmailsAsync();
        var userEmails = await db.Users.Select(u => u.Email).ToListAsync();
        var missing = authEmails.Except(userEmails).ToList();

        int count = 0;

        foreach (var email in missing)
        {
            var user = await auth.GetUserDetailAsync(email);
            if (user is not null)
            {
                bus.Publish(user);
                count++;
            }
            db.EventLogs.Add(new EventLog
            {
                Email = user.Email,
                Status = "reprocesado",
                Source = "sync"
            });
            await db.SaveChangesAsync();

        }

        return Ok(new { Published = count });
    }

    [HttpGet("event-logs")]
    public async Task<IActionResult> GetEventLogs()
    {
        var logs = await _db.EventLogs
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        return Ok(logs);
    }
}


