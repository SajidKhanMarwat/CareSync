using CareSync.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using ReturnResult;
using SharpCart.ApplicationLayer.Common;
using SharpCart.ApplicationLayer.Contracts.UsersDTOs;
using SharpCart.ApplicationLayer.IServices.EntitiesServices;

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
        return await userService.GenerateLoginToken(input);
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
        return await userService.RegisterNewUser(request);
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
        return await userService.CreatePassword(input);
    }

    [HttpPost("refresh-token")]
    public async Task<Result<LoginResponse>> RefreshToken()
        => await userService.GenerateRefreshTokenAsync();
}
