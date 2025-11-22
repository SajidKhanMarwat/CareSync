using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using System.Text.Json;

namespace CareSync.Services;

/// <summary>
/// Service for calling Patient API endpoints
/// </summary>
public class PatientApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PatientApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PatientApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<PatientApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Get patient dashboard data from API
    /// </summary>
    /// <returns>Patient dashboard DTO</returns>
    public async Task<Result<PatientDashboard_DTO>> GetPatientDashboardAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("patients/dashboard");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<PatientDashboard_DTO>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved patient dashboard from API");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<PatientDashboard_DTO>.Failure(
                new PatientDashboard_DTO(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling patient dashboard API");
            return Result<PatientDashboard_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get patient dashboard by user ID (for Admin/Doctor use)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Patient dashboard DTO</returns>
    public async Task<Result<PatientDashboard_DTO>> GetPatientDashboardByUserIdAsync(string userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"patients/dashboard/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<PatientDashboard_DTO>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved patient dashboard for user: {userId}");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<PatientDashboard_DTO>.Failure(
                new PatientDashboard_DTO(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling patient dashboard API for user: {userId}");
            return Result<PatientDashboard_DTO>.Exception(ex);
        }
    }
}
