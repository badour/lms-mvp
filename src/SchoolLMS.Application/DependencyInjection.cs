using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchoolLMS.Application.Services.Dashboards;
using SchoolLMS.Application.Services.Lessons;
using SchoolLMS.Application.Services.Messages;
using SchoolLMS.Application.Services.Schools;
using SchoolLMS.Application.Services.Students;
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
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
