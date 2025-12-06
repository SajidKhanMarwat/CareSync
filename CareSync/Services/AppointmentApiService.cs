using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.Shared.Enums.Appointment;

namespace CareSync.Services;

public class AppointmentApiService : IAppointmentService
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<AppointmentApiService> _logger;

    public AppointmentApiService(AdminApiService adminApiService, ILogger<AppointmentApiService> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    public async Task<List<Appointment_DTO>> GetAllAppointmentsAsync()
    {
        try
        {
            var result = await _adminApiService.GetAllAppointmentsAsync<Result<TodaysAppointmentsList_DTO>>();
            if (result?.IsSuccess == true && result.Data?.Appointments != null)
            {
                return result.Data.Appointments.Select(Map).ToList();
            }

            return new List<Appointment_DTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all appointments from API");
            return new List<Appointment_DTO>();
        }
    }

    public async Task<List<Appointment_DTO>> GetAppointmentsForDoctorAsync(int doctorId)
    {
        try
        {
            var all = await GetAllAppointmentsAsync();
            return all.Where(a => a.DoctorID == doctorId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for doctor {DoctorId}", doctorId);
            return new List<Appointment_DTO>();
        }
    }

    public async Task<List<Appointment_DTO>> GetAppointmentsForPatientAsync(int patientId)
    {
        try
        {
            var all = await GetAllAppointmentsAsync();
            return all.Where(a => a.PatientID == patientId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments for patient {PatientId}", patientId);
            return new List<Appointment_DTO>();
        }
    }

    private Appointment_DTO Map(TodayAppointmentItem item)
    {
        var dto = new Appointment_DTO
        {
            AppointmentID = item.AppointmentID,
            DoctorID = item.DoctorID,
            PatientID = item.PatientID,
            AppointmentDate = item.AppointmentDate,
            AppointmentType = item.AppointmentType,
            Status = item.Status,
            DoctorName = item.DoctorName,
            PatientName = item.PatientName,
            Reason = item.Reason ?? string.Empty,
            IsDeleted = false,
            CreatedBy = string.Empty,
            CreatedOn = DateTime.UtcNow
        };

        return dto;
    }
}