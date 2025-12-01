using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Services.EntitiesServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSync.ApplicationLayer.Contracts.LabDTOs;

namespace CareSync.API.Controllers;

/// <summary>
/// Admin controller for dashboard, user management, and administrative operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController(IAdminService adminService, IUserService userService, IUserManagementService userManagementService, ILogger<AdminController> logger) : ControllerBase
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
    /// Toggle doctor active status
    /// </summary>
    [HttpPost("doctors/toggle-status")]
    public async Task<Result<GeneralResponse>> ToggleDoctorStatus([FromBody] ToggleDoctorStatusRequest request)
        => await adminService.ToggleDoctorStatusAsync(request.UserId, request.IsActive);

    /// <summary>
    /// Get doctor profile with complete details
    /// </summary>
    [HttpGet("doctor-profile/{userId}")]
    public async Task<Result<DoctorProfile_DTO>> GetDoctorProfile(string userId)
        => await adminService.GetDoctorProfileAsync(userId);

    /// <summary>
    /// Update doctor information
    /// </summary>
    [HttpPut("doctor/{userId}")]
    public async Task<Result<GeneralResponse>> UpdateDoctor(string userId, [FromBody] UpdateDoctor_DTO updateDto)
        => await adminService.UpdateDoctorAsync(userId, updateDto);

    /// <summary>
    /// Get doctor schedule
    /// </summary>
    [HttpGet("doctor-schedule/{userId}")]
    public async Task<Result<DoctorSchedule_DTO>> GetDoctorSchedule(string userId)
        => await adminService.GetDoctorScheduleAsync(userId);

    /// <summary>
    /// Get patients treated by a specific doctor
    /// </summary>
    [HttpGet("doctor-patients/{userId}")]
    public async Task<Result<List<PatientList_DTO>>> GetDoctorPatients(string userId)
        => await adminService.GetDoctorPatientsAsync(userId);

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
    /// Comprehensive patient search with multiple filters
    /// </summary>
    [HttpPost("patients/search/comprehensive")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<PatientSearchResult_DTO>> SearchPatientsComprehensive([FromBody] PatientSearchRequest_DTO request)
    {
        if (!ModelState.IsValid)
            return Result<PatientSearchResult_DTO>.Failure(
                null!,
                "Invalid search parameters",
                System.Net.HttpStatusCode.BadRequest);

        return await adminService.SearchPatientsComprehensiveAsync(request);
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

    /// <summary>
    /// Toggle patient active status
    /// </summary>
    [HttpPatch("patients/{userId}/toggle-status")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> TogglePatientStatus(string userId, [FromQuery] bool isActive)
    {
        logger.LogInformation("Toggling patient status: UserId={UserId}, IsActive={IsActive}", userId, isActive);
        return await adminService.TogglePatientStatusAsync(userId, isActive);
    }

    /// <summary>
    /// Get patient age distribution for charts
    /// </summary>
    [HttpGet("patients/age-distribution")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<PatientAgeDistribution_DTO>> GetPatientAgeDistribution()
        => await adminService.GetPatientAgeDistributionAsync();

    /// <summary>
    /// Get patient demographics for charts
    /// </summary>
    [HttpGet("patients/demographics")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<PatientDemographics_DTO>> GetPatientDemographics()
        => await adminService.GetPatientDemographicsAsync();

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
    /// Register lab staff (Lab or Lab Assistant) - admin-initiated
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

        // Determine role name from RoleType enum
        var roleName = dto.RoleType == CareSync.Shared.Enums.RoleType.Lab ? "lab" : "labassistant";
        
        logger.LogInformation("Admin registering new lab staff: {Email} with role {Role}", dto.Email, roleName);
        dto.RequiresPasswordReset = true;  // Require password reset on first login
        return await userService.RegisterNewUserAsync(dto, roleName);
    }

    /// <summary>
    /// Get all laboratories
    /// </summary>
    [HttpGet("labs")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<List<LabListDTO>>> GetAllLabs()
    {
        logger.LogInformation("Admin requesting all laboratories");
        return await adminService.GetAllLabsAsync();
    }

    #endregion

    #region Patient Management Extended

    /// <summary>
    /// Update patient information
    /// </summary>
    [HttpPut("patients/update")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<GeneralResponse>> UpdatePatient([FromBody] UserPatientProfileUpdate_DTO updateDto)
    {
        if (!ModelState.IsValid)
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Invalid update data" },
                "Validation failed",
                System.Net.HttpStatusCode.BadRequest);

        logger.LogInformation("Updating patient: {UserId}", updateDto.UserId);
        return await adminService.UpdatePatientAsync(updateDto);
    }

    /// <summary>
    /// Delete patient (soft delete)
    /// </summary>
    [HttpDelete("patients/{userId}/{patientId}")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<GeneralResponse>> DeletePatient(string userId, int patientId)
    {
        logger.LogInformation("Deleting patient: UserId={UserId}, PatientId={PatientId}", userId, patientId);
        return await adminService.DeletePatientAsync(userId, patientId);
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("patients/{patientId}")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<PatientList_DTO>> GetPatientById(int patientId)
    {
        logger.LogInformation("Getting patient by ID: {PatientId}", patientId);
        return await adminService.GetPatientByIdAsync(patientId);
    }

    /// <summary>
    /// Get comprehensive patient profile
    /// </summary>
    [HttpGet("patients/{patientId}/profile")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<PatientProfile_DTO>> GetPatientProfile(int patientId)
    {
        logger.LogInformation("Getting patient profile: {PatientId}", patientId);
        return await adminService.GetPatientProfileAsync(patientId);
    }

    #endregion

    #region User Management

    /// <summary>
    /// Get user statistics for dashboard cards
    /// </summary>
    [HttpGet("users/statistics")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<UserStatistics_DTO>> GetUserStatistics()
    {
        logger.LogInformation("Getting user statistics");
        return await userManagementService.GetUserStatisticsAsync();
    }

    /// <summary>
    /// Get all users with filters and pagination
    /// </summary>
    [HttpPost("users/list")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<PagedResult<UserList_DTO>>> GetAllUsers([FromBody] UserFilter_DTO filter)
    {
        logger.LogInformation("Getting all users with filters");
        return await userManagementService.GetAllUsersAsync(filter);
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    [HttpGet("users/{userId}")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<UserDetail_DTO>> GetUserById(string userId)
    {
        logger.LogInformation("Getting user details for: {UserId}", userId);
        return await userManagementService.GetUserByIdAsync(userId);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost("users")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> CreateUser([FromBody] CreateUpdateUser_DTO dto)
    {
        logger.LogInformation("Creating new user: {Email}", dto.Email);
        return await userManagementService.CreateUserAsync(dto);
    }

    /// <summary>
    /// Update user information
    /// </summary>
    [HttpPut("users/{userId}")]
    public async Task<Result<GeneralResponse>> UpdateUser(string userId, [FromBody] CreateUpdateUser_DTO dto)
    {
        logger.LogInformation("Updating user: {UserId}", userId);
        return await userManagementService.UpdateUserAsync(userId, dto);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("users/{userId}")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> DeleteUser(string userId)
    {
        logger.LogInformation("Deleting user: {UserId}", userId);
        return await userManagementService.DeleteUserAsync(userId);
    }

    /// <summary>
    /// Toggle user active status
    /// </summary>
    [HttpPatch("users/{userId}/toggle-status")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> ToggleUserStatus(string userId, [FromQuery] bool isActive)
    {
        logger.LogInformation("Toggling user status: {UserId} to {IsActive}", userId, isActive);
        return await userManagementService.ToggleUserStatusAsync(userId, isActive);
    }

    /// <summary>
    /// Suspend a user
    /// </summary>
    [HttpPost("users/{userId}/suspend")]
    public async Task<Result<GeneralResponse>> SuspendUser(string userId, [FromBody] string reason)
    {
        logger.LogInformation("Suspending user: {UserId}", userId);
        return await userManagementService.SuspendUserAsync(userId, reason);
    }

    /// <summary>
    /// Reset user password (admin action)
    /// </summary>
    [HttpPost("users/reset-password")]
    public async Task<Result<GeneralResponse>> ResetUserPassword([FromBody] AdminPasswordReset_DTO dto)
    {
        logger.LogInformation("Resetting password for user: {UserId}", dto.UserId);
        return await userManagementService.ResetPasswordAsync(dto);
    }

    /// <summary>
    /// Perform bulk actions on multiple users
    /// </summary>
    [HttpPost("users/bulk-action")]
    public async Task<Result<GeneralResponse>> BulkUserAction([FromBody] BulkUserAction_DTO dto)
    {
        logger.LogInformation("Performing bulk action: {Action} on {Count} users", dto.Action, dto.UserIds?.Count ?? 0);
        return await userManagementService.BulkActionAsync(dto);
    }

    /// <summary>
    /// Get user activities
    /// </summary>
    [HttpGet("users/activities")]
    public async Task<Result<List<UserActivity_DTO>>> GetUserActivities()
    {
        logger.LogInformation("Getting user activities");
        return await userManagementService.GetUserActivitiesAsync();
    }

    /// <summary>
    /// Update user permissions
    /// </summary>
    [HttpPost("users/permissions")]
    public async Task<Result<GeneralResponse>> UpdateUserPermissions([FromBody] UserPermission_DTO dto)
    {
        logger.LogInformation("Updating permissions for user: {UserId}", dto.UserId);
        return await userManagementService.UpdateUserPermissionsAsync(dto);
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    [HttpGet("users/departments")]
    public async Task<Result<List<string>>> GetDepartments()
    {
        logger.LogInformation("Getting all departments");
        return await userManagementService.GetDepartmentsAsync();
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    [HttpGet("users/roles")]
    public async Task<Result<List<string>>> GetRoles()
    {
        logger.LogInformation("Getting all roles");
        return await userManagementService.GetRolesAsync();
    }

    #endregion
}
