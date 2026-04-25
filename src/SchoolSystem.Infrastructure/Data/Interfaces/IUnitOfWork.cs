namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ISchoolRepository      Schools     { get; }
    IUserRepository        Users       { get; }
    ITeacherRepository     Teachers    { get; }
    IStudentRepository     Students    { get; }
    ICourseRepository      Courses     { get; }
    IEnrollmentRepository  Enrollments { get; }
    IGradeRepository       Grades      { get; }
    IScheduleRepository    Schedules   { get; }
    IInvoiceRepository     Invoices    { get; }
    IPaymentRepository     Payments      { get; }
    IGradePeriodRepository GradePeriods  { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
