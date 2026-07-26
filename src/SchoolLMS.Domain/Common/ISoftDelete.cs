namespace SchoolLMS.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedByUserId { get; set; }
}
