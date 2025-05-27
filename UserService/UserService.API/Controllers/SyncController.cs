using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common;
using UserService.Application.Events;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly AuthServiceClient _authService;
    private readonly UserDbContext _db;
    private readonly IEventBus _eventBus;

    public SyncController(AuthServiceClient authService, UserDbContext db, IEventBus eventBus)
    {
        _authService = authService;
        _db = db;
        _eventBus = eventBus;
    }

    [HttpGet("missing")]
    public async Task<IActionResult> GetMissingEmails()
    {
        var authEmails = await _authService.GetAllEmailsAsync();
        var localEmails = await _db.Users.Select(u => u.Email).ToListAsync();

        var missing = authEmails.Except(localEmails).ToList();

        return Ok(missing);
    }

    [HttpPost("reprocess")]
    public async Task<IActionResult> ReprocessMissing()
    {
        var authEmails = await _authService.GetAllEmailsAsync();
        var localEmails = await _db.Users.Select(u => u.Email).ToListAsync();
        var missing = authEmails.Except(localEmails).ToList();

        int reprocessed = 0;

        foreach (var email in missing)
        {
            var detail = await _authService.GetUserDetailAsync(email);
            if (detail is null || string.IsNullOrWhiteSpace(detail.Email))
            {
                Console.WriteLine($"❌ Evento omitido. Usuario no encontrado o email vacío para: {email}");
                continue;
            }
            if (detail is not null)
            {
                Console.WriteLine($"Reprocesando: {email}");
                Console.WriteLine($"Detail → email: {detail?.Email}, role: {detail?.Role}, registeredAt: {detail?.RegisteredAt}");
                _eventBus.Publish(detail);
                _db.EventLogs.Add(new EventLog
                {
                    Email = detail.Email,
                    Status = "reprocesado",
                    Source = "sync"
                });

                reprocessed++;
            }
        }

        await _db.SaveChangesAsync();

        return Ok(new { Reprocessed = reprocessed });
    }
}
