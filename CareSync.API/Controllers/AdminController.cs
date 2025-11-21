using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController(IAdminService adminService, IUserService userService, ILogger<AdminController> logger)
{
    [HttpGet]
    public async Task<Result<T_Users>> TestGet()
    {
        return await adminService.GetUserAdminAsync();
    }

    [HttpPatch("update-admin-profile")]
    public async Task<Result<GeneralResponse>> UpdateUserProfile(UserAdminUpdate_DTO userUpdate_DTO)
        => await adminService.UpdateUserAdminAsync(userUpdate_DTO);

    [HttpGet("get-admin-dashboard-records-row1-counts")]
    [AllowAnonymous]
    public async Task<Result<GetFirstRowCardsData_DTO>> GetAdminDashboardCounts()
        => await adminService.GetAllAppointmentsAsyn();

    [HttpPost("patient-registeration")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> Register([FromBody] UserRegisteration_DTO dto)
    {
        return await userService.RegisterNewUserAsync(dto);
    }

    [HttpPost("create-appointment-with-patient-registeration")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> CreateAppointmentAndPatient()
    {
        throw new NotImplementedException();
    }

    [HttpPost("create-appointment")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> CreateAppointment()
    {
        throw new NotImplementedException();
    }

    [HttpGet("search-patient")]
    [AllowAnonymous]
    public async Task<Result<GeneralResponse>> SearchPatient(string value)
    {
        throw new NotImplementedException();
    }
}
