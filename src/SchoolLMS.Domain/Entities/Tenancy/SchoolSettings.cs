using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Tenancy;

public class SchoolSettings : SchoolOwnedEntity
{
    public int Id { get; set; }
    public string WorkingDaysJson { get; set; } = "[\"Sunday\",\"Monday\",\"Tuesday\",\"Wednesday\",\"Thursday\"]";
    public string AttendanceRulesJson { get; set; } = "{}";
    public string GradeSystemJson { get; set; } = "{}";
    public string ThemeJson { get; set; } = "{}";
    public string SmsSettingsJson { get; set; } = "{}";
    public string NotificationSettingsJson { get; set; } = "{}";
    public string DateFormat { get; set; } = "yyyy/MM/dd";
    public string DefaultCulture { get; set; } = "ar";

    public School? School { get; set; }
}
