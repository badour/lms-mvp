namespace SchoolLMS.Domain.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string? entityName = null, string? entityId = null,
        object? oldValues = null, object? newValues = null, int? schoolId = null,
        bool success = true, string? failureReason = null, CancellationToken cancellationToken = default);
}
