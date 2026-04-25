using Microsoft.EntityFrameworkCore;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Context;

public class SchoolDataContext : DbContext
{
    public SchoolDataContext(DbContextOptions<SchoolDataContext> options) : base(options) { }

    public DbSet<SchoolEntity>          Schools          => Set<SchoolEntity>();
    public DbSet<UserEntity>            Users            => Set<UserEntity>();
    public DbSet<UserRoleEntity>        UserRoles        => Set<UserRoleEntity>();
    public DbSet<TeacherProfileEntity>  TeacherProfiles  => Set<TeacherProfileEntity>();
    public DbSet<StudentProfileEntity>  StudentProfiles  => Set<StudentProfileEntity>();
    public DbSet<ParentProfileEntity>   ParentProfiles   => Set<ParentProfileEntity>();
    public DbSet<StudentParentEntity>   StudentParents   => Set<StudentParentEntity>();
    public DbSet<AcademicYearEntity>    AcademicYears    => Set<AcademicYearEntity>();
    public DbSet<TermEntity>            Terms            => Set<TermEntity>();
    public DbSet<GradePeriodEntity>     GradePeriods     => Set<GradePeriodEntity>();
    public DbSet<CourseEntity>          Courses          => Set<CourseEntity>();
    public DbSet<RoomEntity>            Rooms            => Set<RoomEntity>();
    public DbSet<ScheduleEntity>        Schedules        => Set<ScheduleEntity>();
    public DbSet<EnrollmentEntity>      Enrollments      => Set<EnrollmentEntity>();
    public DbSet<GradeEntity>           Grades           => Set<GradeEntity>();
    public DbSet<FeeStructureEntity>    FeeStructures    => Set<FeeStructureEntity>();
    public DbSet<InvoiceEntity>         Invoices         => Set<InvoiceEntity>();
    public DbSet<InvoiceItemEntity>     InvoiceItems     => Set<InvoiceItemEntity>();
    public DbSet<PaymentEntity>         Payments         => Set<PaymentEntity>();
    public DbSet<HRProfileEntity>       HRProfiles       => Set<HRProfileEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SchoolEntity>(e =>
        {
            e.ToTable("Schools");
            e.HasKey(s => s.SchoolId);
            e.HasMany(s => s.Users).WithOne().HasForeignKey(u => u.SchoolId);
            e.HasMany(s => s.Courses).WithOne().HasForeignKey(c => c.SchoolId);
        });

        modelBuilder.Entity<UserEntity>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.UserId);
            e.HasMany(u => u.UserRoles).WithOne().HasForeignKey(r => r.UserId);
            e.Ignore(u => u.FullName);
        });

        modelBuilder.Entity<UserRoleEntity>(e =>
        {
            e.ToTable("UserRoles");
            e.HasKey(r => r.UserRoleId);
        });

        modelBuilder.Entity<TeacherProfileEntity>(e =>
        {
            e.ToTable("TeacherProfiles");
            e.HasKey(t => t.TeacherProfileId);
        });

        modelBuilder.Entity<StudentProfileEntity>(e =>
        {
            e.ToTable("StudentProfiles");
            e.HasKey(s => s.StudentProfileId);
        });

        modelBuilder.Entity<ParentProfileEntity>(e =>
        {
            e.ToTable("ParentProfiles");
            e.HasKey(p => p.ParentProfileId);
        });

        modelBuilder.Entity<StudentParentEntity>(e =>
        {
            e.ToTable("StudentParents");
            e.HasKey(sp => sp.StudentParentId);
        });

        modelBuilder.Entity<AcademicYearEntity>(e =>
        {
            e.ToTable("AcademicYears");
            e.HasKey(a => a.AcademicYearId);
        });

        modelBuilder.Entity<TermEntity>(e =>
        {
            e.ToTable("Terms");
            e.HasKey(t => t.TermId);
        });

        modelBuilder.Entity<GradePeriodEntity>(e =>
        {
            e.ToTable("GradePeriods");
            e.HasKey(gp => gp.GradePeriodId);
        });

        modelBuilder.Entity<CourseEntity>(e =>
        {
            e.ToTable("Courses");
            e.HasKey(c => c.CourseId);
            e.HasMany(c => c.Enrollments).WithOne().HasForeignKey(en => en.CourseId);
        });

        modelBuilder.Entity<RoomEntity>(e =>
        {
            e.ToTable("Rooms");
            e.HasKey(r => r.RoomId);
        });

        modelBuilder.Entity<ScheduleEntity>(e =>
        {
            e.ToTable("Schedules");
            e.HasKey(s => s.ScheduleId);
        });

        modelBuilder.Entity<EnrollmentEntity>(e =>
        {
            e.ToTable("Enrollments");
            e.HasKey(en => en.EnrollmentId);
        });

        modelBuilder.Entity<GradeEntity>(e =>
        {
            e.ToTable("Grades");
            e.HasKey(g => g.GradeId);
        });

        modelBuilder.Entity<FeeStructureEntity>(e =>
        {
            e.ToTable("FeeStructures");
            e.HasKey(f => f.FeeStructureId);
        });

        modelBuilder.Entity<InvoiceEntity>(e =>
        {
            e.ToTable("Invoices");
            e.HasKey(i => i.InvoiceId);
            e.HasMany(i => i.Items).WithOne().HasForeignKey(ii => ii.InvoiceId);
        });

        modelBuilder.Entity<InvoiceItemEntity>(e =>
        {
            e.ToTable("InvoiceItems");
            e.HasKey(ii => ii.InvoiceItemId);
        });

        modelBuilder.Entity<PaymentEntity>(e =>
        {
            e.ToTable("Payments");
            e.HasKey(p => p.PaymentId);
        });

        modelBuilder.Entity<HRProfileEntity>(e =>
        {
            e.ToTable("HRProfiles");
            e.HasKey(h => h.HRProfileId);
        });
    }
}
