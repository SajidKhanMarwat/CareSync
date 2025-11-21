using CareSync.APIs.Controllers;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
{
    [HttpPost("update-patient-profile")]
    public async Task<Result<GeneralResponse>> UpdateUserProfile(UserPatientProfileUpdate_DTO userUpdate_DTO)
        => await patientService.UpdateUserPatientAsync(userUpdate_DTO);
}
