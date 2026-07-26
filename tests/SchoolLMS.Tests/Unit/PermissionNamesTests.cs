using FluentAssertions;
using SchoolLMS.Application.Authorization;

namespace SchoolLMS.Tests.Unit;

public class PermissionNamesTests
{
    [Fact]
    public void Permission_catalog_contains_core_student_permissions()
    {
        PermissionNames.All.Select(x => x.Key).Should().Contain(new[]
        {
            PermissionNames.StudentsView,
            PermissionNames.StudentsCreate,
            PermissionNames.StudentsEdit
        });
    }

    [Fact]
    public void Role_catalog_contains_sixteen_roles()
    {
        AppRoles.All.Should().HaveCount(16);
        AppRoles.All.Select(x => x.Key).Should().Contain(AppRoles.SuperAdministrator);
    }
}
