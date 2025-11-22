using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

/// <summary>
/// Service interface for admin-specific operations including dashboard, user management, and analytics
/// </summary>
public interface IAdminService
{
    // ========== Dashboard & Analytics ==========
    
    /// <summary>
    /// Get dashboard summary statistics (appointments, doctors, patients counts with percentages)
    /// </summary>
    Task<Result<GetFirstRowCardsData_DTO>> GetDashboardStatsAsync();

    /// <summary>
    /// Get complete dashboard summary with all cards, charts, and urgent items
    /// </summary>
    Task<Result<DashboardSummary_DTO>> GetDashboardSummaryAsync();

    /// <summary>
    /// Get urgent items requiring admin attention (pending approvals, critical lab results)
    /// </summary>
    Task<Result<List<UrgentItem_DTO>>> GetUrgentItemsAsync();

    /// <summary>
    /// Get today's performance metrics (completed appointments, lab reports ready, etc.)
    /// </summary>
    Task<Result<TodayPerformance_DTO>> GetTodayPerformanceAsync();

    /// <summary>
    /// Get user distribution across all roles with month-over-month comparison
    /// </summary>
    Task<Result<UserDistribution_DTO>> GetUserDistributionAsync();

    /// <summary>
    /// Get patient registration trends for last 6 months
    /// </summary>
    Task<Result<RegistrationTrends_DTO>> GetRegistrationTrendsAsync();

    /// <summary>
    /// Get appointment status distribution (confirmed, pending, completed, cancelled)
    /// </summary>
    Task<Result<AppointmentStatusChart_DTO>> GetAppointmentStatusChartAsync();

    /// <summary>
    /// Get today's appointments with doctor and patient details
    /// </summary>
    Task<Result<List<TodayAppointment_DTO>>> GetTodaysAppointmentsAsync();

    // ========== Doctor Management ==========
    
    /// <summary>
    /// Get all doctors with optional filtering and pagination
    /// </summary>
    Task<Result<List<DoctorList_DTO>>> GetAllDoctorsAsync(string? specialization = null, bool? isActive = null);

    /// <summary>
    /// Get doctor statistics (total, active, by specialization)
    /// </summary>
    Task<Result<DoctorStats_DTO>> GetDoctorStatsAsync();

    /// <summary>
    /// Get doctor by ID with full details
    /// </summary>
    Task<Result<DoctorList_DTO>> GetDoctorByIdAsync(int doctorId);

    /// <summary>
    /// Toggle doctor active status
    /// </summary>
    Task<Result<GeneralResponse>> ToggleDoctorStatusAsync(string userId, bool isActive);

    // ========== Patient Management ==========
    
    /// <summary>
    /// Get all patients with optional filtering and pagination
    /// </summary>
    Task<Result<List<PatientList_DTO>>> GetAllPatientsAsync(string? bloodGroup = null, bool? isActive = null);

    /// <summary>
    /// Get patient statistics (total, active, by blood group)
    /// </summary>
    Task<Result<PatientStats_DTO>> GetPatientStatsAsync();

    /// <summary>
    /// Get patient by ID with full details
    /// </summary>
    Task<Result<PatientList_DTO>> GetPatientByIdAsync(int patientId);

    /// <summary>
    /// Search patients by name, email, or phone number
    /// </summary>
    Task<Result<List<PatientSearch_DTO>>> SearchPatientsAsync(string searchTerm);

    /// <summary>
    /// Toggle patient active status
    /// </summary>
    Task<Result<GeneralResponse>> TogglePatientStatusAsync(string userId, bool isActive);

    // ========== Appointment Management ==========
    
    /// <summary>
    /// Create new appointment
    /// </summary>
    Task<Result<GeneralResponse>> CreateAppointmentAsync(AddAppointment_DTO appointment);

    /// <summary>
    /// Create appointment with quick patient registration
    /// </summary>
    Task<Result<GeneralResponse>> CreateAppointmentWithQuickPatientAsync(AddAppointmentWithQuickPatient_DTO input);

    /// <summary>
    /// Create patient account without appointment
    /// </summary>
    Task<Result<GeneralResponse>> CreatePatientAccountAsync(CreatePatient_DTO input);

    // ========== Admin User Management ==========
    
    /// <summary>
    /// Get admin user details
    /// </summary>
    Task<Result<AdminUser_DTO>> GetUserAdminAsync(string userId);

    /// <summary>
    /// Update admin profile
    /// </summary>
    Task<Result<GeneralResponse>> UpdateUserAdminAsync(UserAdminUpdate_DTO request);

    /// <summary>
    /// Delete/deactivate admin user
    /// </summary>
    Task<Result<GeneralResponse>> DeleteUserAdminAsync(string id);

    // ========== Doctor Availability ==========
    
    /// <summary>
    /// Get doctor availability status for dashboard
    /// </summary>
    Task<Result<DoctorAvailabilitySummary_DTO>> GetDoctorAvailabilityAsync();

    // ========== Today's Performance ==========
    
    /// <summary>
    /// Get today's performance metrics for dashboard
    /// </summary>
    Task<Result<TodayPerformanceMetrics_DTO>> GetTodayPerformanceMetricsAsync();

    // ========== Additional Dashboard Widgets ==========
    
    /// <summary>
    /// Get user distribution statistics by role with month comparison
    /// </summary>
    Task<Result<UserDistributionStats_DTO>> GetUserDistributionStatsAsync();

    /// <summary>
    /// Get monthly statistics summary with comparisons
    /// </summary>
    Task<Result<MonthlyStatistics_DTO>> GetMonthlyStatsAsync();

    /// <summary>
    /// Get patient registration trends over 12 months
    /// </summary>
    Task<Result<PatientRegistrationTrends_DTO>> GetPatientRegTrendsAsync();

    /// <summary>
    /// Get appointment status breakdown with percentages
    /// </summary>
    Task<Result<AppointmentStatusBreakdown_DTO>> GetAppointmentStatusAsync();

    /// <summary>
    /// Get today's appointments detailed list
    /// </summary>
    Task<Result<TodaysAppointmentsList_DTO>> GetTodaysApptsListAsync();

    /// <summary>
    /// Get recent lab results list
    /// </summary>
    Task<Result<RecentLabResults_DTO>> GetRecentLabsAsync();
}
