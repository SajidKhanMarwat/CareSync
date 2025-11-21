using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;

namespace CareSync.ApplicationLayer.IServices.EntitiesServices;

public interface IAppointmentService
{
    Task<List<Appointment_DTO>> GetAllAppointmentsAsync();
    Task<List<Appointment_DTO>> GetAppointmentsForDoctorAsync(int doctorId);
    Task<List<Appointment_DTO>> GetAppointmentsForPatientAsync(int patientId);
}
