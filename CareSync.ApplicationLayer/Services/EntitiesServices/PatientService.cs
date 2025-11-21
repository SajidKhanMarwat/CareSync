using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.InfrastructureLayer.Services.EntitiesServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public sealed class PatientService(UserManager<T_Users> userManager,
    IUnitOfWork uow,
    IMapper mapper,
    ILogger<UserService> logger) : IPatientService
{
    public Task<Result<List<GetAllPatients_DTO>>> GetAllPatientsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<GetPatient_DTO>> GetPatientByIdAsync(object patientId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<GeneralResponse>> AddPatientDetailsAsync(RegisterPatient_DTO patient)
    {
        logger.LogInformation("Executing: AddPatientAsync");

        try
        {
            await uow.PatientDetailsRepo.AddAsync(mapper.Map<T_PatientDetails>(patient));
            return Result<GeneralResponse>.Success(new GeneralResponse()
            {
                Success = true,
                Message = "profile updated successfully.",
            });
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogInformation(dbEx.Message);
            var sqlException = dbEx.InnerException as SqlException ?? dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered. Please login to your account.",
                });
            return Result<GeneralResponse>.Exception(dbEx);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserPatientAsync(UserPatientProfileUpdate_DTO request)
    {
        logger.LogInformation("Executing: RegisterNewUser");
        try
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse()
                {
                    Success = false,
                    Message = "User not found"
                });

            mapper.Map(request, user);
            var response = await userManager.UpdateAsync(user);
            if (response.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse()
                {
                    Success = true,
                    Message = "profile updated successfully.",
                });
            return Result<GeneralResponse>.Failure(new GeneralResponse()
            {
                Success = false,
                Message = response.Errors.FirstOrDefault()!.Description,
            });
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogInformation(dbEx.Message);
            var sqlException = dbEx.InnerException as SqlException ?? dbEx.InnerException?.InnerException as SqlException;

            if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "This Email/Username is already registered. Please login to your account.",
                });
            return Result<GeneralResponse>.Exception(dbEx);
        }
    }

    public Task<Result<GeneralResponse>> DeleteUserPatientAsync(string id)
    {
        throw new NotImplementedException();
    }
}
