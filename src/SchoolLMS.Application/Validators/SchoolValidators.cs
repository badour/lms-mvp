using FluentValidation;
using SchoolLMS.Application.DTOs.Schools;
using SchoolLMS.Application.DTOs.Students;

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
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.SchoolId).GreaterThan(0).WithMessage("المدرسة مطلوبة.");
        RuleFor(x => x.StudentNumber).NotEmpty().WithMessage("رقم الطالب مطلوب.").MaximumLength(50);
        RuleFor(x => x.FullNameAr).NotEmpty().WithMessage("اسم الطالب بالعربية مطلوب.").MaximumLength(200);
        RuleFor(x => x.Gender).IsInEnum().WithMessage("الجنس غير صالح.");
    }
}
