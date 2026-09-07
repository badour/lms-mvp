using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Domain.Interfaces;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Infrastructure.Services.Audit;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(
        ApplicationDbContext db,
        ICurrentUserContext currentUser,
        IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string action,
        string? entityName = null,
        string? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        int? schoolId = null,
        bool success = true,
        string? failureReason = null,
        CancellationToken cancellationToken = default)
    {
        var http = _httpContextAccessor.HttpContext;
        var entry = new AuditLog
        {
            UserId = _currentUser.UserId,
            UserName = _currentUser.UserName,
            SchoolId = schoolId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues is null ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues is null ? null : JsonSerializer.Serialize(newValues),
            IpAddress = http?.Connection.RemoteIpAddress?.ToString(),
            Browser = http?.Request.Headers.UserAgent.ToString(),
            Device = http?.Request.Headers["sec-ch-ua-platform"].ToString(),
            Success = success,
            FailureReason = failureReason,
            CreatedAt = DateTime.UtcNow
        };

        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
