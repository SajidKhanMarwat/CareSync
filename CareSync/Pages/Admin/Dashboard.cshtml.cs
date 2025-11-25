using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

namespace CareSync.Pages.Admin;

public class DashboardModel : BasePageModel
{
    private readonly ILogger<DashboardModel> _logger;
    private readonly AdminApiService _adminApiService;

    public DashboardModel(ILogger<DashboardModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    // Dashboard Data Properties
    public GetFirstRowCardsData_DTO? DashboardStats { get; set; }
    public List<UrgentItem_DTO> UrgentItems { get; set; } = new();
    public TodayPerformance_DTO? TodayPerformance { get; set; }
    public List<TodayAppointment_DTO> TodaysAppointments { get; set; } = new();
    public UserDistribution_DTO? UserDistribution { get; set; }
    public RegistrationTrends_DTO? RegistrationTrends { get; set; }
    public AppointmentStatusChart_DTO? AppointmentStatusChart { get; set; }
    public DoctorAvailabilitySummary_DTO? DoctorAvailability { get; set; }
    public TodayPerformanceMetrics_DTO? TodayPerformanceMetrics { get; set; }
    
    // Additional Dashboard Widgets
    public UserDistributionStats_DTO? UserDistributionStats { get; set; }
    public MonthlyStatistics_DTO? MonthlyStatistics { get; set; }
    public PatientRegistrationTrends_DTO? PatientRegistrationTrends { get; set; }
    public AppointmentStatusBreakdown_DTO? AppointmentStatusBreakdown { get; set; }
    public TodaysAppointmentsList_DTO? TodaysAppointmentsList { get; set; }
    public RecentLabResults_DTO? RecentLabResults { get; set; }
    
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Check if user is authenticated and has Admin role
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Loading admin dashboard for user {Role}", UserRole);

            // Load dashboard data in parallel for better performance
            var statsTask = _adminApiService.GetDashboardStatsAsync<Result<GetFirstRowCardsData_DTO>>();
            var urgentTask = _adminApiService.GetUrgentItemsAsync<Result<List<UrgentItem_DTO>>>();
            var performanceTask = _adminApiService.GetTodayPerformanceAsync<Result<TodayPerformance_DTO>>();
            var appointmentsTask = _adminApiService.GetTodaysAppointmentsAsync<Result<List<TodayAppointment_DTO>>>();
            var distributionTask = _adminApiService.GetUserDistributionAsync<Result<UserDistribution_DTO>>();
            var trendsTask = _adminApiService.GetRegistrationTrendsAsync<Result<RegistrationTrends_DTO>>();
            var chartTask = _adminApiService.GetAppointmentStatusChartAsync<Result<AppointmentStatusChart_DTO>>();
            var availabilityTask = _adminApiService.GetDoctorAvailabilityAsync<Result<DoctorAvailabilitySummary_DTO>>();
            var performanceMetricsTask = _adminApiService.GetTodayPerformanceMetricsAsync<Result<TodayPerformanceMetrics_DTO>>();
            
            // Additional dashboard widgets
            var userDistStatsTask = _adminApiService.GetUserDistributionStatsAsync<Result<UserDistributionStats_DTO>>();
            var monthlyStatsTask = _adminApiService.GetMonthlyStatisticsAsync<Result<MonthlyStatistics_DTO>>();
            var patientTrendsTask = _adminApiService.GetPatientRegistrationTrendsAsync<Result<PatientRegistrationTrends_DTO>>();
            var apptStatusTask = _adminApiService.GetAppointmentStatusBreakdownAsync<Result<AppointmentStatusBreakdown_DTO>>();
            var todaysApptListTask = _adminApiService.GetTodaysAppointmentsListAsync<Result<TodaysAppointmentsList_DTO>>();
            var recentLabsTask = _adminApiService.GetRecentLabResultsAsync<Result<RecentLabResults_DTO>>();

            await Task.WhenAll(statsTask, urgentTask, performanceTask, appointmentsTask, 
                              distributionTask, trendsTask, chartTask, availabilityTask, performanceMetricsTask,
                              userDistStatsTask, monthlyStatsTask, patientTrendsTask, apptStatusTask,
                              todaysApptListTask, recentLabsTask);

            // Extract data from results
            var statsResult = await statsTask;
            if (statsResult?.IsSuccess == true && statsResult.Data != null)
                DashboardStats = statsResult.Data;

            var urgentResult = await urgentTask;
            if (urgentResult?.IsSuccess == true && urgentResult.Data != null)
                UrgentItems = urgentResult.Data;

            var performanceResult = await performanceTask;
            if (performanceResult?.IsSuccess == true && performanceResult.Data != null)
                TodayPerformance = performanceResult.Data;

            var appointmentsResult = await appointmentsTask;
            if (appointmentsResult?.IsSuccess == true && appointmentsResult.Data != null)
                TodaysAppointments = appointmentsResult.Data;

            var distributionResult = await distributionTask;
            if (distributionResult?.IsSuccess == true && distributionResult.Data != null)
                UserDistribution = distributionResult.Data;

            var trendsResult = await trendsTask;
            if (trendsResult?.IsSuccess == true && trendsResult.Data != null)
                RegistrationTrends = trendsResult.Data;

            var chartResult = await chartTask;
            if (chartResult?.IsSuccess == true && chartResult.Data != null)
                AppointmentStatusChart = chartResult.Data;

            var availabilityResult = await availabilityTask;
            if (availabilityResult?.IsSuccess == true && availabilityResult.Data != null)
                DoctorAvailability = availabilityResult.Data;

            var performanceMetricsResult = await performanceMetricsTask;
            if (performanceMetricsResult?.IsSuccess == true && performanceMetricsResult.Data != null)
                TodayPerformanceMetrics = performanceMetricsResult.Data;

            // Extract additional dashboard widget data
            var userDistStatsResult = await userDistStatsTask;
            if (userDistStatsResult?.IsSuccess == true && userDistStatsResult.Data != null)
                UserDistributionStats = userDistStatsResult.Data;

            var monthlyStatsResult = await monthlyStatsTask;
            if (monthlyStatsResult?.IsSuccess == true && monthlyStatsResult.Data != null)
                MonthlyStatistics = monthlyStatsResult.Data;

            var patientTrendsResult = await patientTrendsTask;
            if (patientTrendsResult?.IsSuccess == true && patientTrendsResult.Data != null)
                PatientRegistrationTrends = patientTrendsResult.Data;

            var apptStatusResult = await apptStatusTask;
            if (apptStatusResult?.IsSuccess == true && apptStatusResult.Data != null)
                AppointmentStatusBreakdown = apptStatusResult.Data;

            var todaysApptListResult = await todaysApptListTask;
            if (todaysApptListResult?.IsSuccess == true && todaysApptListResult.Data != null)
                TodaysAppointmentsList = todaysApptListResult.Data;

            var recentLabsResult = await recentLabsTask;
            if (recentLabsResult?.IsSuccess == true && recentLabsResult.Data != null)
                RecentLabResults = recentLabsResult.Data;

            _logger.LogInformation("Dashboard data loaded successfully");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            ErrorMessage = "Failed to load dashboard data. Please try again.";
            HasError = true;
            return Page();
        }
    }
}
