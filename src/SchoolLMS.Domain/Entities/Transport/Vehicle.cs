using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Transport;

public class Vehicle : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string? Model { get; set; }
    public int Capacity { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public bool IsActive { get; set; } = true;
}
