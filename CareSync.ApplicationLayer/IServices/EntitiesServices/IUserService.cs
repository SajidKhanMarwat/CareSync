using CareSync.Shared.Models;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.DataLayer.Entities;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

/// <summary>
/// Service interface for user management operations including authentication, registration, and profile management.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Generates a new access token using a valid refresh token
    /// </summary>
    Task<Result<LoginResponse>> GenerateRefreshTokenAsync();

    /// <summary>
    /// Creates or resets a user's password
    /// </summary>
    Task<Result<GeneralResponse>> CreatePasswordAsync(Request_ForgetPassword_DTO input);

    /// <summary>
    /// Authenticates a user and generates JWT tokens
    /// </summary>
    Task<Result<LoginResponse>> GenerateLoginTokenAsync(LoginUser_DTO input);

    /// <summary>
    /// Registers a new user with specified role (patient, doctor, lab, admin)
    /// </summary>
    Task<Result<GeneralResponse>> RegisterNewUserAsync(UserRegisteration_DTO request, string roleName = "patient");

    /// <summary>
    /// Gets a user by their ID with role information
    /// </summary>
    Task<Result<T_Users>> GetUserByIdAsync(string userId);

    /// <summary>
    /// Gets a user by email or username
    /// </summary>
    Task<Result<T_Users>> GetUserByEmailOrUsernameAsync(string emailOrUsername);

    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    Task<Result<List<T_Users>>> GetUsersByRoleAsync(string roleName);

    /// <summary>
    /// Updates user basic information
    /// </summary>
    Task<Result<GeneralResponse>> UpdateUserAsync(UserUpdate_DTO userUpdate);

    /// <summary>
    /// Soft deletes a user (sets IsDeleted flag)
    /// </summary>
    Task<Result<GeneralResponse>> DeleteUserAsync(string userId, string deletedBy);

    /// <summary>
    /// Activates or deactivates a user account
    /// </summary>
    Task<Result<GeneralResponse>> ToggleUserStatusAsync(string userId, bool isActive);

    /// <summary>
    /// Checks if an email or username already exists
    /// </summary>
    Task<Result<bool>> CheckUserExistsAsync(string emailOrUsername);
}
