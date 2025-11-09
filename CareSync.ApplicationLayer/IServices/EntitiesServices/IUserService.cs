using CareSync.Shared.Models;
using ReturnResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IUserService
{
    Task<Result<LoginResponse>> GenerateRefreshTokenAsync();
    Task<Result<GeneralResponse>> CreatePassword(Request_ForgetPassword_DTO input);
    Task<Result<LoginResponse>> GenerateLoginToken(LoginUser_DTO input);
    Task<Result<GeneralResponse>> RegisterNewUser(UserRegisteration_DTO request);
}
