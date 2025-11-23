using System.Net.Http.Json;
using System.Text.Json;

namespace CareSync.Services;

/// <summary>
/// Service to handle API calls to the Admin endpoints
/// </summary>
public class AdminApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AdminApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AdminApiService(IHttpClientFactory httpClientFactory, ILogger<AdminApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient("ApiClient");
    }

    #region Dashboard APIs

    public async Task<T?> GetDashboardStatsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/stats");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return default;
        }
    }


    public async Task<T?> GetUrgentItemsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/urgent-items");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting urgent items");
            return default;
        }
    }

    public async Task<T?> GetTodayPerformanceAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/today-performance");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today performance");
            return default;
        }
    }

    public async Task<T?> GetRegistrationTrendsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/registration-trends");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting registration trends");
            return default;
        }
    }

    public async Task<T?> GetAppointmentStatusChartAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/appointment-status-chart");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment status chart");
            return default;
        }
    }

    public async Task<T?> GetTodaysAppointmentsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/todays-appointments");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments");
            return default;
        }
    }

    #endregion

    #region Doctor Management APIs

    public async Task<T?> GetAllDoctorsAsync<T>(string? specialization = null, bool? isActive = null)
    {
        try
        {
            var client = CreateClient();
            var query = new List<string>();
            if (specialization != null) query.Add($"specialization={Uri.EscapeDataString(specialization)}");
            if (isActive.HasValue) query.Add($"isActive={isActive.Value}");
            
            var queryString = query.Any() ? "?" + string.Join("&", query) : "";
            var response = await client.GetAsync($"Admin/doctors{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors");
            return default;
        }
    }

    public async Task<T?> GetDoctorStatsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/doctors/stats");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor stats");
            return default;
        }
    }

    public async Task<T?> GetDoctorInsightsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/doctors/insights");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor insights");
            return default;
        }
    }

    public async Task<T?> GetDoctorPerformanceAsync<T>(int topCount = 6)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"Admin/doctors/performance?topCount={topCount}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor performance");
            return default;
        }
    }

    public async Task<T?> GetSpecializationDistributionAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/doctors/specialization-distribution");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting specialization distribution");
            return default;
        }
    }

    public async Task<T?> GetAllSpecializationsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/doctors/specializations");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all specializations");
            return default;
        }
    }

    public async Task<T?> GetDoctorAvailabilityOverviewAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/doctors/availability-overview");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor availability overview");
            return default;
        }
    }

    public async Task<T?> GetDoctorGridDataAsync<T>(string? specialization = null, bool? isActive = null, int page = 1, int pageSize = 10)
    {
        try
        {
            var client = CreateClient();
            var query = new List<string>();
            if (specialization != null) query.Add($"specialization={Uri.EscapeDataString(specialization)}");
            if (isActive.HasValue) query.Add($"isActive={isActive.Value}");
            query.Add($"page={page}");
            query.Add($"pageSize={pageSize}");
            
            var queryString = query.Any() ? "?" + string.Join("&", query) : "";
            var response = await client.GetAsync($"Admin/doctors/grid{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor grid data");
            return default;
        }
    }

    public async Task<T?> ToggleDoctorStatusAsync<T>(string userId, bool isActive)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PatchAsync($"Admin/doctors/{userId}/toggle-status?isActive={isActive}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling doctor status");
            return default;
        }
    }

    #endregion

    #region Patient Management APIs

    public async Task<T?> GetAllPatientsAsync<T>(string? bloodGroup = null, bool? isActive = null)
    {
        try
        {
            var client = CreateClient();
            var query = new List<string>();
            if (bloodGroup != null) query.Add($"bloodGroup={Uri.EscapeDataString(bloodGroup)}");
            if (isActive.HasValue) query.Add($"isActive={isActive.Value}");
            
            var queryString = query.Any() ? "?" + string.Join("&", query) : "";
            var response = await client.GetAsync($"Admin/patients{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients");
            return default;
        }
    }

    public async Task<T?> GetPatientStatsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/patients/stats");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient stats");
            return default;
        }
    }

    public async Task<T?> SearchPatientsAsync<T>(string searchTerm)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"Admin/patients/search?searchTerm={Uri.EscapeDataString(searchTerm)}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients");
            return default;
        }
    }

    public async Task<T?> TogglePatientStatusAsync<T>(string userId, bool isActive)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PatchAsync($"Admin/patients/{userId}/toggle-status?isActive={isActive}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling patient status");
            return default;
        }
    }

    #endregion

    #region Appointment Management APIs

    public async Task<T?> CreateAppointmentAsync<T>(object appointmentData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("Admin/appointments", appointmentData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return default;
        }
    }

    public async Task<T?> CreateAppointmentWithQuickPatientAsync<T>(object appointmentData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("Admin/appointments/quick-patient", appointmentData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment with quick patient");
            return default;
        }
    }

    #endregion

    #region User Registration APIs

    public async Task<T?> RegisterPatientAsync<T>(object patientData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("Admin/patient-registration", patientData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient");
            return default;
        }
    }

    public async Task<T?> RegisterDoctorAsync<T>(object doctorData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("Admin/doctor-registration", doctorData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            return default;
        }
    }

    public async Task<T?> RegisterLabAsync<T>(object labData)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("Admin/lab-registration", labData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering lab");
            return default;
        }
    }

    public async Task<T?> GetDoctorAvailabilityAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/doctor-availability");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor availability");
            return default;
        }
    }

    public async Task<T?> GetTodayPerformanceMetricsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/today-performance-metrics");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's performance metrics");
            return default;
        }
    }

    public async Task<T?> GetUserDistributionAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/user-distribution");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user distribution");
            return default;
        }
    }

    public async Task<T?> GetUserDistributionStatsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/user-distribution-stats");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user distribution stats");
            return default;
        }
    }

    public async Task<T?> GetMonthlyStatisticsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/monthly-statistics");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly statistics");
            return default;
        }
    }

    public async Task<T?> GetPatientRegistrationTrendsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/patient-registration-trends");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient registration trends");
            return default;
        }
    }

    public async Task<T?> GetAppointmentStatusBreakdownAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/appointment-status-breakdown");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment status breakdown");
            return default;
        }
    }

    public async Task<T?> GetTodaysAppointmentsListAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/todays-appointments-list");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments list");
            return default;
        }
    }

    public async Task<T?> GetAllAppointmentsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/appointments/all");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all appointments");
            return default;
        }
    }

    public async Task<T?> GetRecentLabResultsAsync<T>()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("Admin/dashboard/recent-lab-results");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent lab results");
            return default;
        }
    }

    #endregion
}
