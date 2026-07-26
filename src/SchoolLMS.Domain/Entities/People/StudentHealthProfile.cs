using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.People;

public class StudentHealthProfile : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? BloodType { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? Allergies { get; set; }
    public string? CurrentMedications { get; set; }
    public string? DisabilityInfo { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateOnly? MedicalExamDate { get; set; }
    public string? DentalExamNotes { get; set; }
    public string? VaccinationNotes { get; set; }
    public string? EmergencyHealthNotes { get; set; }

    public Student? Student { get; set; }
}
