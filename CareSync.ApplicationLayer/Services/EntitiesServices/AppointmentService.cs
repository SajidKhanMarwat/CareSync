using AutoMapper;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Repository;
using CareSync.DataLayer.Entities;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public class AppointmentService(IRepository<T_Appointments> _appointmentRepo, IMapper _mapper) : IAppointmentService
{
    public async Task<List<Appointment_DTO>> GetAllAppointmentsAsync()
    {
        var appointments = await _appointmentRepo.GetAllAsync();
        return _mapper.Map<List<Appointment_DTO>>(appointments);
    }

    public async Task<List<Appointment_DTO>> GetAppointmentsForDoctorAsync(int doctorId)
    {
        var appointments = await _appointmentRepo.GetAsync(a => a.DoctorID == doctorId);
        return _mapper.Map<List<Appointment_DTO>>(appointments);
    }

    public async Task<List<Appointment_DTO>> GetAppointmentsForPatientAsync(int patientId)
    {
        var appointments = await _appointmentRepo.GetAsync(a => a.PatientID == patientId);
        return _mapper.Map<List<Appointment_DTO>>(appointments);
    }
}

