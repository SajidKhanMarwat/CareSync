using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
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

    /// <summary>
    /// Get doctor profile with complete details
    /// </summary>
    Task<Result<DoctorProfile_DTO>> GetDoctorProfileAsync(string userId);

    /// <summary>
    /// Update doctor information
    /// </summary>
    Task<Result<GeneralResponse>> UpdateDoctorAsync(string userId, UpdateDoctor_DTO updateDto);

    /// <summary>
    /// Get doctor schedule
    /// </summary>
    Task<Result<DoctorSchedule_DTO>> GetDoctorScheduleAsync(string userId);

    /// <summary>
    /// Get patients treated by a specific doctor
    /// </summary>
    Task<Result<List<PatientList_DTO>>> GetDoctorPatientsAsync(string userId);

    /// <summary>
    /// Get comprehensive doctor insights and analytics
    /// </summary>
    Task<Result<DoctorInsights_DTO>> GetDoctorInsightsAsync();

    /// <summary>
    /// Get doctor performance metrics
    /// </summary>
    Task<Result<List<DoctorPerformance_DTO>>> GetDoctorPerformanceAsync(int topCount = 6);

    /// <summary>
    /// Get specialization distribution statistics
    /// </summary>
    Task<Result<List<SpecializationDistribution_DTO>>> GetSpecializationDistributionAsync();

    /// <summary>
    /// Get all unique specializations from database
    /// </summary>
    Task<Result<List<string>>> GetAllSpecializationsAsync();

    /// <summary>
    /// Get doctor availability overview
    /// </summary>
    Task<Result<DoctorAvailabilityOverview_DTO>> GetDoctorAvailabilityOverviewAsync();

    /// <summary>
    /// Get doctor workload statistics
    /// </summary>
    Task<Result<List<DoctorWorkload_DTO>>> GetDoctorWorkloadAsync();

    /// <summary>
    /// Get doctors for grid display with extended details
    /// </summary>
    Task<Result<List<DoctorGridItem_DTO>>> GetDoctorGridDataAsync(string? specialization = null, bool? isActive = null, int page = 1, int pageSize = 10);

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
    /// Get comprehensive patient profile with all medical information
    /// </summary>
    Task<Result<PatientProfile_DTO>> GetPatientProfileAsync(int patientId);

    /// <summary>
    /// Search patients by name, email, or phone number
    /// </summary>
    Task<Result<List<PatientSearch_DTO>>> SearchPatientsAsync(string searchTerm);
    
    /// <summary>
    /// Comprehensive patient search with multiple filters and pagination
    /// </summary>
    Task<Result<PatientSearchResult_DTO>> SearchPatientsComprehensiveAsync(PatientSearchRequest_DTO request);

    /// <summary>
    /// Toggle patient active status
    /// </summary>
    Task<Result<GeneralResponse>> TogglePatientStatusAsync(string userId, bool isActive);
    
    /// <summary>
    /// Update patient information
    /// </summary>
    Task<Result<GeneralResponse>> UpdatePatientAsync(UserPatientProfileUpdate_DTO updateDto);
    
    /// <summary>
    /// Delete patient (soft delete)
    /// </summary>
    Task<Result<GeneralResponse>> DeletePatientAsync(string userId, int patientId);
    
    /// <summary>
    /// Get patient registration trends for charts
    /// </summary>
    Task<Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>> GetPatientRegistrationTrendsAsync();
    
    /// <summary>
    /// Get patient age distribution for charts
    /// </summary>
    Task<Result<PatientAgeDistribution_DTO>> GetPatientAgeDistributionAsync();
    
    /// <summary>
    /// Get patient demographics (gender and marital status) for charts
    /// </summary>
    Task<Result<PatientDemographics_DTO>> GetPatientDemographicsAsync();

    // ========== Lab Management ==========
    
    /// <summary>
    /// Get all laboratories
    /// </summary>
    Task<Result<List<LabListDTO>>> GetAllLabsAsync();

    /// <summary>
    /// Get laboratory details by ID
    /// </summary>
    Task<Result<LabDetails_DTO>> GetLabByIdAsync(int labId);

    /// <summary>
    /// Create a new laboratory
    /// </summary>
    Task<Result<GeneralResponse>> CreateLabAsync(CreateLab_DTO dto, string createdBy);

    /// <summary>
    /// Update laboratory information
    /// </summary>
    Task<Result<GeneralResponse>> UpdateLabAsync(UpdateLab_DTO dto, string updatedBy);

    /// <summary>
    /// Delete laboratory (soft delete)
    /// </summary>
    Task<Result<GeneralResponse>> DeleteLabAsync(int labId);

    /// <summary>
    /// Get all services for a specific laboratory
    /// </summary>
    Task<Result<List<LabService_DTO>>> GetLabServicesAsync(int labId);

    /// <summary>
    /// Get all lab services across all laboratories
    /// </summary>
    Task<Result<List<LabService_DTO>>> GetAllLabServicesAsync();

    /// <summary>
    /// Get lab services with pagination and filtering
    /// </summary>
    Task<Result<LabServicesPagedResult_DTO>> GetLabServicesPagedAsync(LabServicesFilter_DTO filter);

    /// <summary>
    /// Create a new lab service
    /// </summary>
    Task<Result<GeneralResponse>> CreateLabServiceAsync(LabService_DTO dto, string createdBy);

    /// <summary>
    /// Update lab service information
    /// </summary>
    Task<Result<GeneralResponse>> UpdateLabServiceAsync(LabService_DTO dto, string updatedBy);

    /// <summary>
    /// Delete lab service (soft delete)
    /// </summary>
    Task<Result<GeneralResponse>> DeleteLabServiceAsync(int serviceId);

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
    Task<Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>> GetPatientRegTrendsAsync();

    /// <summary>
    /// Get appointment status breakdown with percentages
    /// </summary>
    Task<Result<AppointmentStatusBreakdown_DTO>> GetAppointmentStatusAsync();

    /// <summary>
    /// Get today's appointments detailed list
    /// </summary>
    Task<Result<TodaysAppointmentsList_DTO>> GetTodaysApptsListAsync();

    /// <summary>
    /// Get all appointments with pagination support
    /// </summary>
    Task<Result<TodaysAppointmentsList_DTO>> GetAllAppointmentsAsync();

    /// <summary>
    /// Get recent lab results list
    /// </summary>
    Task<Result<RecentLabResults_DTO>> GetRecentLabsAsync();
}
