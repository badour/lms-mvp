using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Infrastructure.Identity;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => User?.Identity?.Name;
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public bool IsSuperAdmin => Roles.Contains(AppRoles.SuperAdministrator);

    public IReadOnlyCollection<int> SchoolIds =>
        User?.FindAll("school_id").Select(c => int.TryParse(c.Value, out var id) ? id : 0).Where(x => x > 0).Distinct().ToArray()
        ?? Array.Empty<int>();

    public IReadOnlyCollection<int> BranchIds =>
        User?.FindAll("branch_id").Select(c => int.TryParse(c.Value, out var id) ? id : 0).Where(x => x > 0).Distinct().ToArray()
        ?? Array.Empty<int>();

    public IReadOnlyCollection<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToArray()
        ?? Array.Empty<string>();

    public IReadOnlyCollection<string> Permissions =>
        User?.FindAll("permission").Select(c => c.Value).Distinct().ToArray()
        ?? Array.Empty<string>();

    public bool HasPermission(string permission) =>
        IsSuperAdmin || Permissions.Contains(permission);

    public bool CanAccessSchool(int schoolId) =>
        IsSuperAdmin || SchoolIds.Contains(schoolId);
}
