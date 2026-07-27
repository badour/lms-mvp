using SchoolLMS.Domain.Entities.Academic;

namespace SchoolLMS.Domain.Entities.Lms;

public class LessonClassSection
{
    public int LessonId { get; set; }
    public int ClassSectionId { get; set; }

    public Lesson? Lesson { get; set; }
    public ClassSection? ClassSection { get; set; }
}
