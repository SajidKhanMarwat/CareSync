using CareSync.ApplicationLayer.Repository;
using CareSync.DataLayer;
using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareSync.ApplicationLayer.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CareSyncDbContext _dbContext;

    public IRepository<T_Users> UserRepo { get; }
    public IRepository<T_PatientDetails> PatientDetailsRepo { get; }
    public IRepository<T_DoctorDetails> DoctorDetailsRepo { get; }
    public IRepository<T_Lab> LabRepo { get; }
    public IRepository<T_Appointments> AppointmentsRepo { get; }

    public UnitOfWork(
        CareSyncDbContext dbContext,
        IRepository<T_Users> userRepo,
        IRepository<T_PatientDetails> patientRepo,
        IRepository<T_DoctorDetails> doctorRepo,
        IRepository<T_Lab> labRepo,
        IRepository<T_Appointments> appointmentRepo)
    {
        _dbContext = dbContext;
        UserRepo = userRepo;
        PatientDetailsRepo = patientRepo;
        DoctorDetailsRepo = doctorRepo;
        LabRepo = labRepo;
        AppointmentsRepo = appointmentRepo;
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