using System.ComponentModel.DataAnnotations;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.DTOs.Attendance;

public class AttendanceStudentRowDto
{
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public bool IsPresent { get; set; } = true;
}

public class AttendanceRosterRequest
{
    public int TeacherId { get; set; }
    public int AcademicStageId { get; set; }
    public int ClassSectionId { get; set; }
    public DateOnly? AttendanceDate { get; set; }
}

public class SaveAttendanceRequest
{
    [Required(ErrorMessage = "المعلم مطلوب")]
    [Display(Name = "المعلم")]
    public int TeacherId { get; set; }

    [Required(ErrorMessage = "المرحلة مطلوبة")]
    [Display(Name = "المرحلة")]
    public int AcademicStageId { get; set; }

    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [Display(Name = "الشعبة")]
    public int ClassSectionId { get; set; }

    [Required(ErrorMessage = "التاريخ مطلوب")]
    [Display(Name = "التاريخ")]
    [DataType(DataType.Date)]
    public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public List<AttendanceStudentMarkDto> Students { get; set; } = [];
}

public class AttendanceStudentMarkDto
{
    public int StudentId { get; set; }
    public bool IsPresent { get; set; } = true;
}

public class AttendanceSessionListItemDto
{
    public int Id { get; set; }
    public string TeacherNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string SectionNameAr { get; set; } = string.Empty;
    public DateOnly AttendanceDate { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public PublicationStatus Status { get; set; }
}
