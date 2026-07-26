using Microsoft.AspNetCore.Identity;

namespace SchoolLMS.Domain.Entities.Identity;

// IdentityUser comes from Microsoft.Extensions.Identity.Stores
public class ApplicationUser : IdentityUser
{
    public string FullNameAr { get; set; } = string.Empty;
    public string? FullNameEn { get; set; }
    public string? ProfileImagePath { get; set; }
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string PreferredCulture { get; set; } = "ar";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserSchoolAssignment> SchoolAssignments { get; set; } = new List<UserSchoolAssignment>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
