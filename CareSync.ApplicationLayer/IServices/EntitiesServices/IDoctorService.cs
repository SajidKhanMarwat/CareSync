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

    /// <summary>
    /// Get full checkup data (patient details, current vitals, history) for an appointment owned by the doctor (doctorUserId).
    /// </summary>
    Task<Result<DoctorCheckup_DTO>> GetCheckupAsync(int appointmentId, string doctorUserId);

    /// <summary>
    /// Update patient vitals and chronic disease info for a given appointment/patient pair, scoped to the doctor user.
    /// </summary>
    Task<Result<GeneralResponse>> UpdateVitalsAsync(DoctorUpdateVitals_DTO input, string doctorUserId);

    /// <summary>
    /// Create a prescription for a given appointment/patient pair, ensuring doctor ownership and applying audit fields.
    /// </summary>
    Task<Result<GeneralResponse>> CreatePrescriptionAsync(DoctorCreatePrescription_DTO input, string doctorUserId);

    /// <summary>
    /// Get all lab reports associated with the doctor identified by the given userId.
    /// </summary>
    Task<Result<List<DoctorLabReport_DTO>>> GetDoctorLabReportsAsync(string userId);
}
