namespace SchoolLMS.Application.Authorization;

public static class PermissionNames
{
    public const string SchoolsView = "Schools.View";
    public const string SchoolsCreate = "Schools.Create";
    public const string SchoolsEdit = "Schools.Edit";

    public const string StudentsView = "Students.View";
    public const string StudentsCreate = "Students.Create";
    public const string StudentsEdit = "Students.Edit";
    public const string StudentsDelete = "Students.Delete";
    public const string StudentsExport = "Students.Export";

    public const string TeachersView = "Teachers.View";
    public const string TeachersCreate = "Teachers.Create";
    public const string TeachersEdit = "Teachers.Edit";
    public const string TeachersDelete = "Teachers.Delete";

    public const string AcademicYearsView = "AcademicYears.View";
    public const string AcademicYearsManage = "AcademicYears.Edit";

    public const string GradesView = "Grades.View";
    public const string GradesEnter = "Grades.Enter";
    public const string GradesApprove = "Grades.Approve";

    public const string AttendanceRecord = "Attendance.Record";
    public const string AttendanceEdit = "Attendance.Edit";
    public const string AttendanceReports = "Attendance.Reports";

    public const string HomeworkCreate = "Homework.Create";
    public const string HomeworkSubmit = "Homework.Submit";
    public const string HomeworkReview = "Homework.Review";

    public const string LessonsView = "Lessons.View";
    public const string LessonsManage = "Lessons.Create";

    public const string PaymentsView = "Payments.View";
    public const string PaymentsReceive = "Payments.Receive";
    public const string PaymentsReports = "Payments.Reports";

    public const string UsersView = "Users.View";
    public const string UsersManage = "Users.Edit";
    public const string RolesManage = "Roles.Edit";
    public const string AuditLogsView = "AuditLogs.View";
    public const string AnnouncementsManage = "Announcements.Create";

    public static IReadOnlyList<(string Key, string NameAr, string NameEn, string Area)> All { get; } =
    [
        (SchoolsView, "عرض المدارس", "View schools", "Schools"),
        (SchoolsCreate, "إنشاء مدرسة", "Create school", "Schools"),
        (SchoolsEdit, "تعديل مدرسة", "Edit school", "Schools"),
        (StudentsView, "عرض الطلاب", "View students", "Students"),
        (StudentsCreate, "إنشاء طالب", "Create student", "Students"),
        (StudentsEdit, "تعديل طالب", "Edit student", "Students"),
        (StudentsDelete, "حذف طالب", "Delete student", "Students"),
        (StudentsExport, "تصدير الطلاب", "Export students", "Students"),
        (TeachersView, "عرض المعلمين", "View teachers", "Teachers"),
        (TeachersCreate, "إنشاء معلم", "Create teacher", "Teachers"),
        (TeachersEdit, "تعديل معلم", "Edit teacher", "Teachers"),
        (TeachersDelete, "حذف معلم", "Delete teacher", "Teachers"),
        (AcademicYearsView, "عرض السنوات الدراسية", "View academic years", "Academic"),
        (AcademicYearsManage, "إدارة السنوات الدراسية", "Manage academic years", "Academic"),
        (GradesView, "عرض الدرجات", "View grades", "Grades"),
        (GradesEnter, "إدخال الدرجات", "Enter grades", "Grades"),
        (GradesApprove, "اعتماد الدرجات", "Approve grades", "Grades"),
        (AttendanceRecord, "تسجيل الحضور", "Record attendance", "Attendance"),
        (AttendanceEdit, "تعديل الحضور", "Edit attendance", "Attendance"),
        (AttendanceReports, "تقارير الحضور", "Attendance reports", "Attendance"),
        (HomeworkCreate, "إنشاء واجب", "Create homework", "Homework"),
        (HomeworkSubmit, "تسليم واجب", "Submit homework", "Homework"),
        (HomeworkReview, "مراجعة واجب", "Review homework", "Homework"),
        (LessonsView, "عرض الدروس", "View lessons", "Lessons"),
        (LessonsManage, "إدارة الدروس", "Manage lessons", "Lessons"),
        (PaymentsView, "عرض المدفوعات", "View payments", "Payments"),
        (PaymentsReceive, "استلام دفعة", "Receive payment", "Payments"),
        (PaymentsReports, "تقارير المالية", "Payment reports", "Payments"),
        (UsersView, "عرض المستخدمين", "View users", "Users"),
        (UsersManage, "إدارة المستخدمين", "Manage users", "Users"),
        (RolesManage, "إدارة الأدوار", "Manage roles", "Roles"),
        (AuditLogsView, "عرض سجل التدقيق", "View audit logs", "Audit"),
        (AnnouncementsManage, "إدارة الإعلانات", "Manage announcements", "Announcements")
    ];
}
