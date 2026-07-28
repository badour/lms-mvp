namespace SchoolLMS.Domain.Interfaces;

public interface IUserDirectory
{
    Task<IReadOnlyList<string>> GetActiveUserIdsByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetActiveUserIdsByRoleForSchoolAsync(string roleName, int schoolId, CancellationToken cancellationToken = default);
    Task<string?> GetDisplayNameAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, string>> GetDisplayNamesAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(string UserId, string DisplayName, string? UserName)>> GetSchoolManagementAccountsAsync(int schoolId, CancellationToken cancellationToken = default);
}
