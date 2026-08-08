using System.ComponentModel.DataAnnotations;

namespace SchoolLMS.Application.DTOs.Schedules;

public class ScheduleListItemDto
{
    public int ClassSectionId { get; set; }
    public int SchoolId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string SectionNameAr { get; set; } = string.Empty;
    public string YearNameAr { get; set; } = string.Empty;
    public int FilledSlots { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SchedulePeriodDto
{
    public int SortOrder { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
}

public class ScheduleCellDto
{
    public byte DayOfWeek { get; set; }
    public int PeriodSortOrder { get; set; }
    public int? SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public string? LessonName { get; set; }
    public string? TeacherName { get; set; }
    public string? TimeSlot { get; set; }
    public string? EntryText { get; set; }
}

public class ScheduleDetailsDto
{
    public int SchoolId { get; set; }
    public int AcademicStageId { get; set; }
    public int ClassSectionId { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string StageNameAr { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string SectionNameAr { get; set; } = string.Empty;
    public List<string> PeriodNames { get; set; } = [];
    public List<SchedulePeriodDto> Periods { get; set; } = [];
    public List<ScheduleCellDto> Cells { get; set; } = [];
}

public class SaveScheduleRequest
{
    [Required(ErrorMessage = "المدرسة مطلوبة")]
    [Display(Name = "المدرسة")]
    public int SchoolId { get; set; }

    [Required(ErrorMessage = "المرحلة مطلوبة")]
    [Display(Name = "المرحلة")]
    public int AcademicStageId { get; set; }

    [Required(ErrorMessage = "الشعبة مطلوبة")]
    [Display(Name = "الشعبة")]
    public int ClassSectionId { get; set; }

    public List<ScheduleCellDto> Cells { get; set; } = [];
}
