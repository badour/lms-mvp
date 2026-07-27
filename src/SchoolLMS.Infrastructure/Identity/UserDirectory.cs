using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        // Fallback: school-scoped role assignments table.
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
}
