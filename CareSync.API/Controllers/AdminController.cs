using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

/// <summary>
/// Admin controller for dashboard, user management, and administrative operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService adminService, IUserService userService, ILogger<AdminController> logger) : ControllerBase
{
    #region Dashboard & Analytics

    /// <summary>
    /// Get dashboard summary statistics (appointments, doctors, patients with percentages)
    /// </summary>
    [HttpGet("dashboard/stats")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GetFirstRowCardsData_DTO>> GetDashboardStats()
        => await adminService.GetDashboardStatsAsync();

    /// <summary>
    /// Get urgent items requiring admin attention
    /// </summary>
    [HttpGet("dashboard/urgent-items")]
    public async Task<Result<List<UrgentItem_DTO>>> GetUrgentItems()
        => await adminService.GetUrgentItemsAsync();

    /// <summary>
    /// Get today's performance metrics
    /// </summary>
    [HttpGet("dashboard/today-performance")]
    public async Task<Result<TodayPerformance_DTO>> GetTodayPerformance()
        => await adminService.GetTodayPerformanceAsync();

    /// <summary>
    /// Get user distribution across all roles
    /// </summary>
    [HttpGet("dashboard/user-distribution")]
    public async Task<Result<UserDistribution_DTO>> GetUserDistribution()
        => await adminService.GetUserDistributionAsync();

    /// <summary>
    /// Get patient registration trends for last 6 months
    /// </summary>
    [HttpGet("dashboard/registration-trends")]
    public async Task<Result<RegistrationTrends_DTO>> GetRegistrationTrends()
        => await adminService.GetRegistrationTrendsAsync();

    /// <summary>
    /// Get appointment status distribution chart data
    /// </summary>
    [HttpGet("dashboard/appointment-status-chart")]
    public async Task<Result<AppointmentStatusChart_DTO>> GetAppointmentStatusChart()
        => await adminService.GetAppointmentStatusChartAsync();

    /// <summary>
    /// Get today's appointments with details
    /// </summary>
    [HttpGet("dashboard/todays-appointments")]
    public async Task<Result<List<TodayAppointment_DTO>>> GetTodaysAppointments()
        => await adminService.GetTodaysAppointmentsAsync();

    /// <summary>
    /// Get doctor availability status for dashboard
    /// </summary>
    [HttpGet("dashboard/doctor-availability")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<DoctorAvailabilitySummary_DTO>> GetDoctorAvailability()
        => await adminService.GetDoctorAvailabilityAsync();

    /// <summary>
    /// Get today's performance metrics (enhanced version with more details)
    /// </summary>
    [HttpGet("dashboard/today-performance-metrics")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<TodayPerformanceMetrics_DTO>> GetTodayPerformanceMetrics()
        => await adminService.GetTodayPerformanceMetricsAsync();

    #endregion

    #region Doctor Management

    /// <summary>
    /// Get all doctors with optional filtering
    /// </summary>
    /// <param name="specialization">Filter by specialization</param>
    /// <param name="isActive">Filter by active status</param>
    [HttpGet("doctors")]
    public async Task<Result<List<DoctorList_DTO>>> GetAllDoctors(
        [FromQuery] string? specialization = null,
        [FromQuery] bool? isActive = null)
        => await adminService.GetAllDoctorsAsync(specialization, isActive);

    /// <summary>
    /// Get doctor statistics summary
    /// </summary>
    [HttpGet("doctors/stats")]
    public async Task<Result<DoctorStats_DTO>> GetDoctorStats()
        => await adminService.GetDoctorStatsAsync();

    /// <summary>
    /// Register doctor (admin-initiated)
    /// </summary>
    [HttpPost("doctor-registration")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> RegisterDoctor([FromBody] UserRegisteration_DTO dto)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid registration data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        logger.LogInformation("Admin registering new doctor: {Email}", dto.Email);
        return await userService.RegisterNewUserAsync(dto, "doctor");
    }

    #endregion

    #region Patient Management

    /// <summary>
    /// Get all patients with optional filtering
    /// </summary>
    /// <param name="bloodGroup">Filter by blood group</param>
    /// <param name="isActive">Filter by active status</param>
    [HttpGet("patients")]
    public async Task<Result<List<PatientList_DTO>>> GetAllPatients(
        [FromQuery] string? bloodGroup = null,
        [FromQuery] bool? isActive = null)
        => await adminService.GetAllPatientsAsync(bloodGroup, isActive);

    /// <summary>
    /// Get patient statistics summary
    /// </summary>
    [HttpGet("patients/stats")]
    public async Task<Result<PatientStats_DTO>> GetPatientStats()
        => await adminService.GetPatientStatsAsync();

    /// <summary>
    /// Search patients by name, email, or phone number
    /// </summary>
    [HttpGet("patients/search")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<PatientSearch_DTO>>> SearchPatients([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<List<PatientSearch_DTO>>.Failure(
                new List<PatientSearch_DTO>(),
                "Search term is required",
                System.Net.HttpStatusCode.BadRequest);

        return await adminService.SearchPatientsAsync(searchTerm);
    }

    /// <summary>
    /// Register patient (admin-initiated)
    /// </summary>
    [HttpPost("patient-registration")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> RegisterPatient([FromBody] UserRegisteration_DTO dto)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid registration data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        logger.LogInformation("Admin registering new patient: {Email}", dto.Email);
        return await userService.RegisterNewUserAsync(dto, "patient");
    }

    #endregion

    #region Appointment Management

    /// <summary>
    /// Create new appointment
    /// </summary>
    [HttpPost("appointments")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> CreateAppointment([FromBody] AddAppointment_DTO appointment)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid appointment data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        return await adminService.CreateAppointmentAsync(appointment);
    }

    /// <summary>
    /// Create appointment with quick patient registration
    /// </summary>
    [HttpPost("appointments/quick-patient")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> CreateAppointmentWithQuickPatient(
        [FromBody] AddAppointmentWithQuickPatient_DTO input)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        return await adminService.CreateAppointmentWithQuickPatientAsync(input);
    }

    #endregion

    #region Lab Registration

    /// <summary>
    /// Register lab (admin-initiated)
    /// </summary>
    [HttpPost("lab-registration")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> RegisterLab([FromBody] UserRegisteration_DTO dto)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid registration data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        logger.LogInformation("Admin registering new lab: {Email}", dto.Email);
        return await userService.RegisterNewUserAsync(dto, "lab");
    }

    #endregion
}
