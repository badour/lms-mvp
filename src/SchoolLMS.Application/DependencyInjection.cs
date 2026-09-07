using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchoolLMS.Application.Services.Attendance;
using SchoolLMS.Application.Services.Dashboards;
using SchoolLMS.Application.Services.Exams;
using SchoolLMS.Application.Services.Lessons;
using SchoolLMS.Application.Services.Messages;
using SchoolLMS.Application.Services.Routines;
using SchoolLMS.Application.Services.Schedules;
using SchoolLMS.Application.Services.Schools;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Application.Services.Teachers;
using SchoolLMS.Application.Validators;

namespace SchoolLMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateSchoolRequestValidator>();
        services.AddScoped<ISchoolService, SchoolService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IQimamCertificateService, QimamCertificateService>();
        services.AddScoped<IStudentInboxService, StudentInboxService>();
        services.AddScoped<IAdminMessagingService, AdminMessagingService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IAttendanceAdminService, AttendanceAdminService>();
        services.AddScoped<IExamAdminService, ExamAdminService>();
        services.AddScoped<IScheduleAdminService, ScheduleAdminService>();
        services.AddScoped<IRoutineLessonService, RoutineLessonService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
