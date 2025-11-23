using CareSync.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.ApiResult;

namespace CareSync.APIs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserService userService, ILogger<AccountController> logger)
{
    [HttpPost("Login")]
    public async Task<Result<LoginResponse>> Login([FromBody] LoginUser_DTO input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Email) ||
            string.IsNullOrWhiteSpace(input.Password))
            return Result<LoginResponse>.Failure(new LoginResponse()
            {
                Success = false,
                Message = "invalid input values.",
                Token = string.Empty
            });
        return await userService.GenerateLoginTokenAsync(input);
    }

    [HttpPost("Register")]
    public async Task<Result<GeneralResponse>> Register([FromBody] UserRegisteration_DTO request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
            return Result<GeneralResponse>.Failure(new GeneralResponse()
            {
                Success = false,
                Message = "invalid input values.",
            });
        return await userService.RegisterNewUserAsync(request);
    }

    [HttpPost("forget-password")]
    public async Task<Result<GeneralResponse>> ForgetPassword([FromBody] Request_ForgetPassword_DTO input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.Email) ||
            string.IsNullOrWhiteSpace(input.NewPassword))
            return Result<GeneralResponse>.Failure(new GeneralResponse()
            {
                Success = false,
                Message = "invalid input values.",
            });
        return await userService.CreatePasswordAsync(input);
    }

    [HttpPost("refresh-token")]
    public async Task<Result<LoginResponse>> RefreshToken()
        => await userService.GenerateRefreshTokenAsync();

    [HttpPost("verify-user")]
    public async Task<Result<VerifyUserResponse>> VerifyUser([FromBody] VerifyUserRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrUsername))
            return Result<VerifyUserResponse>.Failure(new VerifyUserResponse(), 
                "Email or username is required.");

        var userResult = await userService.GetUserByEmailOrUsernameAsync(request.EmailOrUsername);
        
        if (userResult.IsSuccess && userResult.Data != null)
        {
            return Result<VerifyUserResponse>.Success(new VerifyUserResponse()
            {
                Email = userResult.Data.Email ?? string.Empty,
                Username = userResult.Data.UserName ?? string.Empty
            });
        }
        
        return Result<VerifyUserResponse>.Failure(new VerifyUserResponse(), 
            "No account found with the provided email or username.");
    }
}

public class VerifyUserRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
}

public class VerifyUserResponse
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
