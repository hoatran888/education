using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope  = services.CreateScope();
        var db           = scope.ServiceProvider.GetRequiredService<SchoolDataContext>();
        var logger       = scope.ServiceProvider.GetRequiredService<ILogger<SchoolDataContext>>();

        await db.Database.MigrateAsync();

        if (await db.Schools.AnyAsync()) return; // already seeded

        logger.LogInformation("Seeding development data…");

        // ── IDs ──────────────────────────────────────────────────────────────
        var schoolId       = Guid.NewGuid();
        var adminUserId    = Guid.NewGuid();
        var teacher1Id     = Guid.NewGuid();
        var teacher2Id     = Guid.NewGuid();
        var student1Id     = Guid.NewGuid();
        var student2Id     = Guid.NewGuid();
        var student3Id     = Guid.NewGuid();
        var parent1Id      = Guid.NewGuid();
        var academicYearId = Guid.NewGuid();
        var course1Id      = Guid.NewGuid();
        var course2Id      = Guid.NewGuid();
        var room1Id        = Guid.NewGuid();
        var room2Id        = Guid.NewGuid();
        var now            = DateTime.UtcNow;

        // ── School ───────────────────────────────────────────────────────────
        db.Schools.Add(new SchoolEntity
        {
            SchoolId  = schoolId,
            Name      = "Greenwood Academy",
            Street    = "123 Main St",
            City      = "Springfield",
            State     = "IL",
            ZipCode   = "62701",
            Country   = "USA",
            Email     = "admin@greenwood.edu",
            Phone     = "555-0100",
            IsActive  = true,
            CreatedAt = now
        });

        // ── Users ────────────────────────────────────────────────────────────
        // Admin — B2CObjectId left as placeholder; update with your real B2C object ID
        db.Users.AddRange(
            new UserEntity
            {
                UserId      = adminUserId,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000001",
                FirstName   = "Admin",
                LastName    = "User",
                Email       = "admin@greenwood.edu",
                Phone       = "555-0101",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = teacher1Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000002",
                FirstName   = "Alice",
                LastName    = "Johnson",
                Email       = "alice.johnson@greenwood.edu",
                Phone       = "555-0102",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = teacher2Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000003",
                FirstName   = "Bob",
                LastName    = "Williams",
                Email       = "bob.williams@greenwood.edu",
                Phone       = "555-0103",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = student1Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000004",
                FirstName   = "Charlie",
                LastName    = "Brown",
                Email       = "charlie.brown@greenwood.edu",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = student2Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000005",
                FirstName   = "Diana",
                LastName    = "Prince",
                Email       = "diana.prince@greenwood.edu",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = student3Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000006",
                FirstName   = "Edward",
                LastName    = "Norton",
                Email       = "edward.norton@greenwood.edu",
                IsActive    = true,
                CreatedAt   = now
            },
            new UserEntity
            {
                UserId      = parent1Id,
                SchoolId    = schoolId,
                B2CObjectId = "00000000-0000-0000-0000-000000000007",
                FirstName   = "Frank",
                LastName    = "Brown",
                Email       = "frank.brown@example.com",
                Phone       = "555-0104",
                IsActive    = true,
                CreatedAt   = now
            }
        );

        // ── Roles ────────────────────────────────────────────────────────────
        // UserRole enum: Admin=1, Teacher=4, Student=8, Parent=7
        db.UserRoles.AddRange(
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = adminUserId,  SchoolId = schoolId, Role = 1 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = teacher1Id,   SchoolId = schoolId, Role = 4 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = teacher2Id,   SchoolId = schoolId, Role = 4 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student1Id,   SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student2Id,   SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student3Id,   SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = parent1Id,    SchoolId = schoolId, Role = 7 }
        );

        // ── Teacher profiles ─────────────────────────────────────────────────
        db.TeacherProfiles.AddRange(
            new TeacherProfileEntity
            {
                TeacherProfileId = Guid.NewGuid(),
                UserId           = teacher1Id,
                SchoolId         = schoolId,
                Degree           = "M.Sc. Mathematics",
                Specialization   = "Algebra",
                HireDate         = new DateTime(2020, 8, 1),
                IsActive         = true
            },
            new TeacherProfileEntity
            {
                TeacherProfileId = Guid.NewGuid(),
                UserId           = teacher2Id,
                SchoolId         = schoolId,
                Degree           = "B.Sc. Computer Science",
                Specialization   = "Programming",
                HireDate         = new DateTime(2021, 8, 1),
                IsActive         = true
            }
        );

        // ── Student profiles ─────────────────────────────────────────────────
        db.StudentProfiles.AddRange(
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student1Id, SchoolId = schoolId, BirthDate = new DateTime(2010, 3, 15), GradeLevel = 9,  AdmissionDate = new DateTime(2024, 9, 1), IsActive = true },
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student2Id, SchoolId = schoolId, BirthDate = new DateTime(2010, 7, 22), GradeLevel = 9,  AdmissionDate = new DateTime(2024, 9, 1), IsActive = true },
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student3Id, SchoolId = schoolId, BirthDate = new DateTime(2009, 11, 5), GradeLevel = 10, AdmissionDate = new DateTime(2023, 9, 1), IsActive = true }
        );

        // ── Parent profile ───────────────────────────────────────────────────
        db.ParentProfiles.Add(new ParentProfileEntity
        {
            ParentProfileId = Guid.NewGuid(),
            UserId          = parent1Id,
            SchoolId        = schoolId,
            Relationship    = 0
        });

        // ── Academic year ────────────────────────────────────────────────────
        db.AcademicYears.Add(new AcademicYearEntity
        {
            AcademicYearId = academicYearId,
            SchoolId       = schoolId,
            Name           = "2024-2025",
            StartDate      = new DateTime(2024, 9, 1),
            EndDate        = new DateTime(2025, 6, 30),
            IsActive       = true
        });

        // ── Rooms ────────────────────────────────────────────────────────────
        db.Rooms.AddRange(
            new RoomEntity { RoomId = room1Id, SchoolId = schoolId, Name = "Room 101", Building = "Main",  Capacity = 30, RoomType = 0, IsActive = true },
            new RoomEntity { RoomId = room2Id, SchoolId = schoolId, Name = "Lab 201",  Building = "Annex", Capacity = 25, RoomType = 1, IsActive = true }
        );

        // ── Courses ──────────────────────────────────────────────────────────
        db.Courses.AddRange(
            new CourseEntity
            {
                CourseId       = course1Id,
                SchoolId       = schoolId,
                AcademicYearId = academicYearId,
                Name           = "Algebra I",
                Description    = "Foundations of algebra",
                GradeLevel     = 9,
                Credits        = 3,
                MaxStudents    = 30,
                TeacherUserId  = teacher1Id,
                Status         = "Open",
                StartDate      = new DateTime(2024, 9, 1),
                EndDate        = new DateTime(2025, 1, 31),
                CreatedAt      = now,
                CreatedBy      = adminUserId
            },
            new CourseEntity
            {
                CourseId       = course2Id,
                SchoolId       = schoolId,
                AcademicYearId = academicYearId,
                Name           = "Introduction to Programming",
                Description    = "Python basics",
                GradeLevel     = 9,
                Credits        = 3,
                MaxStudents    = 25,
                TeacherUserId  = teacher2Id,
                Status         = "Open",
                StartDate      = new DateTime(2024, 9, 1),
                EndDate        = new DateTime(2025, 1, 31),
                CreatedAt      = now,
                CreatedBy      = adminUserId
            }
        );

        // ── Enrollments ──────────────────────────────────────────────────────
        db.Enrollments.AddRange(
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course1Id, StudentUserId = student1Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course1Id, StudentUserId = student2Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course2Id, StudentUserId = student1Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course2Id, StudentUserId = student3Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId }
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Seed complete. School: {Name} ({Id})", "Greenwood Academy", schoolId);
    }
}
