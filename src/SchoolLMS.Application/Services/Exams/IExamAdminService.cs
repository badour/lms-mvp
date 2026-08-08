using SchoolLMS.Application.Common;
using SchoolLMS.Application.DTOs.Exams;

namespace SchoolLMS.Application.Services.Exams;

public interface IExamAdminService
{
    Task<IReadOnlyList<ExamListItemDto>> ListRecentAsync(int? schoolId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentExamItemDto>> GetForCurrentStudentAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> CreateAsync(CreateExamRequest request, CancellationToken cancellationToken = default);
}
