using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.Shared.Enums;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CareSync.Services;

/// <summary>
/// Service for calling User Management API endpoints
/// </summary>
public class UserManagementApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserManagementApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserManagementApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<UserManagementApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            WriteIndented = true
        };
    }

    /// <summary>
    /// Get user statistics for dashboard cards
    /// </summary>
    public async Task<Result<UserStatistics_DTO>> GetUserStatisticsAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("admin/users/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<UserStatistics_DTO>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved user statistics");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<UserStatistics_DTO>.Failure(
                new UserStatistics_DTO(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling user statistics API");
            return Result<UserStatistics_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get all users with filters and pagination
    /// </summary>
    public async Task<Result<PagedResult<UserList_DTO>>> GetAllUsersAsync(UserFilter_DTO filter)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(filter, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("admin/users/list", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<PagedResult<UserList_DTO>>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved {result.Data?.TotalCount ?? 0} users");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            
            // Log the request details for debugging
            _logger.LogError($"Request failed - Filter: {json}");
            
            return Result<PagedResult<UserList_DTO>>.Failure(
                new PagedResult<UserList_DTO>(),
                $"API request failed with status: {response.StatusCode}. Error: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling get all users API");
            return Result<PagedResult<UserList_DTO>>.Exception(ex);
        }
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    public async Task<Result<UserDetail_DTO>> GetUserByIdAsync(string userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"admin/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<UserDetail_DTO>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully retrieved user details for: {userId}");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<UserDetail_DTO>.Failure(
                null!,
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling get user by ID API for: {userId}");
            return Result<UserDetail_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Toggle user active status
    /// </summary>
    public async Task<Result<GeneralResponse>> ToggleUserStatusAsync(string userId, bool isActive)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PatchAsync($"admin/users/{userId}/toggle-status?isActive={isActive}", null);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully toggled user status for: {userId}");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to toggle user status" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling toggle user status API for: {userId}");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Suspend a user
    /// </summary>
    public async Task<Result<GeneralResponse>> SuspendUserAsync(string userId, string reason)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(reason, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync($"admin/users/{userId}/suspend", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully suspended user: {userId}");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to suspend user" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling suspend user API for: {userId}");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    public async Task<Result<GeneralResponse>> ResetPasswordAsync(AdminPasswordReset_DTO dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("admin/users/reset-password", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully reset user password");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to reset password" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling reset password API");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    public async Task<Result<GeneralResponse>> DeleteUserAsync(string userId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"admin/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully deleted user: {userId}");
                    return result;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to delete user" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calling delete user API for: {userId}");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    /// <summary>
    /// Get all departments
    /// </summary>
    public async Task<Result<List<string>>> GetDepartmentsAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("admin/users/departments");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<List<string>>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved departments");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<List<string>>.Failure(
                new List<string>(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling get departments API");
            return Result<List<string>>.Exception(ex);
        }
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    public async Task<Result<List<string>>> GetRolesAsync()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("admin/users/roles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<List<string>>>(content, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully retrieved roles");
                    return result;
                }
            }

            _logger.LogWarning($"API returned status code: {response.StatusCode}");
            return Result<List<string>>.Failure(
                new List<string>(),
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling get roles API");
            return Result<List<string>>.Exception(ex);
        }
    }
    
    public async Task<Result<GeneralResponse>> CreateUserAsync(CreateUpdateUser_DTO dto)
    {
        try
        {
            // Clean up DTO - only send role-specific data that's needed
            if (dto.RoleType != RoleType.Doctor) dto.DoctorInfo = null;
            if (dto.RoleType != RoleType.Patient) dto.PatientInfo = null;
            if (dto.RoleType != RoleType.Lab) dto.LabInfo = null;
            if (dto.RoleType != RoleType.Admin) dto.AdminInfo = null;

            // Log the DTO for debugging
            _logger.LogInformation($"Creating user with email: {dto.Email}, Gender: {dto.Gender}, RoleType: {dto.RoleType}");

            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            
            // Log the JSON being sent for debugging
            _logger.LogInformation($"JSON payload: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("admin/users", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully created user");
                    return result;
                }
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to create user" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Result<GeneralResponse>.Exception(ex);
        }
    }
    
    public async Task<Result<GeneralResponse>> UpdateUserAsync(string userId, CreateUpdateUser_DTO dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync($"admin/users/{userId}", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation($"Successfully updated user: {userId}");
                    return result;
                }
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to update user" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user: {userId}");
            return Result<GeneralResponse>.Exception(ex);
        }
    }
    
    public async Task<Result<GeneralResponse>> BulkActionAsync(BulkUserAction_DTO dto)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("admin/users/bulk-action", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<GeneralResponse>>(responseContent, _jsonOptions);
                
                if (result != null)
                {
                    _logger.LogInformation("Successfully performed bulk action");
                    return result;
                }
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"API returned status code: {response.StatusCode}. Error: {errorContent}");
            return Result<GeneralResponse>.Failure(
                new GeneralResponse { Success = false, Message = "Failed to perform bulk action" },
                $"API request failed with status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk action");
            return Result<GeneralResponse>.Exception(ex);
        }
    }
}
