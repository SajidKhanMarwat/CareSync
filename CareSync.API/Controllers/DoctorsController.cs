using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareSync.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Doctor")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    /// <summary>
    /// Returns dashboard data scoped to the authenticated doctor.
    /// </summary>
    [HttpGet("dashboard")]
    [AllowAnonymous]
    public async Task<Result<DoctorDashboard_DTO>> GetDashboard()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Doctor dashboard requested but user id claim not present.");
                return Result<DoctorDashboard_DTO>.Failure(new DoctorDashboard_DTO(), "Unauthenticated.");
            }

            _logger.LogInformation("Building dashboard for doctor user {UserId}", userId);
            return await _doctorService.GetDoctorDashboardAsync(userId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in DoctorsController.GetDashboard");
            return Result<DoctorDashboard_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get appointments for the authenticated doctor.
    /// This endpoint is scoped to the Doctor role and intended for UI pages under /Doctor.
    /// </summary>
    [HttpGet("appointments")]
    [AllowAnonymous]
    public async Task<Result<TodaysAppointmentsList_DTO>> GetAppointmentsForCurrentDoctor()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Doctor appointments requested but user id claim not present.");
                return Result<TodaysAppointmentsList_DTO>.Failure(new TodaysAppointmentsList_DTO(), "Unauthenticated.");
            }

            _logger.LogInformation("Getting appointments for doctor user {UserId}", userId);
            var dashboardResult = await _doctorService.GetDoctorDashboardAsync(userId);

            if (dashboardResult == null || !dashboardResult.IsSuccess || dashboardResult.Data == null)
            {
                _logger.LogWarning("Doctor dashboard call failed while fetching appointments for user {UserId}: {Error}", userId, dashboardResult?.GetError());
                return Result<TodaysAppointmentsList_DTO>.Failure(new TodaysAppointmentsList_DTO(), dashboardResult?.GetError() ?? "Failed to load appointments");
            }

            var todayItems = new List<TodayAppointmentItem>();
            var todayAppointments = dashboardResult.Data.TodayAppointments ?? new List<CareSync.ApplicationLayer.Contracts.DoctorsDTOs.TodayAppointment_DTO>();
            var previousAppointments = dashboardResult.Data.PreviousAppointments ?? new List<CareSync.ApplicationLayer.Contracts.DoctorsDTOs.PreviousAppointment_DTO>();

            foreach (var t in todayAppointments)
            {
                todayItems.Add(new TodayAppointmentItem
                {
                    AppointmentID = t.Id,
                    PatientID = t.PatientID,
                    PatientName = t.PatientName,
                    DoctorID = t.DoctorID,
                    DoctorName = dashboardResult.Data.DoctorName,
                    DoctorSpecialization = dashboardResult.Data.Specialization,
                    AppointmentDate = t.AppointmentTime.Date,
                    AppointmentTime = t.AppointmentTime,
                    Status = t.Status,
                    AppointmentType = t.AppointmentType,
                    Reason = t.Diagnosis
                });
            }

            var previousForDashboard = previousAppointments
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs.PreviousAppointment_DTO
                {
                    AppointmentID = a.AppointmentID,
                    PatientID = a.PatientID,
                    PatientName = a.PatientName,
                    DoctorID = a.DoctorID,
                    DoctorName = a.DoctorName,
                    DoctorSpecialization = a.DoctorSpecialization,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    AppointmentType = a.AppointmentType,
                    Reason = a.Reason
                })
                .ToList();

            var list = new TodaysAppointmentsList_DTO
            {
                Appointments = todayItems.OrderBy(a => a.AppointmentTime).ToList(),
                PreviousAppointments = previousForDashboard,
                TotalToday = todayItems.Count(i => i.AppointmentDate == DateTime.Today),
                CompletedToday = todayItems.Count(i => i.Status == AppointmentStatus_Enum.Completed),
                PendingToday = todayItems.Count(i => i.Status == AppointmentStatus_Enum.Pending || i.Status == AppointmentStatus_Enum.Created)
            };

            return Result<TodaysAppointmentsList_DTO>.Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for current doctor");
            return Result<TodaysAppointmentsList_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Start an appointment (sets status to InProgress) - only allowed for the owning doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/start")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> StartAppointment(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.InProgress, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Complete an appointment (sets status to Completed) - only allowed for the owning doctor.
    /// This is typically invoked after the doctor has finished the checkup and documentation.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/complete")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> CompleteAppointment(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.Completed, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// End an appointment (sets status to PrescriptionPending) - only allowed for the owning doctor.
    /// This allows prescriptions and reports to be added before final completion.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/end")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> EndAppointment(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            // Move appointment into documentation phase (prescriptions/reports) before final completion
            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.PrescriptionPending, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Accept an appointment (sets status to Accepted) - only allowed for the owning doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/accept")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> AcceptAppointment(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.Accepted, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Reject/cancel an appointment - only allowed for the owning doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/reject")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> RejectAppointment(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.Rejected, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Mark an appointment as requiring follow-up - only allowed for the owning doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/followup")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> MarkFollowUpRequired(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.FollowUpRequired, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as FollowUpRequired", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Get full checkup data for an appointment for the authenticated doctor.
    /// </summary>
    [HttpGet("appointments/{appointmentId}/checkup")]
    [AllowAnonymous]
    public async Task<Result<DoctorCheckup_DTO>> GetCheckup(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Doctor checkup requested but user id claim not present.");
                return Result<DoctorCheckup_DTO>.Failure(new DoctorCheckup_DTO(), "Unauthenticated.");
            }

            _logger.LogInformation("Getting checkup for appointment {AppointmentId} and doctor user {UserId}", appointmentId, userId);
            return await _doctorService.GetCheckupAsync(appointmentId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting checkup for appointment {AppointmentId}", appointmentId);
            return Result<DoctorCheckup_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Update vitals and chronic disease info for an appointment/patient pair for the authenticated doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/vitals")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> UpdateVitals(int appointmentId, [FromBody] DoctorUpdateVitals_DTO dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UpdateVitals requested but user id claim not present.");
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");
            }

            dto.AppointmentId = appointmentId;
            _logger.LogInformation("Updating vitals for appointment {AppointmentId} by doctor user {UserId}", appointmentId, userId);
            return await _doctorService.UpdateVitalsAsync(dto, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating vitals for appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Create a prescription for an appointment for the authenticated doctor.
    /// </summary>
    [HttpPost("appointments/{appointmentId}/prescriptions")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> CreatePrescription(int appointmentId, [FromBody] DoctorCreatePrescription_DTO dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("CreatePrescription requested but user id claim not present.");
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Unauthenticated" }, "Unauthenticated");
            }

            dto.AppointmentId = appointmentId;
            _logger.LogInformation("Creating prescription for appointment {AppointmentId} by doctor user {UserId}", appointmentId, userId);
            return await _doctorService.CreatePrescriptionAsync(dto, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating prescription for appointment {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Get details of a specific appointment by its ID - only allowable for the owning doctor.
    /// </summary>
    [HttpGet("appointments/{appointmentId}")]
    [AllowAnonymous]
    public async Task<Result<CareSync.ApplicationLayer.Contracts.AppointmentsDTOs.AppointmentDetails_DTO>> GetAppointmentDetails(int appointmentId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Doctor appointment details requested but user id claim not present.");
                return Result<CareSync.ApplicationLayer.Contracts.AppointmentsDTOs.AppointmentDetails_DTO>.Failure(new CareSync.ApplicationLayer.Contracts.AppointmentsDTOs.AppointmentDetails_DTO(), "Unauthenticated.");
            }

            _logger.LogInformation("Getting appointment {AppointmentId} details for doctor user {UserId}", appointmentId, userId);
            return await _doctorService.GetAppointmentDetailsAsync(appointmentId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DoctorsController.GetAppointmentDetails for {AppointmentId}", appointmentId);
            return Result<CareSync.ApplicationLayer.Contracts.AppointmentsDTOs.AppointmentDetails_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get all lab reports associated with the authenticated doctor.
    /// </summary>
    [HttpGet("labreports")]
    [AllowAnonymous]
    public async Task<Result<List<DoctorLabReport_DTO>>> GetDoctorLabReports()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Doctor lab reports requested but user id claim not present.");
                return Result<List<DoctorLabReport_DTO>>.Failure(new List<DoctorLabReport_DTO>(), "Unauthenticated.");
            }

            _logger.LogInformation("Getting lab reports for doctor user {UserId}", userId);
            return await _doctorService.GetDoctorLabReportsAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lab reports for current doctor");
            return Result<List<DoctorLabReport_DTO>>.Exception(ex);
        }
    }

    /// <summary>
    /// Get aggregated medical history for a specific patient, scoped to the authenticated doctor.
    /// </summary>
    [HttpGet("patients/{patientId}/medical-history")]
    [AllowAnonymous]
    public async Task<Result<DoctorPatientMedicalHistory_DTO>> GetPatientMedicalHistory(int patientId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Patient medical history requested but user id claim not present.");
                return Result<DoctorPatientMedicalHistory_DTO>.Failure(new DoctorPatientMedicalHistory_DTO(), "Unauthenticated.");
            }

            _logger.LogInformation("Getting medical history for patient {PatientId} and doctor user {UserId}", patientId, userId);
            return await _doctorService.GetPatientMedicalHistoryAsync(patientId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting medical history for patient {PatientId}", patientId);
            return Result<DoctorPatientMedicalHistory_DTO>.Exception(ex);
        }
    }
}