namespace SchoolLMS.Domain.Interfaces;

public interface ICurrentUserContext
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    IReadOnlyCollection<int> SchoolIds { get; }
    IReadOnlyCollection<int> BranchIds { get; }
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool HasPermission(string permission);
    bool CanAccessSchool(int schoolId);
}
