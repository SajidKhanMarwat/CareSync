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

    /// <summary>
    /// Get complete dashboard summary with all widgets data in single call
    /// </summary>
    [HttpGet("dashboard/complete-summary")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<DashboardSummary_DTO>> GetCompleteDashboardSummary()
        => await adminService.GetDashboardSummaryAsync();

    /// <summary>
    /// Get user distribution statistics with month-over-month comparison
    /// </summary>
    [HttpGet("dashboard/user-distribution-stats")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<UserDistributionStats_DTO>> GetUserDistributionStats()
        => await adminService.GetUserDistributionStatsAsync();

    /// <summary>
    /// Get monthly statistics summary with comparisons
    /// </summary>
    [HttpGet("dashboard/monthly-statistics")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<MonthlyStatistics_DTO>> GetMonthlyStatistics()
        => await adminService.GetMonthlyStatsAsync();

    /// <summary>
    /// Get patient registration trends over last 12 months
    /// </summary>
    [HttpGet("dashboard/patient-registration-trends")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<PatientRegistrationTrends_DTO>> GetPatientRegistrationTrends()
        => await adminService.GetPatientRegTrendsAsync();

    /// <summary>
    /// Get appointment status breakdown with percentages
    /// </summary>
    [HttpGet("dashboard/appointment-status-breakdown")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<AppointmentStatusBreakdown_DTO>> GetAppointmentStatusBreakdown()
        => await adminService.GetAppointmentStatusAsync();

    /// <summary>
    /// Get today's appointments detailed list
    /// </summary>
    [HttpGet("dashboard/todays-appointments-list")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<TodaysAppointmentsList_DTO>> GetTodaysAppointmentsList()
        => await adminService.GetTodaysApptsListAsync();

    /// <summary>
    /// Get all appointments
    /// </summary>
    [HttpGet("appointments/all")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<TodaysAppointmentsList_DTO>> GetAllAppointments()
        => await adminService.GetAllAppointmentsAsync();

    /// <summary>
    /// Get recent lab results list
    /// </summary>
    [HttpGet("dashboard/recent-lab-results")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<RecentLabResults_DTO>> GetRecentLabResults()
        => await adminService.GetRecentLabsAsync();

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
        dto.RequiresPasswordReset = true;  // Require password reset on first login
        return await userService.RegisterNewUserAsync(dto, "doctor");
    }

    /// <summary>
    /// Get comprehensive doctor insights and analytics
    /// </summary>
    [HttpGet("doctors/insights")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<DoctorInsights_DTO>> GetDoctorInsights()
        => await adminService.GetDoctorInsightsAsync();

    /// <summary>
    /// Get doctor performance metrics
    /// </summary>
    /// <param name="topCount">Number of top performers to return</param>
    [HttpGet("doctors/performance")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<DoctorPerformance_DTO>>> GetDoctorPerformance([FromQuery] int topCount = 6)
        => await adminService.GetDoctorPerformanceAsync(topCount);

    /// <summary>
    /// Get specialization distribution statistics
    /// </summary>
    [HttpGet("doctors/specialization-distribution")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<SpecializationDistribution_DTO>>> GetSpecializationDistribution()
        => await adminService.GetSpecializationDistributionAsync();

    /// <summary>
    /// Get all unique specializations from database
    /// </summary>
    [HttpGet("doctors/specializations")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<string>>> GetAllSpecializations()
        => await adminService.GetAllSpecializationsAsync();

    /// <summary>
    /// Get doctor availability overview
    /// </summary>
    [HttpGet("doctors/availability-overview")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<DoctorAvailabilityOverview_DTO>> GetDoctorAvailabilityOverview()
        => await adminService.GetDoctorAvailabilityOverviewAsync();

    /// <summary>
    /// Get doctor workload statistics
    /// </summary>
    [HttpGet("doctors/workload")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<DoctorWorkload_DTO>>> GetDoctorWorkload()
        => await adminService.GetDoctorWorkloadAsync();

    /// <summary>
    /// Get doctors for grid display with extended details
    /// </summary>
    /// <param name="specialization">Filter by specialization</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    [HttpGet("doctors/grid")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<DoctorGridItem_DTO>>> GetDoctorGridData(
        [FromQuery] string? specialization = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => await adminService.GetDoctorGridDataAsync(specialization, isActive, page, pageSize);

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
        dto.RequiresPasswordReset = true;  // Require password reset on first login
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
        dto.RequiresPasswordReset = true;  // Require password reset on first login
        return await userService.RegisterNewUserAsync(dto, "lab");
    }

    #endregion
}
