using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public class AdminService(
    UserManager<T_Users> userManager,
    RoleManager<T_Roles> roleManager,
    IUnitOfWork uow,
    IPatientService patientService,
    IDoctorService doctorService,
    ILabService labService,
    IAppointmentService appointmentService,
    IMapper mapper,
    ILogger<AdminService> logger) : IAdminService
{
    public async Task<Result<T_Users>> GetUserAdminAsync()
    {
        logger.LogInformation("Executing: GetUserAdminAsync");
        var result = await uow.UserRepo.GetByIdAsync("b495e75a-f38c-49be-8c12-c4da30c5f1bd");
        return Result<T_Users>.Success(result!);
    }

    //public async Task<Result<GeneralResponse>> AddUserPatientAsync(UserRegisteration_DTO registerPatient)
    //{
    //    return await userService.RegisterNewUserAsync(registerPatient);
    //}

    public async Task<Result<GeneralResponse>> UpdateUserAdminAsync(UserAdminUpdate_DTO request)
    {
        return null;
        //logger.LogInformation("Executing: RegisterNewUser");
        //try
        //{
        //    var user = await userManager.FindByIdAsync(request.UserId);
        //    if (user == null)
        //        return Result<GeneralResponse>.Failure(new GeneralResponse()
        //        {
        //            Success = false,
        //            Message = "User not found"
        //        });

        //    mapper.Map(request, user);
        //    var response = await userManager.UpdateAsync(user);
        //    if (response.Succeeded)
        //        return Result<GeneralResponse>.Success(new GeneralResponse()
        //        {
        //            Success = true,
        //            Message = "profile updated successfully.",
        //        });
        //    return Result<GeneralResponse>.Failure(new GeneralResponse()
        //    {
        //        Success = false,
        //        Message = response.Errors.FirstOrDefault()!.Description,
        //    });
        //}
        //catch (DbUpdateException dbEx)
        //{
        //    logger.LogInformation(dbEx.Message);
        //    var sqlException = dbEx.InnerException as SqlException ?? dbEx.InnerException?.InnerException as SqlException;

        //    if (sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627))
        //        return Result<GeneralResponse>.Failure(new GeneralResponse
        //        {
        //            Success = false,
        //            Message = "Failed to update.",
        //        });
        //    return Result<GeneralResponse>.Exception(dbEx);
        //}
    }

    public Task<Result<GeneralResponse>> DeleteUserAdminAsync(string id)
    {
        throw new NotImplementedException();
    }

    //public async Task<Result<GetFirstRowCardsData_DTO>> GetAllAppointmentsAsyn()
    //{
    //    var appointmentsCount = await uow.AppointmentsRepo.GetCountAsync(c => true);

    //    var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
    //    var doctorsCount = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id);

    //    var patiendRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
    //    var patientsCount = await uow.UserRepo.GetCountAsync(u => u.RoleID == patiendRole!.Id);

    //    GetFirstRowCardsData_DTO cardsData_DTO = new GetFirstRowCardsData_DTO();
    //    cardsData_DTO.TotalAppointments = appointmentsCount;
    //    cardsData_DTO.TotalDoctors = doctorsCount;
    //    cardsData_DTO.TotalPatients = patientsCount;
    //    return Result<GetFirstRowCardsData_DTO>.Success(cardsData_DTO);
    //}

    public async Task<Result<GetFirstRowCardsData_DTO>> GetAllAppointmentsAsyn()
    {
        // ========== ROLE ID RESOLUTION ==========
        var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
        var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());


        // ========== CURRENT MONTH RANGE ==========
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
        var lastDayLastMonth = firstDayThisMonth.AddDays(-1);


        // ========== TOTAL COUNTS ==========
        var totalAppointments = await uow.AppointmentsRepo.GetCountAsync(a => true);
        var totalDoctors = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id);
        var totalPatients = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id);


        // ========== THIS MONTH COUNTS ==========
        var thisMonthAppointments = await uow.AppointmentsRepo
            .GetCountAsync(a => a.CreatedOn >= firstDayThisMonth);

        var thisMonthDoctors = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == doctorRole!.Id
                             && u.CreatedOn >= firstDayThisMonth);

        var thisMonthPatients = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == patientRole!.Id
                             && u.CreatedOn >= firstDayThisMonth);


        // ========== LAST MONTH COUNTS ==========
        var lastMonthAppointments = await uow.AppointmentsRepo
            .GetCountAsync(a => a.CreatedOn >= firstDayLastMonth
                             && a.CreatedOn <= lastDayLastMonth);

        var lastMonthDoctors = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == doctorRole!.Id
                             && u.CreatedOn >= firstDayLastMonth
                             && u.CreatedOn <= lastDayLastMonth);

        var lastMonthPatients = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == patientRole!.Id
                             && u.CreatedOn >= firstDayLastMonth
                             && u.CreatedOn <= lastDayLastMonth);


        // ========== PERCENTAGE CALCULATIONS ==========
        decimal AppointmentPercent = CalculatePercentage(thisMonthAppointments, lastMonthAppointments);
        decimal DoctorPercent = CalculatePercentage(thisMonthDoctors, lastMonthDoctors);
        decimal PatientPercent = CalculatePercentage(thisMonthPatients, lastMonthPatients);

        var getFirstRowCardsData = new GetFirstRowCardsData_DTO
        {
            TotalAppointments = totalAppointments,
            ThisVsLastMonthPercentageAppointment = AppointmentPercent,

            TotalDoctors = totalDoctors,
            ThisVsLastMonthPercentageDoctors = DoctorPercent,

            TotalPatients = totalPatients,
            ThisVsLastMonthPercentagePatients = PatientPercent
        };

        return Result<GetFirstRowCardsData_DTO>.Success(getFirstRowCardsData);
    }

    private decimal CalculatePercentage(int thisMonth, int lastMonth)
    {
        if (lastMonth == 0)
        {
            if (thisMonth == 0) return 0;
            return 100;
        }

        return Math.Round(((decimal)(thisMonth - lastMonth) / lastMonth) * 100, 2);
    }

    public Task<Result<AddAppointment_DTO>> CreateAppointmentAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<AddAppointmentWithQuickPatient_DTO>> CreateAppointmentWithQuickPatientAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> CreatePatientWithDetails()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> CreateDoctorWithDetails()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> CreateLabWithDetails()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetUrgentItemsForAdmin()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetTodayPerformanceCardData()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetUserDistributionCardData()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetPatientRegistrationTrendsChartData()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetAppointmentStatusChartData()
    {
        throw new NotImplementedException();
    }

    public Task<Result<object>> GetTodaysAppointmentsCardData()
    {
        throw new NotImplementedException();
    }
}
