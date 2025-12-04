using CareSync.ApplicationLayer.Repository;
using CareSync.DataLayer;
using CareSync.DataLayer.Entities;
using CareSync.DataLayer.Entities.Lab_Entities;

namespace CareSync.ApplicationLayer.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CareSyncDbContext _dbContext;

    public IRepository<T_Users> UserRepo { get; }
    public IRepository<T_PatientDetails> PatientDetailsRepo { get; }
    public IRepository<T_DoctorDetails> DoctorDetailsRepo { get; }
    public IRepository<T_Lab> LabRepo { get; }
    public IRepository<T_UserLabAssistant> UserLabAssistantRepo { get; }
    public IRepository<T_LabServices> LabServicesRepo { get; }
    public IRepository<T_Appointments> AppointmentsRepo { get; }
    public IRepository<T_PatientVitals> PatientVitalsRepo { get; }
    public IRepository<T_PatientReports> PatientReportsRepo { get; }
    public IRepository<T_Prescriptions> PrescriptionsRepo { get; }
    public IRepository<T_LabReports> LabReportsRepo { get; }

    public UnitOfWork(
        CareSyncDbContext dbContext,
        IRepository<T_Users> userRepo,
        IRepository<T_PatientDetails> patientRepo,
        IRepository<T_DoctorDetails> doctorRepo,
        IRepository<T_Lab> labRepo,
        IRepository<T_UserLabAssistant> userLabAssistantRepo,
        IRepository<T_LabServices> labServicesRepo,
        IRepository<T_Appointments> appointmentRepo,
        IRepository<T_PatientVitals> patientVitalsRepo,
        IRepository<T_PatientReports> patientReportsRepo,
        IRepository<T_Prescriptions> prescriptionsRepo,
        IRepository<T_LabReports> labReportsRepo)
    {
        _dbContext = dbContext;
        UserRepo = userRepo;
        PatientDetailsRepo = patientRepo;
        DoctorDetailsRepo = doctorRepo;
        LabRepo = labRepo;
        UserLabAssistantRepo = userLabAssistantRepo;
        LabServicesRepo = labServicesRepo;
        AppointmentsRepo = appointmentRepo;
        PatientVitalsRepo = patientVitalsRepo;
        PatientReportsRepo = patientReportsRepo;
        PrescriptionsRepo = prescriptionsRepo;
        LabReportsRepo = labReportsRepo;
    }

    public async Task BeginTransactionAsync() =>
        await _dbContext.Database.BeginTransactionAsync();

    public async Task CommitAsync() =>
        await _dbContext.Database.CommitTransactionAsync();

    public async Task RollbackAsync() =>
        await _dbContext.Database.RollbackTransactionAsync();

    public async Task<int> SaveChangesAsync() =>
        await _dbContext.SaveChangesAsync();

    public void Dispose()
    {
        _dbContext?.Dispose();
        GC.SuppressFinalize(this);
    }
}