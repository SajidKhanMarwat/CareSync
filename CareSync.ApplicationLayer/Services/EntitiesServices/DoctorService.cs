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

            // Appointments that are waiting for prescriptions
            var pendingPrescriptionsCount = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.PrescriptionPending);

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
                    DoctorID = doctorId,
                    PatientID = a.PatientID,
                    PatientName = patientName,
                    PatientAge = age,
                    AppointmentTime = a.AppointmentDate,
                    Diagnosis = a.Reason ?? string.Empty,
                    Status = a.Status,
                    AppointmentType = a.AppointmentType
                });
            }

            var previousAppointmentDtos = allAppointments
                .Where(a => a.AppointmentDate.Date < today)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new PreviousAppointment_DTO
                {
                    AppointmentID = a.AppointmentID,
                    PatientID = a.PatientID,
                    PatientName = patientById.TryGetValue(a.PatientID, out var pd)
                        && !string.IsNullOrEmpty(pd.UserID)
                        && userById.TryGetValue(pd.UserID, out var u)
                        ? $"{u.FirstName} {u.LastName}".Trim()
                        : $"P{a.PatientID}",
                    DoctorID = doctorId,
                    DoctorName = $"{doctor?.User?.FirstName} {doctor?.User?.LastName}".Trim(),
                    DoctorSpecialization = doctor?.Specialization,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentDate,
                    Status = a.Status,
                    AppointmentType = a.AppointmentType,
                    Reason = a.Reason
                })
                .ToList();

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
                PendingPrescriptionsCount = pendingPrescriptionsCount,
                PendingAppointments = pendingAppointments,
                LabReports = labReportsCount,
                NewPatients = newPatients,
                RegularPatients = regularPatients,
                FollowUpPatients = followUpPatients,
                TodayAppointments = todayDtos,
                PreviousAppointments = previousAppointmentDtos,
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

    public async Task<Result<List<DoctorLabReport_DTO>>> GetDoctorLabReportsAsync(string userId)
    {
        try
        {
            // Resolve doctor
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == userId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
            {
                _logger.LogWarning("Doctor not found for user id {UserId}", userId);
                return Result<List<DoctorLabReport_DTO>>.Failure(new List<DoctorLabReport_DTO>(), "Doctor profile not found.");
            }

            var doctorId = doctor.DoctorID;

            var labReports = (await _uow.LabReportsRepo.GetAllAsync(l => l.ReviewedByDoctorID == doctorId && !l.IsDeleted))
                             ?.ToList() ?? new List<T_LabReports>();

            if (!labReports.Any())
            {
                return Result<List<DoctorLabReport_DTO>>.Success(new List<DoctorLabReport_DTO>());
            }

            // Load appointments and patients for names
            var appointmentIds = labReports.Select(l => l.AppointmentID).Distinct().ToList();
            var appointments = (await _uow.AppointmentsRepo.GetAllAsync(a => appointmentIds.Contains(a.AppointmentID) && !a.IsDeleted))
                               ?.ToList() ?? new List<T_Appointments>();

            var patientIds = appointments.Select(a => a.PatientID).Distinct().ToList();
            var patientDetails = (await _uow.PatientDetailsRepo.GetAllAsync(p => patientIds.Contains(p.PatientID) && !p.IsDeleted))
                                 ?.ToList() ?? new List<T_PatientDetails>();

            var userIds = patientDetails.Select(p => p.UserID).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
            var users = (await _uow.UserRepo.GetAllAsync(u => userIds.Contains(u.Id) && !u.IsDeleted))
                        ?.ToList() ?? new List<T_Users>();

            var patientById = patientDetails.ToDictionary(p => p.PatientID, p => p);
            var userById = users.ToDictionary(u => u.Id, u => u);

            string ResolvePatientName(int patientId)
            {
                if (patientById.TryGetValue(patientId, out var pd) && !string.IsNullOrEmpty(pd.UserID) && userById.TryGetValue(pd.UserID, out var u))
                {
                    return ($"{u.FirstName} {u.LastName}").Trim();
                }
                return $"P{patientId}";
            }

            var dtoList = new List<DoctorLabReport_DTO>();

            foreach (var report in labReports.OrderByDescending(l => l.ReviewedDate ?? l.CreatedOn))
            {
                var appointment = appointments.FirstOrDefault(a => a.AppointmentID == report.AppointmentID);
                var patientName = appointment != null ? ResolvePatientName(appointment.PatientID) : string.Empty;

                dtoList.Add(new DoctorLabReport_DTO
                {
                    LabReportID = report.LabReportID,
                    AppointmentID = report.AppointmentID,
                    ReviewedByDoctorID = report.ReviewedByDoctorID,
                    ReviewedDate = report.ReviewedDate,
                    PatientName = patientName,
                    ReportName = report.ReportName,
                    ResultSummary = report.ResultSummary
                });
            }

            return Result<List<DoctorLabReport_DTO>>.Success(dtoList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor lab reports for user {UserId}", userId);
            return Result<List<DoctorLabReport_DTO>>.Exception(ex);
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
                AppointmentType = appointment.AppointmentType,
                Status = appointment.Status,
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

    public async Task<Result<DoctorCheckup_DTO>> GetCheckupAsync(int appointmentId, string doctorUserId)
    {
        try
        {
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == doctorUserId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();
            if (doctor == null)
            {
                _logger.LogWarning("Doctor profile not found for user {UserId}", doctorUserId);
                return Result<DoctorCheckup_DTO>.Failure(new DoctorCheckup_DTO(), "Doctor profile not found.");
            }

            var appointment = await _uow.AppointmentsRepo.GetByIdAsync(appointmentId);
            if (appointment == null || appointment.IsDeleted)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found", appointmentId);
                return Result<DoctorCheckup_DTO>.Failure(new DoctorCheckup_DTO(), "Appointment not found.");
            }

            if (appointment.DoctorID != doctor.DoctorID)
            {
                _logger.LogWarning("Doctor {DoctorId} attempted to access checkup for appointment {AppointmentId} they don't own", doctor.DoctorID, appointmentId);
                return Result<DoctorCheckup_DTO>.Failure(new DoctorCheckup_DTO(), "Unauthorized to view this appointment.");
            }

            var patientDetails = await _uow.PatientDetailsRepo.GetByIdAsync(appointment.PatientID);
            if (patientDetails == null || patientDetails.IsDeleted)
            {
                _logger.LogWarning("Patient details not found for patient {PatientId}", appointment.PatientID);
                return Result<DoctorCheckup_DTO>.Failure(new DoctorCheckup_DTO(), "Patient details not found.");
            }

            var dto = new DoctorCheckup_DTO
            {
                AppointmentId = appointment.AppointmentID,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentType = appointment.AppointmentType,
                Reason = appointment.Reason ?? string.Empty,

                PatientId = patientDetails.PatientID,
                BloodGroup = patientDetails.BloodGroup,
                MaritalStatus = patientDetails.MaritalStatus.ToString(),
                EmergencyContactName = patientDetails.EmergencyContactName ?? string.Empty,
                EmergencyContactNumber = patientDetails.EmergencyContactNumber ?? string.Empty,
                PatientName = $"P{patientDetails.PatientID}",
                PatientAge = 0
            };

            if (!string.IsNullOrEmpty(patientDetails.UserID))
            {
                var user = await _uow.UserRepo.GetByIdAsync(patientDetails.UserID);
                if (user != null && !user.IsDeleted)
                {
                    dto.PatientName = $"{user.FirstName} {user.LastName}".Trim();

                    if (user.DateOfBirth.HasValue)
                    {
                        dto.PatientAge = CalculateAge(user.DateOfBirth.Value, DateTime.UtcNow.Date);
                    }

                    dto.Gender = user.Gender;
                }
            }

            var vitalsList = (await _uow.PatientVitalsRepo
                .GetAllAsync(v => v.PatientID == patientDetails.PatientID && !v.IsDeleted))?.ToList()
                ?? new List<T_PatientVitals>();

            var latestVitals = vitalsList
                .OrderByDescending(v => v.CreatedOn)
                .FirstOrDefault();

            if (latestVitals != null)
            {
                dto.Height = latestVitals.Height;
                dto.Weight = latestVitals.Weight;
                dto.PulseRate = latestVitals.PulseRate;
                dto.BloodPressure = latestVitals.BloodPressure;
                dto.IsDiabetic = latestVitals.IsDiabetic ?? false;
                dto.DiabeticReadings = latestVitals.DiabeticReadings;
                dto.HasHighBloodPressure = latestVitals.HasHighBloodPressure ?? false;
                dto.BloodPressureReadings = latestVitals.BloodPressureReadings;
            }

            dto.ChronicDiseases = (patientDetails.ChronicDiseases ?? new List<T_ChronicDiseases>())
                .Where(cd => !cd.IsDeleted)
                .Select(cd => new CheckupChronicDisease_DTO
                {
                    DiseaseName = cd.DiseaseName,
                    DiagnosedDate = cd.DiagnosedDate,
                    CurrentStatus = cd.CurrentStatus
                })
                .ToList();

            var prescriptions = (await _uow.PrescriptionsRepo
                .GetAllAsync(p => p.PatientID == patientDetails.PatientID && !p.IsDeleted))?.ToList()
                ?? new List<T_Prescriptions>();

            dto.PreviousPrescriptions = prescriptions
                .OrderByDescending(p => p.CreatedOn)
                .Take(10)
                .Select(p => new PreviousPrescription_DTO
                {
                    PrescriptionID = p.PrescriptionID,
                    DoctorName = p.Doctor != null && p.Doctor.User != null
                        ? $"Dr. {p.Doctor.User.FirstName} {p.Doctor.User.LastName}".Trim()
                        : "Doctor",
                    CreatedOn = p.CreatedOn,
                    Notes = p.Notes,
                    Medications = string.Join(", ", (p.PrescriptionItems ?? new List<T_PrescriptionItems>())
                        .Where(i => !i.IsDeleted && !string.IsNullOrWhiteSpace(i.MedicineName))
                        .Select(i => $"{i.MedicineName} {i.Dosage}"))
                })
                .ToList();

            dto.PreviousVitals = vitalsList
                .OrderByDescending(v => v.CreatedOn)
                .Skip(1)
                .Take(10)
                .Select(v => new PreviousVital_DTO
                {
                    RecordedDate = v.CreatedOn,
                    Height = v.Height,
                    Weight = v.Weight,
                    BloodPressure = v.BloodPressure,
                    PulseRate = v.PulseRate,
                    IsDiabetic = v.IsDiabetic ?? false
                })
                .ToList();

            return Result<DoctorCheckup_DTO>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building doctor checkup model for appointment {AppointmentId}", appointmentId);
            return Result<DoctorCheckup_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> CreatePrescriptionAsync(DoctorCreatePrescription_DTO input, string doctorUserId)
    {
        try
        {
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == doctorUserId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                _logger.LogWarning("Doctor profile not found for user {UserId}", doctorUserId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Doctor profile not found." },
                    "Doctor profile not found");
            }

            if (input == null)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Invalid request." },
                    "Input is null");
            }

            if (input.Medications == null || input.Medications.Count == 0)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Please add at least one medication to create a prescription." },
                    "No medications provided");
            }

            var appointment = await _uow.AppointmentsRepo.GetByIdAsync(input.AppointmentId);
            if (appointment == null || appointment.IsDeleted)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found while creating prescription.", input.AppointmentId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Appointment not found." },
                    "Appointment not found");
            }

            if (appointment.DoctorID != doctor.DoctorID)
            {
                _logger.LogWarning("Doctor {DoctorId} attempted to create prescription for appointment {AppointmentId} they don't own.", doctor.DoctorID, input.AppointmentId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Unauthorized to modify this appointment." },
                    "Unauthorized");
            }

            if (appointment.PatientID != input.PatientId)
            {
                _logger.LogWarning("Appointment {AppointmentId} patient mismatch when creating prescription. Expected {ExpectedPatientId}, got {ActualPatientId}",
                    input.AppointmentId, appointment.PatientID, input.PatientId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Invalid patient for this appointment." },
                    "Patient mismatch");
            }

            await _uow.BeginTransactionAsync();

            var prescription = new T_Prescriptions
            {
                AppointmentID = appointment.AppointmentID,
                DoctorID = doctor.DoctorID,
                PatientID = appointment.PatientID,
                Notes = input.PrescriptionNotes,
                CreatedBy = doctorUserId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.PrescriptionsRepo.AddAsync(prescription);

            var items = new List<T_PrescriptionItems>();
            foreach (var med in input.Medications)
            {
                if (string.IsNullOrWhiteSpace(med.MedicineName))
                    continue;

                var item = new T_PrescriptionItems
                {
                    MedicineName = med.MedicineName,
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    Duration = med.DurationDays > 0 ? $"{med.DurationDays} days" : null,
                    Notes = med.Instructions,
                    CreatedBy = doctorUserId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                items.Add(item);
            }

            prescription.PrescriptionItems = items;

            if (appointment.Status == AppointmentStatus_Enum.PrescriptionPending)
            {
                appointment.Status = AppointmentStatus_Enum.Completed;
                appointment.UpdatedOn = DateTime.UtcNow;
            }

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Prescription created successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription for doctor user {UserId}", doctorUserId);

            try
            {
                await _uow.RollbackAsync();
            }
            catch
            {
                // ignore rollback errors
            }

            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateVitalsAsync(DoctorUpdateVitals_DTO input, string doctorUserId)
    {
        try
        {
            var doctors = (await _uow.DoctorDetailsRepo.GetAllAsync(d => d.UserID == doctorUserId && !d.IsDeleted))?.ToList()
                          ?? new List<T_DoctorDetails>();
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                _logger.LogWarning("Doctor profile not found for user {UserId}", doctorUserId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Doctor profile not found." },
                    "Doctor profile not found");
            }

            if (input == null)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Invalid request." },
                    "Input is null");
            }

            var appointment = await _uow.AppointmentsRepo.GetByIdAsync(input.AppointmentId);
            if (appointment == null || appointment.IsDeleted)
            {
                _logger.LogWarning("Appointment {AppointmentId} not found while updating vitals.", input.AppointmentId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Appointment not found." },
                    "Appointment not found");
            }

            if (appointment.DoctorID != doctor.DoctorID)
            {
                _logger.LogWarning("Doctor {DoctorId} attempted to update vitals for appointment {AppointmentId} they don't own.", doctor.DoctorID, input.AppointmentId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Unauthorized to modify this appointment." },
                    "Unauthorized");
            }

            if (appointment.PatientID != input.PatientId)
            {
                _logger.LogWarning("Appointment {AppointmentId} patient mismatch when updating vitals. Expected {ExpectedPatientId}, got {ActualPatientId}. Using appointment's patient ID.",
                    input.AppointmentId, appointment.PatientID, input.PatientId);

                // Trust the appointment's patient association to avoid blocking updates
                input.PatientId = appointment.PatientID;
            }

            await _uow.BeginTransactionAsync();

            var vitals = new T_PatientVitals
            {
                PatientID = input.PatientId,
                Height = input.Height,
                Weight = input.Weight,
                PulseRate = input.PulseRate,
                BloodPressure = input.BloodPressure,
                IsDiabetic = input.IsDiabetic,
                DiabeticReadings = input.DiabeticReadings,
                HasHighBloodPressure = input.HasHighBloodPressure,
                BloodPressureReadings = input.BloodPressureReadings,
                CreatedBy = doctorUserId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.PatientVitalsRepo.AddAsync(vitals);

            var patientDetails = await _uow.PatientDetailsRepo.GetByIdAsync(input.PatientId);
            if (patientDetails == null || patientDetails.IsDeleted)
            {
                _logger.LogWarning("Patient details not found while updating vitals for patient {PatientId}", input.PatientId);
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Patient details not found." },
                    "Patient details not found");
            }

            // Ensure chronic diseases navigation collection is initialized to avoid null reference issues
            if (patientDetails.ChronicDiseases == null)
            {
                patientDetails.ChronicDiseases = new List<T_ChronicDiseases>();
            }

            var existingDiseases = patientDetails.ChronicDiseases
                .Where(cd => !cd.IsDeleted)
                .ToList();

            var submitted = input.ChronicDiseases ?? new List<CheckupChronicDisease_DTO>();

            foreach (var existing in existingDiseases)
            {
                var match = submitted.FirstOrDefault(cd =>
                    string.Equals(cd.DiseaseName, existing.DiseaseName, StringComparison.OrdinalIgnoreCase)
                    && cd.DiagnosedDate == existing.DiagnosedDate);

                if (match == null)
                {
                    existing.IsDeleted = true;
                    existing.UpdatedBy = doctorUserId;
                    existing.UpdatedOn = DateTime.UtcNow;
                }
                else
                {
                    existing.CurrentStatus = match.CurrentStatus;
                    existing.UpdatedBy = doctorUserId;
                    existing.UpdatedOn = DateTime.UtcNow;
                }
            }

            foreach (var cd in submitted)
            {
                var exists = existingDiseases.Any(e =>
                    string.Equals(e.DiseaseName, cd.DiseaseName, StringComparison.OrdinalIgnoreCase)
                    && e.DiagnosedDate == cd.DiagnosedDate
                    && !e.IsDeleted);

                if (!exists && !string.IsNullOrWhiteSpace(cd.DiseaseName))
                {
                    var entity = new T_ChronicDiseases
                    {
                        PatientID = input.PatientId,
                        DiseaseName = cd.DiseaseName,
                        DiagnosedDate = cd.DiagnosedDate,
                        CurrentStatus = cd.CurrentStatus,
                        CreatedBy = doctorUserId,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    patientDetails.ChronicDiseases.Add(entity);
                }
            }

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Vitals updated successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vitals for doctor user {UserId}", doctorUserId);

            try
            {
                await _uow.RollbackAsync();
            }
            catch
            {
                // ignore rollback errors
            }

            return Result<GeneralResponse>.Exception(ex);
        }
    }
}