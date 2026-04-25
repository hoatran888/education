using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.Repositories;

namespace SchoolSystem.Infrastructure.Data.UnitOfWork;

public class LinqUnitOfWork : IUnitOfWork
{
    private readonly SchoolDataContext _context;
    private readonly Guid              _schoolId;

    private ISchoolRepository?      _schools;
    private IUserRepository?        _users;
    private ITeacherRepository?     _teachers;
    private IStudentRepository?     _students;
    private ICourseRepository?      _courses;
    private IEnrollmentRepository?  _enrollments;
    private IGradeRepository?       _grades;
    private IScheduleRepository?    _schedules;
    private IInvoiceRepository?     _invoices;
    private IPaymentRepository?     _payments;
    private IGradePeriodRepository? _gradePeriods;

    public LinqUnitOfWork(SchoolDataContext context, Guid schoolId)
    {
        _context  = context;
        _schoolId = schoolId;
    }

    public ISchoolRepository     Schools      => _schools     ??= new LinqSchoolRepository(_context);
    public IUserRepository       Users        => _users       ??= new LinqUserRepository(_context, _schoolId);
    public ITeacherRepository    Teachers     => _teachers    ??= new LinqTeacherRepository(_context, _schoolId);
    public IStudentRepository    Students     => _students    ??= new LinqStudentRepository(_context, _schoolId);
    public ICourseRepository     Courses      => _courses     ??= new LinqCourseRepository(_context, _schoolId);
    public IEnrollmentRepository Enrollments  => _enrollments ??= new LinqEnrollmentRepository(_context, _schoolId);
    public IGradeRepository      Grades       => _grades      ??= new LinqGradeRepository(_context, _schoolId);
    public IScheduleRepository   Schedules    => _schedules   ??= new LinqScheduleRepository(_context, _schoolId);
    public IInvoiceRepository    Invoices     => _invoices    ??= new LinqInvoiceRepository(_context, _schoolId);
    public IPaymentRepository    Payments     => _payments    ??= new LinqPaymentRepository(_context, _schoolId);
    public IGradePeriodRepository GradePeriods => _gradePeriods ??= new LinqGradePeriodRepository(_context, _schoolId);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
