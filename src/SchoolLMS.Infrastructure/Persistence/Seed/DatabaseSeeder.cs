using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolLMS.Application.Authorization;
using SchoolLMS.Domain.Entities.Academic;
using SchoolLMS.Domain.Entities.Communication;
using SchoolLMS.Domain.Entities.Finance;
using SchoolLMS.Domain.Entities.Identity;
using SchoolLMS.Domain.Entities.People;
using SchoolLMS.Domain.Entities.Tenancy;
using SchoolLMS.Domain.Enums;

namespace SchoolLMS.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
        var db = sp.GetRequiredService<ApplicationDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

        // SQL Server uses EF migrations (see database/SchoolLMS.Schema.sql).
        // SQLite keeps EnsureCreated for lightweight local/dev databases.
        if (db.Database.IsSqlServer())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        foreach (var (key, nameAr) in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(key))
            {
                await roleManager.CreateAsync(new IdentityRole(key));
            }
        }

        if (!await db.Permissions.AnyAsync())
        {
            db.Permissions.AddRange(PermissionNames.All.Select(p => new Permission
            {
                Key = p.Key,
                NameAr = p.NameAr,
                NameEn = p.NameEn,
                Area = p.Area
            }));
            await db.SaveChangesAsync();
        }

        var permissions = await db.Permissions.ToListAsync();
        var superRole = await roleManager.FindByNameAsync(AppRoles.SuperAdministrator);
        if (superRole is not null)
        {
            var existing = await db.RolePermissions.Where(x => x.RoleId == superRole.Id).Select(x => x.PermissionId).ToListAsync();
            foreach (var permission in permissions.Where(p => !existing.Contains(p.Id)))
            {
                db.RolePermissions.Add(new RolePermission { RoleId = superRole.Id, PermissionId = permission.Id });
            }

            await db.SaveChangesAsync();
        }

        var adminEmail = "admin@schoollms.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FullNameAr = "المشرف العام",
                FullNameEn = "Super Administrator",
                IsActive = true,
                PreferredCulture = "ar"
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdministrator);
            }
        }

        // Keep school-management demo accounts available even when schools were seeded earlier.
        foreach (var school in await db.Schools.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync())
        {
            var email = $"schooladmin{school.Id}@schoollms.local";
            var schoolAdmin = await userManager.FindByEmailAsync(email);
            if (schoolAdmin is null)
            {
                schoolAdmin = new ApplicationUser
                {
                    UserName = $"schooladmin{school.Id}",
                    Email = email,
                    EmailConfirmed = true,
                    FullNameAr = $"إدارة المدرسة {school.Id}",
                    IsActive = true,
                    PreferredCulture = "ar"
                };
                var created = await userManager.CreateAsync(schoolAdmin, "SchoolAdmin@12345");
                if (created.Succeeded)
                {
                    await userManager.AddToRoleAsync(schoolAdmin, AppRoles.SchoolAdministrator);
                    db.UserSchoolAssignments.Add(new UserSchoolAssignment
                    {
                        UserId = schoolAdmin.Id,
                        SchoolId = school.Id,
                        IsActive = true
                    });
                    await db.SaveChangesAsync();
                }
            }
            else if (!await userManager.IsInRoleAsync(schoolAdmin, AppRoles.SchoolAdministrator))
            {
                await userManager.AddToRoleAsync(schoolAdmin, AppRoles.SchoolAdministrator);
            }
        }

        if (await db.Schools.AnyAsync())
        {
            return;
        }

        var schools = new[]
        {
            ("مدرسة النور الأهلية", "Al-Noor Private School", "بغداد - الكرادة"),
            ("مدرسة الأمل النموذجية", "Al-Amal Model School", "بغداد - المنصور"),
            ("مدرسة المستقبل", "Future School", "البصرة"),
            ("مدرسة الإبداع", "Ibdaa School", "أربيل")
        }.Select(s => new School
        {
            NameAr = s.Item1,
            NameEn = s.Item2,
            Address = s.Item3,
            Phone = "07700000000",
            Email = "info@school.local",
            SchoolType = "Private",
            GenderType = "CoEducational",
            Currency = "IQD",
            IsActive = true,
            Settings = new SchoolSettings { DefaultCulture = "ar", DateFormat = "yyyy/MM/dd" }
        }).ToList();

        db.Schools.AddRange(schools);
        await db.SaveChangesAsync();

        foreach (var school in schools)
        {
            db.SchoolBranches.Add(new SchoolBranch
            {
                SchoolId = school.Id,
                NameAr = "الفرع الرئيسي",
                NameEn = "Main Branch",
                Address = school.Address,
                IsActive = true
            });

            var year = new AcademicYear
            {
                SchoolId = school.Id,
                NameAr = "2025-2026",
                NameEn = "2025-2026",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2026, 6, 30),
                Status = AcademicYearStatus.Open,
                IsCurrent = true
            };
            db.AcademicYears.Add(year);

            var stage = new AcademicStage
            {
                SchoolId = school.Id,
                NameAr = "المرحلة الابتدائية",
                NameEn = "Primary",
                SortOrder = 1
            };
            db.AcademicStages.Add(stage);
            await db.SaveChangesAsync();

            var grade = new GradeLevel
            {
                SchoolId = school.Id,
                AcademicStageId = stage.Id,
                NameAr = "الصف الخامس",
                NameEn = "Grade 5",
                SortOrder = 5
            };
            db.GradeLevels.Add(grade);
            await db.SaveChangesAsync();

            var section = new ClassSection
            {
                SchoolId = school.Id,
                GradeLevelId = grade.Id,
                NameAr = "أ",
                NameEn = "A",
                Capacity = 30
            };
            db.ClassSections.Add(section);

            var math = new Subject { SchoolId = school.Id, NameAr = "الرياضيات", NameEn = "Mathematics", Code = "MATH" };
            var arabic = new Subject { SchoolId = school.Id, NameAr = "اللغة العربية", NameEn = "Arabic", Code = "AR" };
            db.Subjects.AddRange(math, arabic);

            db.FeeTypes.Add(new FeeType { SchoolId = school.Id, NameAr = "قسط دراسي", NameEn = "Tuition" });
            db.Badges.Add(new Badge { SchoolId = school.Id, NameAr = "متفوق", NameEn = "Excellence", Category = "Academic", Points = 10 });
            db.Announcements.Add(new Announcement
            {
                SchoolId = school.Id,
                TitleAr = "مرحباً بكم في نظام إدارة المدارس",
                ContentAr = "تم تهيئة النظام بالبيانات التجريبية.",
                Priority = AnnouncementPriority.Important,
                IsPublished = true,
                PublishAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();

            var schoolAdminUser = new ApplicationUser
            {
                UserName = $"schooladmin{school.Id}",
                Email = $"schooladmin{school.Id}@schoollms.local",
                EmailConfirmed = true,
                FullNameAr = $"إدارة المدرسة {school.Id}",
                IsActive = true,
                PreferredCulture = "ar"
            };
            var existingSchoolAdmin = await userManager.FindByEmailAsync(schoolAdminUser.Email);
            if (existingSchoolAdmin is null)
            {
                await userManager.CreateAsync(schoolAdminUser, "SchoolAdmin@12345");
                await userManager.AddToRoleAsync(schoolAdminUser, AppRoles.SchoolAdministrator);
                db.UserSchoolAssignments.Add(new UserSchoolAssignment
                {
                    UserId = schoolAdminUser.Id,
                    SchoolId = school.Id,
                    IsActive = true
                });
            }
            else
            {
                schoolAdminUser = existingSchoolAdmin;
                if (!await userManager.IsInRoleAsync(schoolAdminUser, AppRoles.SchoolAdministrator))
                {
                    await userManager.AddToRoleAsync(schoolAdminUser, AppRoles.SchoolAdministrator);
                }
            }

            var teacherUser = new ApplicationUser
            {
                UserName = $"teacher{school.Id}",
                Email = $"teacher{school.Id}@schoollms.local",
                EmailConfirmed = true,
                FullNameAr = $"معلم المدرسة {school.Id}",
                IsActive = true,
                PreferredCulture = "ar"
            };
            var existingTeacherUser = await userManager.FindByEmailAsync(teacherUser.Email);
            if (existingTeacherUser is null)
            {
                await userManager.CreateAsync(teacherUser, "Teacher@12345");
                await userManager.AddToRoleAsync(teacherUser, AppRoles.Teacher);
                db.UserSchoolAssignments.Add(new UserSchoolAssignment
                {
                    UserId = teacherUser.Id,
                    SchoolId = school.Id,
                    IsActive = true
                });
            }
            else
            {
                teacherUser = existingTeacherUser;
            }

            var teacher = new Teacher
            {
                SchoolId = school.Id,
                UserId = teacherUser.Id,
                FullNameAr = teacherUser.FullNameAr,
                Specialization = "رياضيات",
                Email = teacherUser.Email,
                IsActive = true
            };
            db.Teachers.Add(teacher);

            var studentUser = new ApplicationUser
            {
                UserName = $"student{school.Id}",
                Email = $"student{school.Id}@schoollms.local",
                EmailConfirmed = true,
                FullNameAr = $"طالب تجريبي {school.Id}",
                IsActive = true,
                PreferredCulture = "ar"
            };
            if (await userManager.FindByEmailAsync(studentUser.Email) is null)
            {
                await userManager.CreateAsync(studentUser, "Student@12345");
                await userManager.AddToRoleAsync(studentUser, AppRoles.Student);
                db.UserSchoolAssignments.Add(new UserSchoolAssignment
                {
                    UserId = studentUser.Id,
                    SchoolId = school.Id,
                    IsActive = true
                });
            }

            var student = new Student
            {
                SchoolId = school.Id,
                UserId = studentUser.Id,
                StudentNumber = $"S{school.Id:000}-001",
                FullNameAr = studentUser.FullNameAr,
                FullNameEn = $"Demo Student {school.Id}",
                Gender = Gender.Male,
                DateOfBirth = new DateOnly(2014, 5, 1),
                Nationality = "عراقي",
                RegistrationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                Status = StudentStatus.Active
            };
            db.Students.Add(student);
            await db.SaveChangesAsync();

            db.StudentEnrollments.Add(new StudentEnrollment
            {
                SchoolId = school.Id,
                StudentId = student.Id,
                AcademicYearId = year.Id,
                GradeLevelId = grade.Id,
                ClassSectionId = section.Id,
                Status = EnrollmentStatus.Active,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow),
                SeatNumber = 1
            });

            var parentUser = new ApplicationUser
            {
                UserName = $"parent{school.Id}",
                Email = $"parent{school.Id}@schoollms.local",
                EmailConfirmed = true,
                FullNameAr = $"ولي أمر {school.Id}",
                IsActive = true,
                PreferredCulture = "ar"
            };
            if (await userManager.FindByEmailAsync(parentUser.Email) is null)
            {
                await userManager.CreateAsync(parentUser, "Parent@12345");
                await userManager.AddToRoleAsync(parentUser, AppRoles.Parent);
                db.UserSchoolAssignments.Add(new UserSchoolAssignment
                {
                    UserId = parentUser.Id,
                    SchoolId = school.Id,
                    IsActive = true
                });
            }

            var guardian = new Guardian
            {
                SchoolId = school.Id,
                UserId = parentUser.Id,
                FullNameAr = parentUser.FullNameAr,
                Phone = "07800000000",
                Email = parentUser.Email
            };
            db.Guardians.Add(guardian);
            await db.SaveChangesAsync();

            db.StudentGuardians.Add(new StudentGuardian
            {
                StudentId = student.Id,
                GuardianId = guardian.Id,
                Relationship = "أب",
                IsPrimary = true,
                IsFinanciallyResponsible = true,
                CanReceiveNotifications = true,
                CanCollectStudent = true
            });

            db.TeacherAssignments.Add(new TeacherAssignment
            {
                SchoolId = school.Id,
                TeacherId = teacher.Id,
                AcademicYearId = year.Id,
                SubjectId = math.Id,
                ClassSectionId = section.Id,
                IsActive = true
            });

            await db.SaveChangesAsync();
        }

        logger.LogInformation("Database seeded with demo Arabic schools and users.");
    }
}
