using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.DataLayer.Entities;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IAdminService
{
    //Task<Result<GeneralResponse>> AddUserPatientAsync(UserRegisteration_DTO registerPatient);
    Task<Result<T_Users>> GetUserAdminAsync();
    Task<Result<GeneralResponse>> UpdateUserAdminAsync(UserAdminUpdate_DTO request);
    Task<Result<GeneralResponse>> DeleteUserAdminAsync(string id);
    Task<Result<GetFirstRowCardsData_DTO>> GetAllAppointmentsAsyn();
    Task<Result<AddAppointment_DTO>> CreateAppointmentAsync();
    Task<Result<bool>> CreateAppointmentWithQuickPatientAsync(AddAppointmentWithQuickPatient_DTO input);

    // instead of object i will add proper DTOs to get proper structured data
    Task<Result<object>> CreatePatientWithDetails();
    Task<Result<object>> CreateDoctorWithDetails();
    Task<Result<object>> CreateLabWithDetails();
    //Task<Result<>> ViewUserReports();
    Task<Result<object>> GetUrgentItemsForAdmin();

    /// <summary>
    /// This will contain all the details
    /// Appointments Completed
    /// Lab Reports Ready
    /// </summary>
    /// <returns></returns>
    Task<Result<object>> GetTodayPerformanceCardData();

    /// <summary>
    /// Patients Counts & % based on the current and last month
    /// Doctors Counts & % based on the current and last month
    /// Admin Staff Counts
    /// Lab Counts & % based on the current and last month
    /// </summary>
    /// <returns></returns>
    Task<Result<object>> GetUserDistributionCardData();

    /// <summary>
    /// New Registrations This Month
    /// Total Appointments
    /// Lab Tests Completed
    /// </summary>
    /// <returns></returns>
    Task<Result<object>> GetPatientRegistrationTrendsChartData();

    /// <summary>
    /// Appointment Status Card Stats
    /// Confirmed Appointments
    /// Pending Appointments
    /// Completed Appointments
    /// Cancelled Appointments
    /// Rescheduled Appointments
    /// </summary>
    /// <returns></returns>
    Task<Result<object>> GetAppointmentStatusChartData();

    /// <summary>
    /// Today's Appointments with details
    /// Doctor Name
    /// Patient Name
    /// Appointment Time
    /// Appointment Type (Regular Checkup, Follow-up, Specialist Consultation etc)
    /// </summary>
    /// <returns></returns>
    Task<Result<object>> GetTodaysAppointmentsCardData();
}
