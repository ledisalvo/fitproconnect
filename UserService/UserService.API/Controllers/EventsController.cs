using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Data;
using UserService.Domain.Entities;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly UserDbContext _db;

    public EventsController(UserDbContext db)
    {
        _db = db;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetEventLogs(
        [FromQuery] string? email,
        [FromQuery] string? status,
        [FromQuery] string? source)
    {
        var query = _db.EventLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(e => e.Email == email);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(e => e.Source == source);

        var logs = await query
            .OrderByDescending(e => e.Timestamp)
            .ToListAsync();

        return Ok(logs);
    }
}
