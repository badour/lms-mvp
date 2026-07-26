using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Transport;

public class TransportRoute : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int? VehicleId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Vehicle? Vehicle { get; set; }
}
