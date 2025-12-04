using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using System.Text.Json;

namespace CareSync.Services;

public class DoctorApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DoctorApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DoctorApiService(IHttpClientFactory httpClientFactory, ILogger<DoctorApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ApiClient");

    /// <summary>
    /// Get the doctor dashboard for the authenticated doctor.
    /// </summary>
    public async Task<Result<DoctorDashboard_DTO>?> GetDashboardAsync()
    {
        try
        {
            var client = CreateClient();
            var resp = await client.GetAsync("doctors/dashboard");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Doctor dashboard API returned {Status}", resp.StatusCode);
                return Result<DoctorDashboard_DTO>.Failure(new DoctorDashboard_DTO(), $"API returned {resp.StatusCode}");
            }

            var content = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<DoctorDashboard_DTO>>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling doctor dashboard API");
            return Result<DoctorDashboard_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Fetch appointments for the authenticated doctor.
    /// Returns TodaysAppointmentsList_DTO wrapped in Result.
    /// </summary>
    public async Task<Result<TodaysAppointmentsList_DTO>?> GetAppointmentsAsync()
    {
        try
        {
            var client = CreateClient();
            var resp = await client.GetAsync("doctors/appointments");
            if (!resp.IsSuccessStatusCode)
            {
                var errorBody = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("Doctor appointments API returned {Status}. Body: {Body}", resp.StatusCode, Truncate(errorBody, 1000));
                return Result<TodaysAppointmentsList_DTO>.Failure(new TodaysAppointmentsList_DTO(), $"API returned {resp.StatusCode}");
            }

            var content = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<TodaysAppointmentsList_DTO>>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling doctor appointments API");
            return Result<TodaysAppointmentsList_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Doctor-scoped helper: Get doctor schedule by userId.
    /// This method is dedicated to DoctorApiService (do not call AdminApiService).
    /// Endpoint: GET /api/Admin/doctor-schedule/{userId}
    /// </summary>
    public async Task<T?> GetDoctorScheduleAsync<T>(string userId)
    {
        try
        {
            var client = CreateClient();
            // Correct endpoint for doctor schedule (Admin area)
            var endpoint = $"Admin/doctor-schedule/{Uri.EscapeDataString(userId)}";
            var resp = await client.GetAsync(endpoint);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("GetDoctorScheduleAsync returned {Status} for user {UserId}. Endpoint: {Endpoint}. Body: {Body}", resp.StatusCode, userId, endpoint, Truncate(body, 1000));
                return default;
            }

            var content = await resp.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            if (result != null)
                return result;

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("data", out var dataElem))
                {
                    var inner = dataElem.Deserialize<T>(_jsonOptions);
                    return inner;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract 'data' property when mapping doctor schedule for user {UserId}", userId);
            }

            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDoctorScheduleAsync for user {UserId}", userId);
            return default;
        }
    }

    /// <summary>
    /// Doctor-scoped helper: Get doctor profile by userId.
    /// This method is dedicated to DoctorApiService (do not call AdminApiService).
    /// Endpoint: GET /api/Admin/doctor-profile/{userId}
    /// </summary>
    public async Task<T?> GetDoctorProfileAsync<T>(string userId)
    {
        try
        {
            var client = CreateClient();
            var resp = await client.GetAsync($"Admin/doctor-profile/{Uri.EscapeDataString(userId)}");
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("GetDoctorProfileAsync returned {Status} for user {UserId}. Body: {Body}", resp.StatusCode, userId, Truncate(body, 1000));
                return default;
            }

            var content = await resp.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetDoctorProfileAsync for user {UserId}", userId);
            return default;
        }
    }

    /// <summary>
    /// Ask API to mark appointment as InProgress for the authenticated doctor.
    /// </summary>
    public async Task<Result<GeneralResponse>?> StartAppointmentAsync(int appointmentId)
    {
        try
        {
            var client = CreateClient();
            var resp = await client.PostAsync($"doctors/appointments/{appointmentId}/start", null);
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("StartAppointmentAsync returned {Status}. Body: {Body}", resp.StatusCode, Truncate(content, 1000));
                var failure = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
                return failure ?? Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "API failure" }, $"API {resp.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling StartAppointment API for {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Ask API to mark appointment as Completed for the authenticated doctor.
    /// </summary>
    public async Task<Result<GeneralResponse>?> EndAppointmentAsync(int appointmentId)
    {
        try
        {
            var client = CreateClient();
            var resp = await client.PostAsync($"doctors/appointments/{appointmentId}/end", null);
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("EndAppointmentAsync returned {Status}. Body: {Body}", resp.StatusCode, Truncate(content, 1000));
                var failure = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
                return failure ?? Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "API failure" }, $"API {resp.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling EndAppointment API for {AppointmentId}", appointmentId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Get appointment details by appointment ID.
    /// </summary>
    public async Task<Result<AppointmentDetails_DTO>?> GetAppointmentByIdAsync(int appointmentId)
    {
        try
        {
            var client = CreateClient();
            var resp = await client.GetAsync($"doctors/appointments/{appointmentId}");
            var content = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetAppointmentByIdAsync returned {Status} for id {Id}. Body: {Body}", resp.StatusCode, appointmentId, Truncate(content, 1000));
                return JsonSerializer.Deserialize<Result<AppointmentDetails_DTO>>(content, _jsonOptions) ?? Result<AppointmentDetails_DTO>.Failure(new AppointmentDetails_DTO(), $"API returned {resp.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<Result<AppointmentDetails_DTO>>(content, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GetAppointmentById API for {AppointmentId}", appointmentId);
            return Result<AppointmentDetails_DTO>.Exception(ex);
        }
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= max ? value : value.Substring(0, max) + "...";
    }
}