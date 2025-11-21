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

    private async Task<T_Users?> GetUserByEmailOrUsername(string Email)
    {
        var query = signInManager?.UserManager?.Users?
            .Include(u => u.UserRole)!
            .ThenInclude(ur => ur.Role)!;
        T_Users? user;
        if (Email.Contains("@"))
            user = await query.FirstOrDefaultAsync(u => u.Email == Email);
        else
            user = await query.FirstOrDefaultAsync(u => u.UserName == Email || u.LoginID.ToString() == Email);
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
    UserRegisteration_DTO request, string roleName = "patient", string roleId = "")
    {
        logger.LogInformation("Executing: RegisterNewUserAsync");
        try
        {
            await uow.BeginTransactionAsync();
            var userEntity = mapper.Map<T_Users>(request);

            var role = await roleManager.FindByNameAsync(roleName);
            userEntity.RoleID = role?.Id!;

            var userManagerResponse = await userManager.CreateAsync(userEntity, request.Password);
            if (!userManagerResponse.Succeeded)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = userManagerResponse.Errors.First().Description
                });
            await userManager.AddToRoleAsync(userEntity, roleName);

            if( request.RegisterPatient != null ||
                request.RegisterDoctor != null ||
                request.RegisterLabAssistant != null )
                return await CreateUserDetails(request, roleName);
            else
            {
                await uow.SaveChangesAsync();
                await uow.CommitAsync();
                return Result<GeneralResponse>.Success(new GeneralResponse()
                {
                    Success = userManagerResponse.Succeeded,
                    Message = "account created successfully, check confirmation email.",
                });
            }
        }
        catch (DbUpdateException dbEx)
        {
            await uow.RollbackAsync();
            logger.LogInformation(dbEx.Message);
            var sqlException =
                dbEx.InnerException as SqlException ??
                dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null &&
                (sqlException.Number == 2601 || sqlException.Number == 2627))
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered.",
                });
            }

            return Result<GeneralResponse>.Exception(dbEx);
        }
    }

    private async Task<Result<GeneralResponse>> CreateUserDetails(UserRegisteration_DTO request, string roleName)
    {
        logger.LogInformation("Executing: CreateUserDetails");
        Result<GeneralResponse> detailsResult = new();
        try
        {
            if (roleName.ToLower() == "patient")
            {
                var patientDto = mapper.Map<RegisterPatient_DTO>(request.RegisterPatient);
                detailsResult = await patientService.AddPatientDetailsAsync(patientDto);
            }
            else if (roleName.ToLower() == "doctor")
            {
                var doctorDto = mapper.Map<RegisterDoctor_DTO>(request.RegisterDoctor);
                //detailsResult = await doctorService.AddPatientAsync(doctorDto);
            }
            else if (roleName.ToLower() == "labassistant")
            {
                var labDto = mapper.Map<RegisterLabAssistant_DTO>(request.RegisterLabAssistant);
                //detailsResult = await labService.AddPatientAsync(labDto);
            }
            await uow.SaveChangesAsync();
            await uow.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception: {ex}");
            Result<GeneralResponse>.Exception(ex);
        }

        return detailsResult;
    }
}
