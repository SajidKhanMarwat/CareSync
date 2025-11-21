using CareSync.Shared.Enums.Appointment;

namespace CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;

public record Appointment_DTO(int AppointmentID, 
    int DoctorID, 
    int PatientID, 
    string AppointmentDate,
    AppointmentType_Enum AppointmentType,
    AppointmentStatus_Enum Status,
    string Reason,
    string Notes,
    bool IsDeleted,
    string CreatedBy,
    DateTime CreatedOn,
    string UpdatedBy,
    DateTime UpdatedOn
    );
