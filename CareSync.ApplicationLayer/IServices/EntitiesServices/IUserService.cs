using CareSync.Shared.Models;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.DataLayer.Entities;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IUserService
{
    Task<Result<LoginResponse>> GenerateRefreshTokenAsync();
    Task<Result<GeneralResponse>> CreatePasswordAsync(Request_ForgetPassword_DTO input);
    Task<Result<LoginResponse>> GenerateLoginTokenAsync(LoginUser_DTO input);
    Task<Result<GeneralResponse>> RegisterNewUserAsync(UserRegisteration_DTO request, string roleName = "patient");
}
