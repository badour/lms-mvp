namespace SchoolLMS.Domain.Interfaces;

public interface IUserDirectory
{
    Task<IReadOnlyList<string>> GetActiveUserIdsByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetActiveUserIdsByRoleForSchoolAsync(string roleName, int schoolId, CancellationToken cancellationToken = default);
}
