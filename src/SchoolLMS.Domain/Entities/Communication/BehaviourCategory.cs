using SchoolLMS.Domain.Common;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Domain.Entities.Communication;

public class BehaviourCategory : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public BehaviourPolarity Polarity { get; set; }
    public int DefaultPoints { get; set; }
    public bool IsActive { get; set; } = true;
}
