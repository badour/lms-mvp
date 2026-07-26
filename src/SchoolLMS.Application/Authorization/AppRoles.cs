namespace SchoolLMS.Application.Authorization;

public static class AppRoles
{
    public const string SuperAdministrator = "SuperAdministrator";
    public const string CentralAdministrator = "CentralAdministrator";
    public const string SchoolAdministrator = "SchoolAdministrator";
    public const string AcademicSupervisor = "AcademicSupervisor";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";
    public const string Accountant = "Accountant";
    public const string RegistrationOfficer = "RegistrationOfficer";
    public const string AttendanceOfficer = "AttendanceOfficer";
    public const string GuidanceCounsellor = "GuidanceCounsellor";
    public const string HealthOfficer = "HealthOfficer";
    public const string Librarian = "Librarian";
    public const string TransportationOfficer = "TransportationOfficer";
    public const string ContentManager = "ContentManager";
    public const string SupportUser = "SupportUser";

    public static IReadOnlyList<(string Key, string NameAr)> All { get; } =
    [
        (SuperAdministrator, "المشرف العام"),
        (CentralAdministrator, "الإدارة المركزية"),
        (SchoolAdministrator, "مدير المدرسة"),
        (AcademicSupervisor, "المشرف الأكاديمي"),
        (Teacher, "معلم"),
        (Student, "طالب"),
        (Parent, "ولي أمر"),
        (Accountant, "محاسب"),
        (RegistrationOfficer, "مسؤول التسجيل"),
        (AttendanceOfficer, "مسؤول الحضور"),
        (GuidanceCounsellor, "المرشد التربوي"),
        (HealthOfficer, "المسؤول الصحي"),
        (Librarian, "أمين المكتبة"),
        (TransportationOfficer, "مسؤول النقل"),
        (ContentManager, "مدير المحتوى"),
        (SupportUser, "مستخدم الدعم")
    ];
}
