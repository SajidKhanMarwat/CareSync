using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Common;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IDoctorService
{
    /// <summary>
    /// Build dashboard data for the doctor identified by the given userId (identity user id).
    /// </summary>
    Task<Result<DoctorDashboard_DTO>> GetDoctorDashboardAsync(string userId);

    /// <summary>
    /// Update an appointment status for a doctor (ensures doctor owns the appointment).
    /// </summary>
    Task<Result<GeneralResponse>> UpdateAppointmentStatusAsync(int appointmentId, CareSync.Shared.Enums.Appointment.AppointmentStatus_Enum newStatus, string doctorUserId);

    /// <summary>
    /// Get detailed appointment information by appointment id for the given doctor (userId).
    /// </summary>
    Task<Result<AppointmentDetails_DTO>> GetAppointmentDetailsAsync(int appointmentId, string doctorUserId);
}
