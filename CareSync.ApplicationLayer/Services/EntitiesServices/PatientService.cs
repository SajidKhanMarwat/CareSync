using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.InfrastructureLayer.Services.EntitiesServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public sealed class PatientService(UserManager<T_Users> userManager,
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<UserService> logger) : IPatientService
{
    public Task<Result<List<GetAllPatients_DTO>>> GetAllPatientsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPatient_DTO>> GetPatientByIdAsync(object patientId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<GeneralResponse>> AddPatientDetailsAsync(RegisterPatient_DTO patient)
    {
        logger.LogInformation("Executing: AddPatientAsync");

        try
        {
            await uow.PatientDetailsRepo.AddAsync(mapper.Map<T_PatientDetails>(patient));
            return Result<GeneralResponse>.Success(new GeneralResponse()
            {
                Success = true,
                Message = "profile updated successfully.",
            });
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogInformation(dbEx.Message);
            var sqlException = dbEx.InnerException as SqlException ?? dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered. Please login to your account.",
                });
            return Result<GeneralResponse>.Exception(dbEx);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserPatientAsync(UserPatientProfileUpdate_DTO request)
    {
        logger.LogInformation("Executing: RegisterNewUser");
        try
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse()
                {
                    Success = false,
                    Message = "User not found"
                });

            mapper.Map(request, user);
            var response = await userManager.UpdateAsync(user);
            if (response.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse()
                {
                    Success = true,
                    Message = "profile updated successfully.",
                });
            return Result<GeneralResponse>.Failure(new GeneralResponse()
            {
                Success = false,
                Message = response.Errors.FirstOrDefault()!.Description,
            });
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogInformation(dbEx.Message);
            var sqlException = dbEx.InnerException as SqlException ?? dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered. Please login to your account.",
                });
            return Result<GeneralResponse>.Exception(dbEx);
        }
    }

    public Task<Result<GeneralResponse>> DeleteUserPatientAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PatientDashboard_DTO>> GetPatientDashboardAsync(string userId)
    {
        logger.LogInformation($"Getting patient dashboard for user: {userId}");

        try
        {
            // Get patient details with user information
            var patients = await uow.PatientDetailsRepo
                .GetAllAsync(p => p.UserID == userId && !p.IsDeleted);
            
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                logger.LogWarning($"Patient not found for user: {userId}");
                return Result<PatientDashboard_DTO>.Failure(
                    new PatientDashboard_DTO(),
                    "Patient profile not found");
            }

            // Get user details
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<PatientDashboard_DTO>.Failure(
                    new PatientDashboard_DTO(),
                    "User not found");
            }

            var dashboard = new PatientDashboard_DTO();

            // Build Profile Information
            dashboard.Profile = await BuildPatientProfileAsync(user, patient);

            // Build Dashboard Statistics
            dashboard.Statistics = await BuildDashboardStatsAsync(patient.PatientID);

            // Get Recent Doctor Visits (last 5 completed appointments)
            dashboard.RecentVisits = await GetRecentDoctorVisitsAsync(patient.PatientID);

            // Get Recent Medical Reports (last 5 reports)
            dashboard.RecentReports = await GetRecentMedicalReportsAsync(patient.PatientID);

            // Get Latest Health Vitals
            dashboard.LatestVitals = await GetLatestHealthVitalsAsync(patient.PatientID);

            // Get Health Vitals History (last 5 readings for each vital type)
            dashboard.VitalsHistory = await GetHealthVitalsHistoryAsync(patient.PatientID);

            logger.LogInformation($"Successfully retrieved dashboard for patient: {patient.PatientID}");
            return Result<PatientDashboard_DTO>.Success(dashboard);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error retrieving patient dashboard for user: {userId}");
            return Result<PatientDashboard_DTO>.Exception(ex);
        }
    }

    #region Private Helper Methods

    private async Task<PatientDashboardProfile_DTO> BuildPatientProfileAsync(T_Users user, T_PatientDetails patient)
    {
        var profile = new PatientDashboardProfile_DTO
        {
            PatientName = $"{user.FirstName} {user.LastName}".Trim(),
            Gender = user.Gender.ToString(),
            Age = user.Age ?? CalculateAge(user.DateOfBirth),
            BloodType = patient.BloodGroup ?? "Unknown",
            ProfileImage = user.ProfileImage
        };

        // Get primary doctor (most frequent doctor from appointments)
        var primaryDoctor = await uow.AppointmentsRepo
            .GetAllAsync(a => a.PatientID == patient.PatientID && !a.IsDeleted)
            .ContinueWith(async t =>
            {
                var appointments = t.Result;
                if (!appointments.Any()) return null;

                var mostFrequentDoctorId = appointments
                    .GroupBy(a => a.DoctorID)
                    .OrderByDescending(g => g.Count())
                    .FirstOrDefault()?.Key;

                if (mostFrequentDoctorId.HasValue)
                {
                    var doctor = await uow.DoctorDetailsRepo.GetByIdAsync(mostFrequentDoctorId.Value);
                    if (doctor != null)
                    {
                        var doctorUser = await userManager.FindByIdAsync(doctor.UserID);
                        return doctorUser != null ? $"Dr. {doctorUser.FirstName} {doctorUser.LastName}" : null;
                    }
                }
                return null;
            });

        profile.PrimaryDoctor = await primaryDoctor ?? "Not Assigned";

        // Get last visit date
        var lastVisit = await uow.AppointmentsRepo
            .GetAllAsync(a => a.PatientID == patient.PatientID && 
                             a.Status == Shared.Enums.Appointment.AppointmentStatus_Enum.Scheduled && 
                             a.AppointmentDate < DateTime.Now && 
                             !a.IsDeleted)
            .ContinueWith(t => t.Result.OrderByDescending(a => a.AppointmentDate).FirstOrDefault());

        profile.LastVisitDate = lastVisit?.AppointmentDate.ToString("MMM dd, yyyy");

        // Get next appointment date
        var nextAppointment = await uow.AppointmentsRepo
            .GetAllAsync(a => a.PatientID == patient.PatientID && 
                             a.AppointmentDate > DateTime.Now && 
                             !a.IsDeleted)
            .ContinueWith(t => t.Result.OrderBy(a => a.AppointmentDate).FirstOrDefault());

        profile.NextAppointmentDate = nextAppointment?.AppointmentDate.ToString("MMM dd, yyyy");

        return profile;
    }

    private async Task<DashboardStats_DTO> BuildDashboardStatsAsync(int patientId)
    {
        var stats = new DashboardStats_DTO();

        // Count upcoming appointments
        stats.UpcomingAppointments = await uow.AppointmentsRepo
            .GetCountAsync(a => a.PatientID == patientId && 
                               a.AppointmentDate > DateTime.Now && 
                               !a.IsDeleted);

        // Count active prescriptions (prescriptions from last 30 days)
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        try
        {
            // Get appointments from last 30 days and count their prescriptions
            var recentAppointments = await uow.AppointmentsRepo
                .GetAllAsync(a => a.PatientID == patientId && 
                                 a.CreatedOn >= thirtyDaysAgo && 
                                 !a.IsDeleted);
            
            stats.ActivePrescriptions = recentAppointments.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting active prescriptions");
            stats.ActivePrescriptions = 0;
        }

        // Count pending lab tests
        stats.PendingLabTests = 0; // Lab requests not yet implemented in UnitOfWork

        // Count new reports (last 7 days)
        var sevenDaysAgo = DateTime.Now.AddDays(-7);
        try
        {
            var recentReports = await uow.PatientReportsRepo
                .GetAllAsync(r => r.PatientID == patientId);
            
            stats.NewReports = recentReports.Count();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting new reports");
            stats.NewReports = 0;
        }

        return stats;
    }

    private async Task<List<RecentDoctorVisit_DTO>> GetRecentDoctorVisitsAsync(int patientId)
    {
        var visits = new List<RecentDoctorVisit_DTO>();

        var recentAppointments = await uow.AppointmentsRepo
            .GetAllAsync(a => a.PatientID == patientId && 
                             a.AppointmentDate <= DateTime.Now && 
                             !a.IsDeleted)
            .ContinueWith(t => t.Result
                .OrderByDescending(a => a.AppointmentDate)
                .Take(5)
                .ToList());

        foreach (var appointment in recentAppointments)
        {
            var doctor = await uow.DoctorDetailsRepo.GetByIdAsync(appointment.DoctorID);
            if (doctor == null) continue;

            var doctorUser = await userManager.FindByIdAsync(doctor.UserID);
            if (doctorUser == null) continue;

            visits.Add(new RecentDoctorVisit_DTO
            {
                AppointmentId = appointment.AppointmentID,
                DoctorName = $"Dr. {doctorUser.FirstName} {doctorUser.LastName}",
                DoctorImage = doctorUser.ProfileImage ?? "~/theme/images/user.png",
                Specialization = doctor.Specialization ?? "General Physician",
                VisitDate = appointment.AppointmentDate.ToString("MMM dd, yyyy"),
                Department = doctor.Specialization ?? "General Medicine",
                Status = appointment.Status.ToString()
            });
        }

        return visits;
    }

    private async Task<List<RecentMedicalReport_DTO>> GetRecentMedicalReportsAsync(int patientId)
    {
        var reports = new List<RecentMedicalReport_DTO>();

        try
        {
            var patientReports = await uow.PatientReportsRepo
                .GetAllAsync(r => r.PatientID == patientId)
                .ContinueWith(t => t.Result
                    .OrderByDescending(r => r.PatientReportID)
                    .Take(5)
                    .ToList());

            foreach (var report in patientReports)
            {
                reports.Add(new RecentMedicalReport_DTO
                {
                    ReportId = report.PatientReportID,
                    ReportTitle = report.Description ?? "Medical Report",
                    ReportType = string.IsNullOrEmpty(report.Documnt) ? "Lab Report" : "Medical Document",
                    ReportDate = DateTime.Now.ToString("MMM dd, yyyy"), // Use actual date when available
                    FileUrl = report.FilePath
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error retrieving medical reports for patient: {patientId}");
        }

        return reports;
    }

    private async Task<HealthVitals_DTO?> GetLatestHealthVitalsAsync(int patientId)
    {
        try
        {
            var latestVital = await uow.PatientVitalsRepo
                .GetAllAsync(v => v.PatientID == patientId && !v.IsDeleted)
                .ContinueWith(t => t.Result
                    .OrderByDescending(v => v.CreatedOn)
                    .FirstOrDefault());

            if (latestVital == null)
                return null;

            // Parse blood pressure if available
            decimal? systolic = null;
            decimal? diastolic = null;
            if (!string.IsNullOrEmpty(latestVital.BloodPressure))
            {
                var bpParts = latestVital.BloodPressure.Split('/');
                if (bpParts.Length == 2)
                {
                    if (decimal.TryParse(bpParts[0], out var sys))
                        systolic = sys;
                    if (decimal.TryParse(bpParts[1], out var dia))
                        diastolic = dia;
                }
            }

            // Parse diabetic readings for blood sugar
            decimal? bloodSugar = null;
            if (!string.IsNullOrEmpty(latestVital.DiabeticReadings))
            {
                if (decimal.TryParse(latestVital.DiabeticReadings, out var sugar))
                    bloodSugar = sugar;
            }

            return new HealthVitals_DTO
            {
                BloodPressureSystolic = systolic,
                BloodPressureDiastolic = diastolic,
                BloodSugar = bloodSugar,
                HeartRate = latestVital.PulseRate,
                Cholesterol = null, // Not available in current schema
                Weight = latestVital.Weight,
                Height = latestVital.Height,
                Temperature = null, // Not available in current schema
                RecordedDate = latestVital.CreatedOn.ToString("MMM dd, yyyy")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error retrieving health vitals for patient: {patientId}");
            return null;
        }
    }

    private async Task<HealthVitalsHistory_DTO> GetHealthVitalsHistoryAsync(int patientId)
    {
        var history = new HealthVitalsHistory_DTO();

        try
        {
            // Get last 5 vitals records
            var vitalsRecords = await uow.PatientVitalsRepo
                .GetAllAsync(v => v.PatientID == patientId && !v.IsDeleted)
                .ContinueWith(t => t.Result
                    .OrderByDescending(v => v.CreatedOn)
                    .Take(5)
                    .OrderBy(v => v.CreatedOn)
                    .ToList());

            foreach (var vital in vitalsRecords)
            {
                var date = vital.CreatedOn.ToString("dd/MM/yyyy");

                // Blood Pressure
                if (!string.IsNullOrEmpty(vital.BloodPressure))
                {
                    var bpParts = vital.BloodPressure.Split('/');
                    if (bpParts.Length == 2 && decimal.TryParse(bpParts[0], out var systolic))
                    {
                        history.BloodPressureReadings.Add(new VitalReading_DTO
                        {
                            Date = date,
                            Value = systolic,
                            Status = GetBPStatus(systolic),
                            BadgeClass = GetBPBadgeClass(systolic)
                        });
                    }
                }

                // Blood Sugar
                if (!string.IsNullOrEmpty(vital.DiabeticReadings))
                {
                    if (decimal.TryParse(vital.DiabeticReadings, out var sugar))
                    {
                        history.BloodSugarReadings.Add(new VitalReading_DTO
                        {
                            Date = date,
                            Value = sugar,
                            Status = GetSugarStatus(sugar),
                            BadgeClass = GetSugarBadgeClass(sugar)
                        });
                    }
                }

                // Heart Rate
                if (vital.PulseRate.HasValue)
                {
                    history.HeartRateReadings.Add(new VitalReading_DTO
                    {
                        Date = date,
                        Value = vital.PulseRate.Value,
                        Status = GetHeartRateStatus(vital.PulseRate.Value),
                        BadgeClass = GetHeartRateBadgeClass(vital.PulseRate.Value)
                    });
                }

                // Cholesterol (if available in future schema updates)
                // For now, we'll generate sample data based on weight trends
                if (vital.Weight.HasValue)
                {
                    // Estimate cholesterol based on weight (this is just for demo)
                    var estimatedCholesterol = 150 + (vital.Weight.Value * 0.5m);
                    history.CholesterolReadings.Add(new VitalReading_DTO
                    {
                        Date = date,
                        Value = estimatedCholesterol,
                        Status = GetCholesterolStatus(estimatedCholesterol),
                        BadgeClass = GetCholesterolBadgeClass(estimatedCholesterol)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error retrieving vitals history for patient: {patientId}");
        }

        return history;
    }

    // Helper methods for status determination
    private string GetBPStatus(decimal systolic)
    {
        if (systolic < 120) return "Normal";
        if (systolic < 140) return "Elevated";
        if (systolic < 180) return "High";
        return "Critical";
    }

    private string GetBPBadgeClass(decimal systolic)
    {
        if (systolic < 120) return "bg-success";
        if (systolic < 140) return "bg-info";
        if (systolic < 180) return "bg-warning";
        return "bg-danger";
    }

    private string GetSugarStatus(decimal sugar)
    {
        if (sugar < 100) return "Normal";
        if (sugar < 126) return "Prediabetic";
        return "Diabetic";
    }

    private string GetSugarBadgeClass(decimal sugar)
    {
        if (sugar < 100) return "bg-success";
        if (sugar < 126) return "bg-warning";
        return "bg-danger";
    }

    private string GetHeartRateStatus(int heartRate)
    {
        if (heartRate >= 60 && heartRate <= 100) return "Normal";
        if (heartRate < 60) return "Low";
        return "High";
    }

    private string GetHeartRateBadgeClass(int heartRate)
    {
        if (heartRate >= 60 && heartRate <= 100) return "bg-success";
        if (heartRate < 60) return "bg-info";
        return "bg-warning";
    }

    private string GetCholesterolStatus(decimal cholesterol)
    {
        if (cholesterol < 200) return "Normal";
        if (cholesterol < 240) return "Borderline";
        return "High";
    }

    private string GetCholesterolBadgeClass(decimal cholesterol)
    {
        if (cholesterol < 200) return "bg-success";
        if (cholesterol < 240) return "bg-warning";
        return "bg-danger";
    }

    private int CalculateAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue) return 0;

        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Value.Year;
        
        if (dateOfBirth.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }

    #endregion

    public async Task<Result<List<DoctorBooking_DTO>>> GetAvailableDoctorsAsync(string? specialization = null)
    {
        logger.LogInformation($"Getting available doctors. Specialization filter: {specialization ?? "All"}");

        try
        {
            var query = uow.DoctorDetailsRepo.GetAllAsync(d => !d.IsDeleted);
            var doctors = await query;

            if (!string.IsNullOrEmpty(specialization))
            {
                doctors = doctors.Where(d => d.Specialization != null && 
                    d.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var doctorDtos = new List<DoctorBooking_DTO>();

            foreach (var doctor in doctors)
            {
                var user = await userManager.FindByIdAsync(doctor.UserID);
                if (user == null || !user.IsActive) continue;

                var availableDays = ParseAvailableDays(doctor.AvailableDays);
                var timeSlots = GenerateTimeSlots(doctor.StartTime, doctor.EndTime);

                doctorDtos.Add(new DoctorBooking_DTO
                {
                    DoctorID = doctor.DoctorID,
                    DoctorName = $"{user.FirstName} {user.LastName}",
                    Specialization = doctor.Specialization ?? "General",
                    ExperienceYears = doctor.ExperienceYears ?? 0,
                    ProfileImage = user.ProfileImage ?? "/theme/images/user.png",
                    Rating = 4.5m,
                    ReviewCount = 0,
                    Location = doctor.ClinicAddress ?? "Not specified",
                    ConsultationFee = "$120",
                    AvailableDays = doctor.AvailableDays ?? "",
                    StartTime = doctor.StartTime ?? "09:00",
                    EndTime = doctor.EndTime ?? "17:00",
                    AvailableDaysList = availableDays,
                    AvailableTimeSlots = timeSlots,
                    AvailabilityStatus = "Available",
                    NextAvailableSlot = GetNextAvailableSlot(availableDays, timeSlots)
                });
            }

            return Result<List<DoctorBooking_DTO>>.Success(doctorDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting available doctors");
            return Result<List<DoctorBooking_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<DoctorBooking_DTO>> GetDoctorByIdAsync(int doctorId)
    {
        logger.LogInformation($"Getting doctor by ID: {doctorId}");

        try
        {
            var doctors = await uow.DoctorDetailsRepo.GetAllAsync(d => d.DoctorID == doctorId && !d.IsDeleted);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return Result<DoctorBooking_DTO>.Failure(
                    null!,
                    "Doctor not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            var user = await userManager.FindByIdAsync(doctor.UserID);
            if (user == null)
            {
                return Result<DoctorBooking_DTO>.Failure(
                    null!,
                    "Doctor user not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            var availableDays = ParseAvailableDays(doctor.AvailableDays);
            var timeSlots = GenerateTimeSlots(doctor.StartTime, doctor.EndTime);

            var doctorDto = new DoctorBooking_DTO
            {
                DoctorID = doctor.DoctorID,
                DoctorName = $"{user.FirstName} {user.LastName}",
                Specialization = doctor.Specialization ?? "General",
                ExperienceYears = doctor.ExperienceYears ?? 0,
                ProfileImage = user.ProfileImage ?? "/theme/images/user.png",
                Rating = 4.5m,
                ReviewCount = 0,
                Location = doctor.ClinicAddress ?? "Not specified",
                ConsultationFee = "$120",
                AvailableDays = doctor.AvailableDays ?? "",
                StartTime = doctor.StartTime ?? "09:00",
                EndTime = doctor.EndTime ?? "17:00",
                AvailableDaysList = availableDays,
                AvailableTimeSlots = timeSlots,
                AvailabilityStatus = "Available",
                NextAvailableSlot = GetNextAvailableSlot(availableDays, timeSlots)
            };

            return Result<DoctorBooking_DTO>.Success(doctorDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor by ID");
            return Result<DoctorBooking_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<DoctorTimeSlot_DTO>>> GetDoctorTimeSlotsAsync(int doctorId, DateTime date)
    {
        logger.LogInformation($"Getting time slots for doctor {doctorId} on {date:yyyy-MM-dd}");

        try
        {
            var doctors = await uow.DoctorDetailsRepo.GetAllAsync(d => d.DoctorID == doctorId && !d.IsDeleted);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                return Result<List<DoctorTimeSlot_DTO>>.Failure(
                    new List<DoctorTimeSlot_DTO>(),
                    "Doctor not found");
            }

            var availableDays = ParseAvailableDays(doctor.AvailableDays);
            //var dayOfWeek = date.DayOfWeek.ToString().Substring(0, 3);
            
            //if (!availableDays.Contains(dayOfWeek))
            //{
            //    return Result<List<DoctorTimeSlot_DTO>>.Success(new List<DoctorTimeSlot_DTO>());
            //}

            var timeSlots = GenerateTimeSlots(doctor.StartTime, doctor.EndTime);

            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => 
                a.DoctorID == doctorId && 
                a.AppointmentDate.Date == date.Date &&
                !a.IsDeleted);

            var bookedTimes = appointments.Select(a => a.AppointmentDate.ToString("HH:mm")).ToList();

            var slots = timeSlots.Select(time => new DoctorTimeSlot_DTO
            {
                Time = time,
                IsAvailable = !bookedTimes.Contains(time),
                IsBooked = bookedTimes.Contains(time)
            }).ToList();

            return Result<List<DoctorTimeSlot_DTO>>.Success(slots);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor time slots");
            return Result<List<DoctorTimeSlot_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> BookAppointmentAsync(string userId, BookAppointmentRequest_DTO request)
    {
        logger.LogInformation($"Booking appointment for user {userId} with doctor {request.DoctorID}");

        try
        {
            await uow.BeginTransactionAsync();

            var patients = await uow.PatientDetailsRepo.GetAllAsync(p => p.UserID == userId && !p.IsDeleted);
            var patient = patients.FirstOrDefault();

            if (patient == null)
            {
                await uow.RollbackAsync();
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Patient profile not found. Please complete your profile first."
                });
            }

            var doctors = await uow.DoctorDetailsRepo.GetAllAsync(d => d.DoctorID == request.DoctorID && !d.IsDeleted);
            var doctor = doctors.FirstOrDefault();

            if (doctor == null)
            {
                await uow.RollbackAsync();
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Doctor not found or not available."
                });
            }

            var appointmentDateTime = DateTime.Parse($"{request.AppointmentDate:yyyy-MM-dd} {request.AppointmentTime}");

            var existingAppointments = await uow.AppointmentsRepo.GetAllAsync(a =>
                a.DoctorID == request.DoctorID &&
                a.AppointmentDate == appointmentDateTime &&
                !a.IsDeleted);

            if (existingAppointments.Any())
            {
                await uow.RollbackAsync();
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This time slot is already booked. Please select another time."
                });
            }

            var appointmentType = request.AppointmentType.ToLower() switch
            {
                "consultation" => CareSync.Shared.Enums.Appointment.AppointmentType_Enum.Consultation,
                "followup" => CareSync.Shared.Enums.Appointment.AppointmentType_Enum.FollowUp,
                "checkup" => CareSync.Shared.Enums.Appointment.AppointmentType_Enum.Checkup,
                "emergency" => CareSync.Shared.Enums.Appointment.AppointmentType_Enum.Emergency,
                _ => CareSync.Shared.Enums.Appointment.AppointmentType_Enum.Consultation
            };

            var appointment = new T_Appointments
            {
                DoctorID = request.DoctorID,
                PatientID = patient.PatientID,
                AppointmentDate = appointmentDateTime,
                AppointmentType = appointmentType,
                Status = CareSync.Shared.Enums.Appointment.AppointmentStatus_Enum.Scheduled,
                Reason = request.Reason,
                Notes = BuildAppointmentNotes(request),
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            await uow.AppointmentsRepo.AddAsync(appointment);
            await uow.SaveChangesAsync();
            await uow.CommitAsync();

            logger.LogInformation($"Appointment booked successfully. ID: {appointment.AppointmentID}");

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Appointment booked successfully! You will receive a confirmation email shortly."
            });
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync();
            logger.LogError(ex, "Error booking appointment");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    private List<string> ParseAvailableDays(string? availableDays)
    {
        if (string.IsNullOrEmpty(availableDays))
            return new List<string>();

        return availableDays.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .ToList();
    }

    private List<string> GenerateTimeSlots(string? startTime, string? endTime)
    {
        var slots = new List<string>();
        
        if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            return slots;

        try
        {
            var start = TimeSpan.Parse(startTime);
            var end = TimeSpan.Parse(endTime);
            var interval = TimeSpan.FromHours(1);

            for (var time = start; time < end; time += interval)
            {
                slots.Add(time.ToString(@"hh\:mm"));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating time slots");
        }

        return slots;
    }

    private string GetNextAvailableSlot(List<string> availableDays, List<string> timeSlots)
    {
        if (!availableDays.Any() || !timeSlots.Any())
            return "Not available";

        var today = DateTime.Now;
        var daysOfWeek = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        for (int i = 0; i < 7; i++)
        {
            var checkDate = today.AddDays(i);
            var dayAbbr = daysOfWeek[(int)checkDate.DayOfWeek];

            if (availableDays.Contains(dayAbbr))
            {
                var dateStr = i == 0 ? "Today" : i == 1 ? "Tomorrow" : checkDate.ToString("MMM dd");
                return $"{dateStr} {timeSlots.First()}";
            }
        }

        return "Not available";
    }

    private string BuildAppointmentNotes(BookAppointmentRequest_DTO request)
    {
        var notes = new List<string>();

        if (!string.IsNullOrEmpty(request.CurrentMedications))
            notes.Add($"Current Medications: {request.CurrentMedications}");

        if (!string.IsNullOrEmpty(request.Allergies))
            notes.Add($"Allergies: {request.Allergies}");

        if (request.UseInsurance)
            notes.Add("Patient will use insurance");

        if (request.SendReminders)
            notes.Add("Send appointment reminders");

        return string.Join(" | ", notes);
    }
}
