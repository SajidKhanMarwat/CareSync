using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IPatientService
{
    Task<Result<List<GetAllPatients_DTO>>> GetAllPatientsAsync();
    Task<Result<GetPatient_DTO>> GetPatientByIdAsync(object patientId);
    Task<Result<GeneralResponse>> AddPatientDetailsAsync(RegisterPatient_DTO patient);
    //Task<Result<bool>> AddPatientDetailsAsync(RegisterPatient_DTO patient);
    Task<Result<GeneralResponse>> UpdateUserPatientAsync(UserPatientProfileUpdate_DTO request);
    Task<Result<GeneralResponse>> DeleteUserPatientAsync(string id);

    /// <summary>
    /// Gets complete patient dashboard data including profile, statistics, visits, and reports
    /// </summary>
    Task<Result<PatientDashboard_DTO>> GetPatientDashboardAsync(string userId);
}
