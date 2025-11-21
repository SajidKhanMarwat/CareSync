using CareSync.ApplicationLayer.Repository;
using CareSync.DataLayer;
using CareSync.DataLayer.Entities;

namespace CareSync.ApplicationLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T_Users> UserRepo { get; }
    IRepository<T_PatientDetails> PatientDetailsRepo { get; }
    IRepository<T_DoctorDetails> DoctorDetailsRepo { get; }
    IRepository<T_Lab> LabRepo { get; }
    IRepository<T_Appointments> AppointmentsRepo { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}