using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Repository;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;
using CareSync.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using static CareSync.InfrastructureLayer.Services.CookieServices.CookieService;
using static CareSync.InfrastructureLayer.Services.Security.JwtTokenGenerator;

namespace CareSync.InfrastructureLayer.Services.EntitiesServices;

public sealed class UserService(
    IUnitOfWork uow,
    UserManager<T_Users> userManager, 
    SignInManager<T_Users> signInManager, 
    RoleManager<T_Roles> roleManager,
    IPatientService patientService,
    IDoctorService doctorService,
    IMapper mapper, 
    ILogger<UserService> logger) : IUserService
{
    public async Task<Result<LoginResponse>> GenerateRefreshTokenAsync()
    {
        var refreshToken = await Get_RefreshToken_Cookies();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return CreateUnauthorizedResult();

        var principal = GetPrincipalFromToken(refreshToken);
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return CreateUnauthorizedResult();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return CreateUnauthorizedResult();

        var storedToken = await userManager.GetAuthenticationTokenAsync(user, "CareSync", "RefreshToken");
        if (storedToken != null && storedToken != refreshToken)
            return CreateUnauthorizedResult();

        (var newAccessToken, var newRefreshToken) =
            (await GenerateToken(user, userManager, roleManager), GenerateRefreshToken(userId));

        // await Set_RefreshToken_Cookies(newRefreshToken, 1);

        return Result<LoginResponse>.Success(new LoginResponse()
        {
            Success = true,
            Message = "Success",
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    private Result<LoginResponse> CreateUnauthorizedResult()
    {
        return Result<LoginResponse>.Failure(new LoginResponse
        {
            Success = false,
            Message = "Unauthorized",
            Token = string.Empty,
            RefreshToken = string.Empty
        });
    }

    public async Task<Result<GeneralResponse>> CreatePasswordAsync(Request_ForgetPassword_DTO input)
    {
        logger.LogInformation("Executing: CreatePassword");
        T_Users? user = await GetUserByEmailOrUsername(input.Email);

        if (user == null)
            return Result<GeneralResponse>.Failure(new GeneralResponse()
            {
                Success = false,
                Message = "invalid email or password, please try again with correct email & password.",
            });

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var passwordChanged = await userManager.ResetPasswordAsync(user, resetToken, input.NewPassword);

        if (passwordChanged.Succeeded)
            return Result<GeneralResponse>.Success(new GeneralResponse()
            {
                Success = true,
                Message = "password changed successfully.",
            });
        return Result<GeneralResponse>.Failure(new GeneralResponse()
        {
            Success = false,
            Message = passwordChanged.Errors.FirstOrDefault()!.Description,
        });
    }

    private async Task<T_Users?> GetUserByEmailOrUsername(string email)
    {
        var query = signInManager?.UserManager?.Users?
            .Include(u => u.UserRole)!
            .ThenInclude(ur => ur.Role)!;
        T_Users? user;
        if (email.Contains("@"))
        {
            user = await query.FirstOrDefaultAsync(u => u.Email == email);
        }
        else
        {
            // Try to parse as LoginID (int) or use as UserName
            if (int.TryParse(email, out int loginId))
            {
                user = await query.FirstOrDefaultAsync(u => u.UserName == email || u.LoginID == loginId);
            }
            else
            {
                user = await query.FirstOrDefaultAsync(u => u.UserName == email);
            }
        }
        return user;
    }

    public async Task<Result<LoginResponse>> GenerateLoginTokenAsync(LoginUser_DTO input)
    {
        logger.LogInformation("Executing: GenerateLoginToken");
        try
        {
            T_Users? user = await GetUserByEmailOrUsername(input.Email);
            if (user == null)
                return Result<LoginResponse>.Failure(new LoginResponse()
                {
                    Success = false,
                    Message = "invalid email or password, please try again with correct email & password.",
                    Token = string.Empty
                });

            var response = await signInManager.PasswordSignInAsync(user!, input.Password, true, false);
            (string token, string refreshToken) = ("", "");
            if (response.Succeeded)
            {
                (token, refreshToken) = (await GenerateToken(user, userManager, roleManager), GenerateRefreshToken(user.Id.ToString()));

                // Save refresh token in Identity
                await userManager.SetAuthenticationTokenAsync(user, "CareSync", "RefreshToken", refreshToken);

                // Add RefreshToken to cookie here...
                //await Set_RefreshToken_Cookies(refreshToken, 1);
            }
            return Result<LoginResponse>.Success(new LoginResponse()
            {
                Success = response.Succeeded,
                Message = response.Succeeded
                    ? "Success"
                    : "failed to login, please try again with correct email & password.",
                Token = response.Succeeded ? token : string.Empty,
                RefreshToken = refreshToken,
                Role = user?.Role?.RoleName!,
            });
        }
        catch (DbUpdateException ex)
        {
            logger.LogInformation(ex.Message);
            return Result<LoginResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> RegisterNewUserAsync(
        UserRegisteration_DTO request, string roleName = "patient")
    {
        logger.LogInformation("Executing: RegisterNewUserAsync for role: {RoleName}", roleName);
        try
        {
            await uow.BeginTransactionAsync();
            
            // Map user data
            var userEntity = mapper.Map<T_Users>(request);

            // Get role and set role type
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                await uow.RollbackAsync();
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = $"Role '{roleName}' not found in the system."
                });
            }

            userEntity.RoleID = role.Id;
            userEntity.RoleType = Enum.Parse<RoleType>(roleName, true);

            // Create user account
            var userManagerResponse = await userManager.CreateAsync(userEntity, request.Password);
            if (!userManagerResponse.Succeeded)
            {
                await uow.RollbackAsync();
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = userManagerResponse.Errors.First().Description
                });
            }

            // Add user to role
            await userManager.AddToRoleAsync(userEntity, roleName);

            // Create role-specific details if provided
            if (request.RegisterPatient != null || 
                request.RegisterDoctor != null || 
                request.RegisterLabAssistant != null)
            {
                var detailsResult = await CreateUserDetailsAsync(userEntity.Id, request, roleName);
                if (!detailsResult.IsSuccess)
                {
                    await uow.RollbackAsync();
                    return detailsResult;
                }
            }

            await uow.SaveChangesAsync();
            await uow.CommitAsync();
            
            logger.LogInformation("User {Email} registered successfully with role {RoleName}", request.Email, roleName);
            
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"Account created successfully. Welcome to CareSync!"
            });
        }
        catch (DbUpdateException dbEx)
        {
            await uow.RollbackAsync();
            logger.LogError(dbEx, "Database error during user registration");
            
            var sqlException = dbEx.InnerException as SqlException ?? 
                             dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered."
                });
            }

            return Result<GeneralResponse>.Exception(dbEx);
        }
        catch (Exception ex)
        {
            await uow.RollbackAsync();
            logger.LogError(ex, "Unexpected error during user registration");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    private async Task<Result<GeneralResponse>> CreateUserDetailsAsync(string userId, UserRegisteration_DTO request, string roleName)
    {
        logger.LogInformation("Creating {RoleName} details for user {UserId}", roleName, userId);
        
        try
        {
            switch (roleName.ToLower())
            {
                case "patient":
                    if (request.RegisterPatient != null)
                    {
                        request.RegisterPatient.UserID = userId;
                        request.RegisterPatient.CreatedBy = userId;
                        var patientDetails = mapper.Map<T_PatientDetails>(request.RegisterPatient);
                        await uow.PatientDetailsRepo.AddAsync(patientDetails);
                        logger.LogInformation("Patient details created for user {UserId}", userId);
                    }
                    break;

                case "doctor":
                    if (request.RegisterDoctor != null)
                    {
                        request.RegisterDoctor.UserID = userId;
                        request.RegisterDoctor.CreatedBy = userId;
                        var doctorDetails = mapper.Map<T_DoctorDetails>(request.RegisterDoctor);
                        await uow.DoctorDetailsRepo.AddAsync(doctorDetails);
                        logger.LogInformation("Doctor details created for user {UserId}", userId);
                    }
                    break;

                case "lab":
                case "labassistant":
                    if (request.RegisterLabAssistant != null)
                    {
                        request.RegisterLabAssistant.UserID = userId;
                        request.RegisterLabAssistant.CreatedBy = userId;
                        var labDetails = mapper.Map<T_Lab>(request.RegisterLabAssistant);
                        await uow.LabRepo.AddAsync(labDetails);
                        logger.LogInformation("Lab details created for user {UserId}", userId);
                    }
                    break;

                default:
                    logger.LogWarning("No specific details to create for role {RoleName}", roleName);
                    break;
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"{roleName} details created successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating {RoleName} details for user {UserId}", roleName, userId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<T_Users>> GetUserByIdAsync(string userId)
    {
        logger.LogInformation("Executing: GetUserByIdAsync for {UserId}", userId);
        try
        {
            var user = await signInManager.UserManager.Users
                .Include(u => u.Role)
                .Include(u => u.UserRole)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Result<T_Users>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);
            }

            return Result<T_Users>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user {UserId}", userId);
            return Result<T_Users>.Exception(ex);
        }
    }

    public async Task<Result<T_Users>> GetUserByEmailOrUsernameAsync(string emailOrUsername)
    {
        logger.LogInformation("Executing: GetUserByEmailOrUsernameAsync");
        try
        {
            var user = await GetUserByEmailOrUsername(emailOrUsername);
            
            if (user == null)
            {
                return Result<T_Users>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);
            }

            return Result<T_Users>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user by email/username");
            return Result<T_Users>.Exception(ex);
        }
    }

    public async Task<Result<List<T_Users>>> GetUsersByRoleAsync(string roleName)
    {
        logger.LogInformation("Executing: GetUsersByRoleAsync for role {RoleName}", roleName);
        try
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return Result<List<T_Users>>.Failure(new List<T_Users>(), 
                    $"Role '{roleName}' not found", 
                    System.Net.HttpStatusCode.NotFound);
            }

            var users = await signInManager.UserManager.Users
                .Include(u => u.Role)
                .Where(u => u.RoleID == role.Id && !u.IsDeleted)
                .ToListAsync();

            return Result<List<T_Users>>.Success(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving users for role {RoleName}", roleName);
            return Result<List<T_Users>>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserAsync(UserUpdate_DTO userUpdate)
    {
        logger.LogInformation("Executing: UpdateUserAsync for user {UserId}", userUpdate.UserId);
        try
        {
            var user = await userManager.FindByIdAsync(userUpdate.UserId);
            if (user == null)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            // Update user properties
            mapper.Map(userUpdate, user);
            user.UpdatedBy = userUpdate.UserId;
            user.UpdatedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Update failed"
                });
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "User updated successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {UserId}", userUpdate.UserId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeleteUserAsync(string userId, string deletedBy)
    {
        logger.LogInformation("Executing: DeleteUserAsync for user {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            // Soft delete
            user.IsDeleted = true;
            user.UpdatedBy = deletedBy;
            user.UpdatedOn = DateTime.UtcNow;
            user.IsActive = false;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Delete failed"
                });
            }

            logger.LogInformation("User {UserId} soft deleted by {DeletedBy}", userId, deletedBy);
            
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "User deleted successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {UserId}", userId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> ToggleUserStatusAsync(string userId, bool isActive)
    {
        logger.LogInformation("Executing: ToggleUserStatusAsync for user {UserId} to {IsActive}", userId, isActive);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            user.IsActive = isActive;
            user.UpdatedBy = userId;
            user.UpdatedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Status toggle failed"
                });
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"User {(isActive ? "activated" : "deactivated")} successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error toggling status for user {UserId}", userId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<bool>> CheckUserExistsAsync(string emailOrUsername)
    {
        logger.LogInformation("Executing: CheckUserExistsAsync");
        try
        {
            var user = await GetUserByEmailOrUsername(emailOrUsername);
            return Result<bool>.Success(user != null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if user exists");
            return Result<bool>.Exception(ex);
        }
    }
}
