namespace SchoolLMS.Application.DTOs.Dashboards;

public class AdminDashboardDto
{
    public int TotalSchools { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int ActiveUsers { get; set; }
    public int PresentToday { get; set; }
    public int AbsentToday { get; set; }
    public int LateToday { get; set; }
    public decimal CollectedFees { get; set; }
    public decimal OutstandingFees { get; set; }
    public int PendingHomework { get; set; }
    public IReadOnlyList<AnnouncementCardDto> Announcements { get; set; } = Array.Empty<AnnouncementCardDto>();
    public IReadOnlyList<ChartPointDto> StudentsPerSchool { get; set; } = Array.Empty<ChartPointDto>();
}

public class StudentDashboardDto
{
    public string StudentNameAr { get; set; } = string.Empty;
    public string? ProfileImagePath { get; set; }
    public string SchoolNameAr { get; set; } = string.Empty;
    public string? GradeNameAr { get; set; }
    public string? SectionNameAr { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string? AcademicYearNameAr { get; set; }
    public decimal AttendancePercent { get; set; }
    public int DaysPresent { get; set; }
    public int DaysAbsent { get; set; }
    public int DaysLate { get; set; }
    public int PendingHomework { get; set; }
    public int SubmittedHomework { get; set; }
    public IReadOnlyList<AnnouncementCardDto> Announcements { get; set; } = Array.Empty<AnnouncementCardDto>();
    public IReadOnlyList<SimpleItemDto> UpcomingExams { get; set; } = Array.Empty<SimpleItemDto>();
}

public class TeacherDashboardDto
{
    public string TeacherNameAr { get; set; } = string.Empty;
    public int TodayClasses { get; set; }
    public int AssignedSubjects { get; set; }
    public int PendingHomeworkReviews { get; set; }
    public int UnreadMessages { get; set; }
    public IReadOnlyList<SimpleItemDto> TodaySchedule { get; set; } = Array.Empty<SimpleItemDto>();
    public IReadOnlyList<SimpleItemDto> UpcomingExams { get; set; } = Array.Empty<SimpleItemDto>();
}

public class ParentDashboardDto
{
    public IReadOnlyList<ChildSummaryDto> Children { get; set; } = Array.Empty<ChildSummaryDto>();
    public ChildSummaryDto? SelectedChild { get; set; }
    public decimal FeeBalance { get; set; }
    public IReadOnlyList<AnnouncementCardDto> Announcements { get; set; } = Array.Empty<AnnouncementCardDto>();
}

public class ChildSummaryDto
{
    public int StudentId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? GradeNameAr { get; set; }
    public decimal AttendancePercent { get; set; }
    public int PendingHomework { get; set; }
}

public class AnnouncementCardDto
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public DateTime PublishAt { get; set; }
}

public class ChartPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class SimpleItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public DateTime? Date { get; set; }
}
