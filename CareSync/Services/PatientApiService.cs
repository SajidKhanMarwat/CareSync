using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using System.Text;
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

    #region Appointment Booking

    /// <summary>
    /// Get all available doctors
    /// </summary>
    /// <param name="specialization">Optional specialization filter</param>
    /// <returns>List of available doctors</returns>
    public async Task<Result<List<DoctorBooking_DTO>>> GetAvailableDoctorsAsync(string? specialization = null)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var url = "patients/doctors/available";
            
            if (!string.IsNullOrEmpty(specialization))
            {
                url += $"?specialization={Uri.EscapeDataString(specialization)}";
            }

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<List<DoctorBooking_DTO>>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved {result.Data?.Count ?? 0} available doctors");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<List<DoctorBooking_DTO>>.Failure(
                new List<DoctorBooking_DTO>(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling available doctors API");
            return Result<List<DoctorBooking_DTO>>.Exception(ex);
        }
    }

    /// <summary>
    /// Get doctor details by ID
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>Doctor details</returns>
    public async Task<Result<DoctorBooking_DTO>> GetDoctorByIdAsync(int doctorId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"patients/doctors/{doctorId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<DoctorBooking_DTO>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved doctor details for ID: {doctorId}");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<DoctorBooking_DTO>.Failure(
                null!,
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling doctor details API for ID: {doctorId}");
            return Result<DoctorBooking_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get available time slots for a doctor on a specific date
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="date">Date to check</param>
    /// <returns>List of time slots</returns>
    public async Task<Result<List<DoctorTimeSlot_DTO>>> GetDoctorTimeSlotsAsync(int doctorId, DateTime date)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var dateStr = date.ToString("yyyy-MM-dd");
            var response = await client.GetAsync($"patients/doctors/{doctorId}/timeslots?date={dateStr}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<List<DoctorTimeSlot_DTO>>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved time slots for doctor {doctorId} on {dateStr}");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<List<DoctorTimeSlot_DTO>>.Failure(
                new List<DoctorTimeSlot_DTO>(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling time slots API for doctor {doctorId}");
            return Result<List<DoctorTimeSlot_DTO>>.Exception(ex);
        }
    }

    /// <summary>
    /// Book an appointment
    /// </summary>
    /// <param name="request">Booking request details</param>
    /// <returns>Booking result</returns>
    public async Task<Result<GeneralResponse>> BookAppointmentAsync(BookAppointmentRequest_DTO request)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("patients/appointments/book", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully booked appointment");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to book appointment" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling book appointment API");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    #endregion
}
