using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.LinqEntities;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Infrastructure.Data.Seed;

public static class DatabaseSeeder
{
    // Fixed GUIDs so dev auto-login always works with the same user/school
    public static readonly Guid DevSchoolId    = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    public static readonly Guid DevAdminUserId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var env         = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var db          = scope.ServiceProvider.GetRequiredService<SchoolDataContext>();
        var hasher      = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger      = scope.ServiceProvider.GetRequiredService<ILogger<SchoolDataContext>>();

        if (env.IsDevelopment())
        {
            // SQLite: EnsureCreated is simpler and avoids provider type conflicts.
            // Migrations are SQL Server-typed and won't run cleanly on SQLite.
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            // SQL Server (production): apply pending migrations on every deploy
            await db.Database.MigrateAsync();
            return; // no dev seed data in production
        }

        if (await db.Schools.AnyAsync()) return; // already seeded

        logger.LogInformation("Seeding development data…");

        var schoolId       = DevSchoolId;
        var adminUserId    = DevAdminUserId;
        var teacher1Id     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
        var teacher2Id     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
        var student1Id     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005");
        var student2Id     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006");
        var student3Id     = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000007");
        var parent1Id      = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000008");
        var academicYearId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000009");
        var course1Id      = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000010");
        var course2Id      = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000011");
        var room1Id        = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000012");
        var room2Id        = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000013");
        var now            = DateTime.UtcNow;
        var defaultPwHash  = hasher.Hash("School123!");

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

        db.Users.AddRange(
            new UserEntity { UserId = adminUserId, SchoolId = schoolId, B2CObjectId = "dev-admin", FirstName = "Admin",   LastName = "User",    Email = "admin@greenwood.edu",   Phone = "555-0101", Street = "123 Main St",    City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = teacher1Id,  SchoolId = schoolId, B2CObjectId = "dev-t1",    FirstName = "Alice",   LastName = "Johnson", Email = "alice@greenwood.edu",   Phone = "555-0102", Street = "456 Oak Ave",    City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = teacher2Id,  SchoolId = schoolId, B2CObjectId = "dev-t2",    FirstName = "Bob",     LastName = "Williams",Email = "bob@greenwood.edu",     Phone = "555-0103", Street = "789 Pine Rd",    City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = student1Id,  SchoolId = schoolId, B2CObjectId = "dev-s1",    FirstName = "Charlie", LastName = "Brown",   Email = "charlie@greenwood.edu", Phone = "555-0104", Street = "101 Elm St",     City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = student2Id,  SchoolId = schoolId, B2CObjectId = "dev-s2",    FirstName = "Diana",   LastName = "Prince",  Email = "diana@greenwood.edu",   Phone = "555-0105", Street = "202 Maple Ave",  City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = student3Id,  SchoolId = schoolId, B2CObjectId = "dev-s3",    FirstName = "Edward",  LastName = "Norton",  Email = "edward@greenwood.edu",  Phone = "555-0106", Street = "303 Cedar Blvd", City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash },
            new UserEntity { UserId = parent1Id,   SchoolId = schoolId, B2CObjectId = "dev-p1",    FirstName = "Frank",   LastName = "Brown",   Email = "frank@example.com",     Phone = "555-0107", Street = "101 Elm St",     City = "Springfield", State = "IL", ZipCode = "62701", Country = "USA", IsActive = true, CreatedAt = now, PasswordHash = defaultPwHash }
        );

        db.UserRoles.AddRange(
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = adminUserId, SchoolId = schoolId, Role = 1 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = teacher1Id,  SchoolId = schoolId, Role = 4 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = teacher2Id,  SchoolId = schoolId, Role = 4 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student1Id,  SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student2Id,  SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = student3Id,  SchoolId = schoolId, Role = 8 },
            new UserRoleEntity { UserRoleId = Guid.NewGuid(), UserId = parent1Id,   SchoolId = schoolId, Role = 7 }
        );

        db.TeacherProfiles.AddRange(
            new TeacherProfileEntity { TeacherProfileId = Guid.NewGuid(), UserId = teacher1Id, SchoolId = schoolId, Degree = "M.Sc. Mathematics",     Specialization = "Algebra",      HireDate = new DateTime(2020, 8, 1), IsActive = true },
            new TeacherProfileEntity { TeacherProfileId = Guid.NewGuid(), UserId = teacher2Id, SchoolId = schoolId, Degree = "B.Sc. Computer Science", Specialization = "Programming",  HireDate = new DateTime(2021, 8, 1), IsActive = true }
        );

        db.StudentProfiles.AddRange(
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student1Id, SchoolId = schoolId, BirthDate = new DateTime(2010, 3, 15),  GradeLevel = 9,  AdmissionDate = new DateTime(2024, 9, 1), IsActive = true },
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student2Id, SchoolId = schoolId, BirthDate = new DateTime(2010, 7, 22),  GradeLevel = 9,  AdmissionDate = new DateTime(2024, 9, 1), IsActive = true },
            new StudentProfileEntity { StudentProfileId = Guid.NewGuid(), UserId = student3Id, SchoolId = schoolId, BirthDate = new DateTime(2009, 11, 5),  GradeLevel = 10, AdmissionDate = new DateTime(2023, 9, 1), IsActive = true }
        );

        db.ParentProfiles.Add(new ParentProfileEntity { ParentProfileId = Guid.NewGuid(), UserId = parent1Id, SchoolId = schoolId, Relationship = 0 });

        db.AcademicYears.Add(new AcademicYearEntity
        {
            AcademicYearId = academicYearId,
            SchoolId       = schoolId,
            Name           = "2024-2025",
            StartDate      = new DateTime(2024, 9, 1),
            EndDate        = new DateTime(2025, 6, 30),
            IsActive       = true
        });

        db.Rooms.AddRange(
            new RoomEntity { RoomId = room1Id, SchoolId = schoolId, Name = "Room 101", Building = "Main",  Capacity = 30, RoomType = 0, IsActive = true },
            new RoomEntity { RoomId = room2Id, SchoolId = schoolId, Name = "Lab 201",  Building = "Annex", Capacity = 25, RoomType = 1, IsActive = true }
        );

        db.Courses.AddRange(
            new CourseEntity { CourseId = course1Id, SchoolId = schoolId, AcademicYearId = academicYearId, Name = "Algebra I",                   Description = "Foundations of algebra", GradeLevel = 9,  Credits = 3, MaxStudents = 30, TeacherUserId = teacher1Id, Status = "Open", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 1, 31), CreatedAt = now, CreatedBy = adminUserId },
            new CourseEntity { CourseId = course2Id, SchoolId = schoolId, AcademicYearId = academicYearId, Name = "Intro to Programming", Description = "Python basics",             GradeLevel = 9,  Credits = 3, MaxStudents = 25, TeacherUserId = teacher2Id, Status = "Open", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2025, 1, 31), CreatedAt = now, CreatedBy = adminUserId }
        );

        db.Enrollments.AddRange(
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course1Id, StudentUserId = student1Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course1Id, StudentUserId = student2Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course2Id, StudentUserId = student1Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId },
            new EnrollmentEntity { EnrollmentId = Guid.NewGuid(), CourseId = course2Id, StudentUserId = student3Id, SchoolId = schoolId, EnrolledDate = now, Status = "Active", EnrolledBy = adminUserId }
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Seed complete — School: Greenwood Academy ({Id})", schoolId);
    }
}
