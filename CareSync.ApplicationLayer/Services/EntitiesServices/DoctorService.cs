using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums.Appointment;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public partial class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IUnitOfWork uow,
        IMapper mapper,
        ILogger<DoctorService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<DoctorDashboard_DTO>> GetDoctorDashboardAsync(string userId)
    {
        try
        {
            // Find doctor record by user id
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == userId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                _logger.LogWarning("Doctor not found for user id {UserId}", userId);
                return Result<DoctorDashboard_DTO>.Failure(new DoctorDashboard_DTO(), "Doctor profile not found.");
            }

            var doctorId = doctor.DoctorID;

            // Appointments for this doctor
            var allAppointments = (await _uow.AppointmentsRepo.GetAllAsync(a => a.DoctorID == doctorId && !a.IsDeleted))?.ToList()
                                  ?? new List<T_Appointments>();

            // Statistics
            var totalPatients = allAppointments.Select(a => a.PatientID).Distinct().Count();
            var today = DateTime.UtcNow.Date;
            var todaysAppointments = allAppointments
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentDate)
                .ToList();
            var todayCount = todaysAppointments.Count;

            var pendingAppointments = allAppointments.Count(a =>
                a.Status == AppointmentStatus_Enum.Created ||
                a.Status == AppointmentStatus_Enum.Pending);

            var prescriptions = (await _uow.PrescriptionsRepo.GetAllAsync(p => p.DoctorID == doctorId && !p.IsDeleted)) ?? new List<T_Prescriptions>();
            var prescriptionsCount = prescriptions.Count();

            var labReports = (await _uow.LabReportsRepo.GetAllAsync(l => l.ReviewedByDoctorID == doctorId && !l.IsDeleted)) ?? new List<T_LabReports>();
            var labReportsCount = labReports.Count();

            var byPatient = allAppointments
                .GroupBy(a => a.PatientID)
                .Select(g => new { PatientID = g.Key, First = g.Min(x => x.AppointmentDate) })
                .ToList();

            var cutoff = DateTime.UtcNow.Date.AddDays(-30);
            var newPatients = byPatient.Count(p => p.First.Date >= cutoff);
            var followUpPatients = allAppointments.Count(a => a.AppointmentType == AppointmentType_Enum.FollowUp);
            var regularPatients = Math.Max(0, totalPatients - newPatients);

            var recentAppts = allAppointments
                .OrderByDescending(a => a.AppointmentDate)
                .Take(10)
                .GroupBy(a => a.PatientID)
                .Select(g => g.OrderByDescending(x => x.AppointmentDate).First())
                .Take(5)
                .ToList();

            var todayPatientIds = todaysAppointments.Select(a => a.PatientID).Distinct();
            var recentPatientIds = recentAppts.Select(a => a.PatientID).Distinct();
            var lookupPatientIds = todayPatientIds.Union(recentPatientIds).Distinct().ToList();

            var patientDetails = (lookupPatientIds.Any()
                ? (await _uow.PatientDetailsRepo.GetAllAsync(p => lookupPatientIds.Contains(p.PatientID) && !p.IsDeleted))?.ToList()
                : new List<T_PatientDetails>()) ?? new List<T_PatientDetails>();

            var userIds = patientDetails.Select(p => p.UserID).Where(u => !string.IsNullOrEmpty(u)).Distinct().ToList();
            var users = (userIds.Any()
                ? (await _uow.UserRepo.GetAllAsync(u => userIds.Contains(u.Id) && !u.IsDeleted))?.ToList()
                : new List<T_Users>()) ?? new List<T_Users>();

            var patientById = patientDetails.ToDictionary(p => p.PatientID, p => p);
            var userById = users.ToDictionary(u => u.Id, u => u);

            var todayDtos = new List<TodayAppointment_DTO>();
            foreach (var a in todaysAppointments)
            {
                string patientName = $"P{a.PatientID}";
                int age = 0;
                if (patientById.TryGetValue(a.PatientID, out var pd))
                {
                    if (!string.IsNullOrEmpty(pd.UserID) && userById.TryGetValue(pd.UserID, out var u))
                    {
                        patientName = $"{u.FirstName} {u.LastName}".Trim();
                        if (u.DateOfBirth.HasValue)
                        {
                            age = CalculateAge(u.DateOfBirth.Value, DateTime.UtcNow.Date);
                        }
                    }
                }

                todayDtos.Add(new TodayAppointment_DTO
                {
                    Id = a.AppointmentID,
                    PatientName = patientName,
                    PatientAge = age,
                    AppointmentTime = a.AppointmentDate,
                    Diagnosis = a.Reason ?? string.Empty,
                    Type = a.AppointmentType.ToString()
                });
            }

            var recentPatients = new List<RecentPatient_DTO>();
            foreach (var a in recentAppts)
            {
                string name = $"P{a.PatientID}";
                if (patientById.TryGetValue(a.PatientID, out var pd))
                {
                    if (!string.IsNullOrEmpty(pd.UserID) && userById.TryGetValue(pd.UserID, out var u))
                    {
                        name = $"{u.FirstName} {u.LastName}".Trim();
                    }
                }

                recentPatients.Add(new RecentPatient_DTO
                {
                    Id = a.PatientID,
                    Name = name,
                    LastAppointment = a.AppointmentDate,
                    Notes = a.Notes ?? string.Empty
                });
            }

            // Build last 12 months overview for appointments (chronological)
            var overviewData = new List<int>();
            var overviewLabels = new List<string>();
            var now = DateTime.UtcNow;
            for (int m = 11; m >= 0; m--)
            {
                var month = new DateTime(now.Year, now.Month, 1).AddMonths(-m);
                var monthStart = new DateTime(month.Year, month.Month, 1);
                var monthEnd = monthStart.AddMonths(1);
                var countForMonth = allAppointments.Count(a => a.AppointmentDate >= monthStart && a.AppointmentDate < monthEnd);
                overviewData.Add(countForMonth);
                overviewLabels.Add(month.ToString("MMM", CultureInfo.InvariantCulture));
            }

            var dto = new DoctorDashboard_DTO
            {
                DoctorName = $"{doctor?.User?.FirstName} {doctor?.User?.LastName}".Trim(),
                Specialization = doctor?.Specialization ?? string.Empty,
                //TotalRatings = doctor.Ratings ?? 0,
                TotalPatients = totalPatients,
                //TotalSurgeries = doctor.TotalSurgeries ?? 0,
                //MonthlyEarnings = doctor.MonthlyEarnings ?? 0,
                TodayAppointmentsCount = todayCount,
                TotalPrescriptions = prescriptionsCount,
                PendingAppointments = pendingAppointments,
                LabReports = labReportsCount,
                NewPatients = newPatients,
                RegularPatients = regularPatients,
                FollowUpPatients = followUpPatients,
                TodayAppointments = todayDtos,
                RecentPatients = recentPatients,
                AppointmentsOverviewData = overviewData,
                AppointmentsOverviewLabels = overviewLabels
            };

            return Result<DoctorDashboard_DTO>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building doctor dashboard for user {UserId}", userId);
            return Result<DoctorDashboard_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Update appointment status for an appointment owned by the doctor identified by doctorUserId.
    /// Validates ownership and persists status change.
    /// </summary>
    public async Task<Result<GeneralResponse>> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus_Enum newStatus, string doctorUserId)
    {
        _logger.LogInformation("Attempting to update appointment {AppointmentId} to {Status} by doctor user {UserId}", appointmentId, newStatus, doctorUserId);

        try
        {
            // Find doctor record
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == doctorUserId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
            {
                _logger.LogWarning("Doctor profile not found for user {UserId}", doctorUserId);
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Doctor profile not found." }, "Doctor profile not found");
            }

            // Find appointment
            var appointment = await _uow.AppointmentsRepo.GetByIdAsync(appointmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found", appointmentId);
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Appointment not found." }, "Appointment not found");
            }

            // Ensure doctor owns this appointment
            if (appointment.DoctorID != doctor.DoctorID)
            {
                _logger.LogWarning("Doctor {DoctorId} attempted to update appointment {AppointmentId} they don't own", doctor.DoctorID, appointmentId);
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthorized to modify this appointment." }, "Unauthorized");
            }

            // Update status and audit fields
            appointment.Status = newStatus;
            appointment.UpdatedOn = DateTime.UtcNow;

            // Persist - repository pattern relies on EF change tracking; SaveChangesAsync on unit of work
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Appointment {AppointmentId} status updated to {Status}", appointmentId, newStatus);
            return Result<GeneralResponse>.Success(new GeneralResponse { Success = true, Message = $"Appointment status updated to {newStatus}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {AppointmentId} status", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    private static int CalculateAge(DateTime dob, DateTime asOf)
    {
        var age = asOf.Year - dob.Year;
        if (asOf < dob.AddYears(age)) age--;
        return Math.Max(0, age);
    }

    public async Task<Result<AppointmentDetails_DTO>> GetAppointmentDetailsAsync(int appointmentId, string doctorUserId)
    {
        try
        {
            // Find doctor record for current user
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == doctorUserId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
            {
                _logger.LogWarning("Doctor profile not found for user {UserId}", doctorUserId);
                return Result<AppointmentDetails_DTO>.Failure(new AppointmentDetails_DTO(), "Doctor profile not found.");
            }

            // Load appointment
            var appointment = await _uow.AppointmentsRepo.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.IsDeleted)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found", appointmentId);
                return Result<AppointmentDetails_DTO>.Failure(new AppointmentDetails_DTO(), "Appointment not found.");
            }

            // Verify ownership
            if (appointment.DoctorID != doctor.DoctorID)
            {
                _logger.LogWarning("Doctor {DoctorId} attempted to access appointment {AppointmentId} they don't own", doctor.DoctorID, appointmentId);
                return Result<AppointmentDetails_DTO>.Failure(new AppointmentDetails_DTO(), "Unauthorized to view this appointment.");
            }

            // Patient details
            var patientDetails = await _uow.PatientDetailsRepo.GetByIdAsync(appointment.PatientID);
            string patientName = $"P{appointment.PatientID}";
            int patientAge = 0;
            string? patientContact = null;
            string? patientEmail = null;

            if (patientDetails != null && !string.IsNullOrEmpty(patientDetails.UserID))
            {
                var user = await _uow.UserRepo.GetByIdAsync(patientDetails.UserID);
                if (user != null)
                {
                    patientName = $"{user.FirstName} {user.LastName}".Trim();
                    patientContact = user.PhoneNumber;
                    patientEmail = user.Email;
                    if (user.DateOfBirth.HasValue)
                        patientAge = CalculateAge(user.DateOfBirth.Value, DateTime.UtcNow.Date);
                }
            }

            // Prescriptions linked to appointment
            var prescriptions = await _uow.PrescriptionsRepo.GetAllAsync(p => p.AppointmentID == appointment.AppointmentID && !p.IsDeleted) ?? new List<T_Prescriptions>();
            var prescriptionItems = prescriptions.Where(c => c.Appointment != null)
                .Select(p => p.PrescriptionItems)
                .SelectMany(pi => pi)
                .Select(i => i.MedicineName ?? string.Empty)
                .ToList();

            // Lab reports linked to appointment
            var labReports = (await _uow.LabReportsRepo.GetAllAsync(l => l.AppointmentID == appointment.AppointmentID && !l.IsDeleted)) ?? new List<T_LabReports>();
            var labReportList = labReports
                .Select(l => l.ReportName)
                .ToList();

            // Build DTO
            var dto = new AppointmentDetails_DTO
            {
                AppointmentID = appointment.AppointmentID,
                //AppointmentNumber = appointment.AppointmentID,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentType = appointment.AppointmentType.ToString(),
                Status = appointment.Status.ToString(),
                Reason = appointment.Reason ?? string.Empty,

                DoctorID = appointment.DoctorID,
                DoctorName = $"{doctor.User?.FirstName} {doctor.User?.LastName}".Trim(),
                DoctorSpecialization = doctor.Specialization ?? string.Empty,

                PatientID = appointment.PatientID,
                PatientName = patientName,
                PatientAge = patientAge,
                PatientContact = patientContact,
                PatientEmail = patientEmail,

                Diagnosis = appointment.Reason,
                TreatmentPlan = appointment.Notes,
                DoctorNotes = appointment.Notes,

                BloodPressure = null,
                HeartRate = null,
                Temperature = null,

                Prescriptions = prescriptionItems,
                LabReports = labReportList
            };

            return Result<AppointmentDetails_DTO>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment details for {AppointmentId}", appointmentId);
            return Result<AppointmentDetails_DTO>.Exception(ex);
        }
    }
}