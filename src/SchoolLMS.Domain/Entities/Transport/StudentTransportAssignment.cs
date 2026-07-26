using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Transport;

public class StudentTransportAssignment : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int TransportRouteId { get; set; }
    public string? PickupLocation { get; set; }
    public TimeOnly? ExpectedPickupTime { get; set; }
    public TimeOnly? ExpectedDropoffTime { get; set; }
    public bool IsActive { get; set; } = true;

    public TransportRoute? TransportRoute { get; set; }
}
