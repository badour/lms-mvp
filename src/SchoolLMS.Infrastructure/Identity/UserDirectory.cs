using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Domain.Interfaces;
using SchoolLMS.Infrastructure.Persistence;

namespace SchoolLMS.Infrastructure.Identity;

public class UserDirectory : IUserDirectory
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserDirectory(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IReadOnlyList<string>> GetActiveUserIdsByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return Array.Empty<string>();
        }

        return await (
            from userRole in _db.UserRoles.AsNoTracking()
            join user in _db.Users.AsNoTracking() on userRole.UserId equals user.Id
            where userRole.RoleId == role.Id && user.IsActive
            select user.Id
        ).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetActiveUserIdsByRoleForSchoolAsync(string roleName, int schoolId, CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return Array.Empty<string>();
        }

        var byGlobalRole = await (
            from userRole in _db.UserRoles.AsNoTracking()
            join user in _db.Users.AsNoTracking() on userRole.UserId equals user.Id
            join assignment in _db.UserSchoolAssignments.AsNoTracking() on user.Id equals assignment.UserId
            where userRole.RoleId == role.Id
                  && user.IsActive
                  && assignment.IsActive
                  && !assignment.IsDeleted
                  && assignment.SchoolId == schoolId
            select user.Id
        ).Distinct().ToListAsync(cancellationToken);

        if (byGlobalRole.Count > 0)
        {
            return byGlobalRole;
        }

        return await (
            from schoolRole in _db.UserSchoolRoles.AsNoTracking()
            join user in _db.Users.AsNoTracking() on schoolRole.UserId equals user.Id
            where schoolRole.RoleId == role.Id
                  && schoolRole.SchoolId == schoolId
                  && schoolRole.IsActive
                  && user.IsActive
            select user.Id
        ).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<string?> GetDisplayNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.FullNameAr != null && x.FullNameAr != "" ? x.FullNameAr : x.UserName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetDisplayNamesAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        var ids = userIds.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<string, string>();
        }

        var rows = await _db.Users.AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, Name = x.FullNameAr != null && x.FullNameAr != "" ? x.FullNameAr : x.UserName })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(x => x.Id, x => x.Name ?? x.Id);
    }

    public async Task<IReadOnlyList<(string UserId, string DisplayName, string? UserName)>> GetSchoolManagementAccountsAsync(int schoolId, CancellationToken cancellationToken = default)
    {
        var ids = (await GetActiveUserIdsByRoleForSchoolAsync(AppRoles.SchoolAdministrator, schoolId, cancellationToken))
            .Concat(await GetActiveUserIdsByRoleForSchoolAsync(AppRoles.AcademicSupervisor, schoolId, cancellationToken))
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            ids = (await GetActiveUserIdsByRoleAsync(AppRoles.SuperAdministrator, cancellationToken)).ToList();
        }

        var names = await GetDisplayNamesAsync(ids, cancellationToken);
        var users = await _db.Users.AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.UserName })
            .ToListAsync(cancellationToken);

        return users
            .Select(x => (x.Id, names.TryGetValue(x.Id, out var name) ? name : x.UserName ?? x.Id, x.UserName))
            .OrderBy(x => x.Item2)
            .ToList();
    }
}
