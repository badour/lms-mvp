using FluentValidation;
using SchoolLMS.Application.DTOs.Lessons;
using SchoolLMS.Application.DTOs.Messages;
using SchoolLMS.Application.DTOs.Schools;
using SchoolLMS.Application.DTOs.Students;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Application.Validators;

public class CreateSchoolRequestValidator : AbstractValidator<CreateSchoolRequest>
{
    public CreateSchoolRequestValidator()
    {
        RuleFor(x => x.NameAr).NotEmpty().WithMessage("اسم المدرسة بالعربية مطلوب.").MaximumLength(200);
        RuleFor(x => x.NameEn).NotEmpty().WithMessage("School English name is required.").MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("البريد الإلكتروني غير صالح.");
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}

public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    private static readonly HashSet<string> AllowedBloodTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
    };

    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.SchoolId).GreaterThan(0).WithMessage("المدرسة مطلوبة.");
        RuleFor(x => x.FullNameAr).NotEmpty().WithMessage("اسم الطالب مطلوب.").MaximumLength(200);
        RuleFor(x => x.FatherName).NotEmpty().WithMessage("اسم الأب / ولي الأمر مطلوب.").MaximumLength(200);
        RuleFor(x => x.MotherName).NotEmpty().WithMessage("اسم الأم مطلوب.").MaximumLength(200);
        RuleFor(x => x.PassportOrCardId).NotEmpty().WithMessage("رقم البطاقة أو جواز السفر مطلوب.").MaximumLength(100);
        RuleFor(x => x.DateOfBirth).NotNull().WithMessage("تاريخ الميلاد مطلوب.");
        RuleFor(x => x.AdmissionDate).NotNull().WithMessage("تاريخ القبول مطلوب.");
        RuleFor(x => x.GradeLevelId).NotNull().WithMessage("الصف مطلوب.");
        RuleFor(x => x.ClassSectionId).NotNull().WithMessage("الشعبة / اسم الصف مطلوب.");
        RuleFor(x => x.Phone1).NotEmpty().WithMessage("رقم الهاتف 1 مطلوب.").MaximumLength(30);
        RuleFor(x => x.Phone2).MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.Phone2));
        RuleFor(x => x.City).NotEmpty().WithMessage("المدينة مطلوبة.").MaximumLength(100);
        RuleFor(x => x.Region).NotEmpty().WithMessage("المنطقة مطلوبة.").MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().WithMessage("العنوان مطلوب.").MaximumLength(500);
        RuleFor(x => x.BloodType)
            .NotEmpty().WithMessage("فصيلة الدم مطلوبة.")
            .Must(x => AllowedBloodTypes.Contains(x.Trim()))
            .WithMessage("فصيلة الدم غير صالحة.");
        RuleFor(x => x.ClassClassification).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ClassClassification));
        RuleFor(x => x.EmergencyContactName).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactName));
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30).When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone));
        RuleForEach(x => x.Hobbies).MaximumLength(150).When(x => x.Hobbies is not null);
        RuleForEach(x => x.NotesList).MaximumLength(2000).When(x => x.NotesList is not null);
    }
}

public class CreateQimamCertificateRequestValidator : AbstractValidator<CreateQimamCertificateRequest>
{
    public CreateQimamCertificateRequestValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("الطالب مطلوب.");
        RuleFor(x => x.CertificateName).NotEmpty().WithMessage("اسم الشهادة مطلوب.").MaximumLength(250);
        RuleFor(x => x.CertificateDate).NotNull().WithMessage("تاريخ الشهادة مطلوب.");
        RuleFor(x => x.ClassName).NotEmpty().WithMessage("اسم الصف مطلوب.").MaximumLength(150);
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
        RuleFor(x => x.Description).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

public class SendStudentInboxMessageRequestValidator : AbstractValidator<SendStudentInboxMessageRequest>
{
    public SendStudentInboxMessageRequestValidator()
    {
        RuleFor(x => x.TargetType).IsInEnum().WithMessage("نوع المستلم غير صالح.");
        RuleFor(x => x.Subject).NotEmpty().WithMessage("الموضوع مطلوب.").MaximumLength(250);
        RuleFor(x => x.Body).NotEmpty().WithMessage("نص الرسالة مطلوب.").MaximumLength(4000);
        RuleFor(x => x.TeacherId)
            .NotNull()
            .WithMessage("اختر المعلم المستلم.")
            .When(x => x.TargetType == StudentMessageTargetType.Instructor);
    }
}

public class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator()
    {
        RuleFor(x => x.SchoolId).GreaterThan(0).WithMessage("المدرسة مطلوبة.");
        RuleFor(x => x.SubjectId).GreaterThan(0).WithMessage("اسم الدرس / المادة مطلوب.");
        RuleFor(x => x.TeacherId).GreaterThan(0).WithMessage("المعلم مطلوب.");
        RuleFor(x => x.ClassSectionIds).NotEmpty().WithMessage("اختر صفاً واحداً على الأقل.");
        RuleFor(x => x.LessonDateTime).NotNull().WithMessage("تاريخ ووقت الدرس مطلوب.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("حالة الدرس غير صالحة.");
        RuleFor(x => x.Description).MaximumLength(4000).When(x => !string.IsNullOrWhiteSpace(x.Description));
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
