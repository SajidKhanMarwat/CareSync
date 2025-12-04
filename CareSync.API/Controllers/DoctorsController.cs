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
            var todayAppointments = dashboardResult.Data.TodayAppointments ?? new List<ApplicationLayer.Contracts.DoctorsDTOs.TodayAppointment_DTO>();

            foreach (var t in todayAppointments)
            {
                todayItems.Add(new TodayAppointmentItem
                {
                    AppointmentID = t.Id,
                    PatientID = t.PatientID,
                    PatientName = t.PatientName ?? "Unknown",
                    DoctorID = t.DoctorID,
                    DoctorName = dashboardResult.Data.DoctorName ?? "Unknown",
                    DoctorSpecialization = dashboardResult.Data.Specialization,
                    AppointmentDate = t.AppointmentTime.Date,
                    AppointmentTime = t.AppointmentTime,
                    Status = t.Type ?? string.Empty,
                    AppointmentType = t.Type ?? string.Empty,
                    Reason = t.Diagnosis ?? string.Empty
                });
            }

            var list = new TodaysAppointmentsList_DTO
            {
                Appointments = todayItems.OrderBy(a => a.AppointmentTime).ToList(),
                TotalToday = todayItems.Count(i => i.AppointmentDate == DateTime.UtcNow.Date),
                CompletedToday = todayItems.Count(i => string.Equals(i.Status, "Completed", StringComparison.OrdinalIgnoreCase)),
                PendingToday = todayItems.Count(i => string.Equals(i.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.Equals(i.Status, "Created", StringComparison.OrdinalIgnoreCase))
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
    /// End an appointment (sets status to Completed) - only allowed for the owning doctor.
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

            return await _doctorService.UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus_Enum.Completed, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending appointment {AppointmentId}", appointmentId);
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
}