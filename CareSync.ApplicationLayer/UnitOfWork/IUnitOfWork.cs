using CareSync.ApplicationLayer.Repository;
using CareSync.DataLayer;
using CareSync.DataLayer.Entities;
using CareSync.DataLayer.Entities.Lab_Entities;

namespace CareSync.ApplicationLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T_Users> UserRepo { get; }
    IRepository<T_PatientDetails> PatientDetailsRepo { get; }
    IRepository<T_DoctorDetails> DoctorDetailsRepo { get; }
    IRepository<T_Lab> LabRepo { get; }
    IRepository<T_UserLabAssistant> UserLabAssistantRepo { get; }
    IRepository<T_LabServices> LabServicesRepo { get; }
    IRepository<T_Appointments> AppointmentsRepo { get; }
    IRepository<T_PatientVitals> PatientVitalsRepo { get; }
    IRepository<T_PatientReports> PatientReportsRepo { get; }
    IRepository<T_Prescriptions> PrescriptionsRepo { get; }
    IRepository<T_LabReports> LabReportsRepo { get; }

    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}