namespace SchoolLMS.Domain.Common;

public abstract class SchoolOwnedEntity : AuditableEntity
{
    public int SchoolId { get; set; }
}
