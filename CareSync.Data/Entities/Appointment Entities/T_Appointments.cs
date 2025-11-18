
using CareSync.Shared.Enums.Appointment;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents a medical appointment in the CareSync system.
/// This entity manages the scheduling and tracking of patient consultations with healthcare providers.
/// It serves as the central record for medical encounters, linking patients with doctors and
/// facilitating the coordination of medical services, prescriptions, and lab requests.
/// </summary>
public class T_Appointments : BaseEntity
{
    /// <summary>
    /// Unique identifier for the appointment record.
    /// Primary key that serves as the main reference for all appointment-related activities.
    /// Auto-incremented integer value assigned when a new appointment is scheduled.
    /// </summary>
    public int AppointmentID { get; set; }

    /// <summary>
    /// Reference to the doctor who will conduct the appointment.
    /// Links to the DoctorID in T_DoctorDetails table to identify the healthcare provider.
    /// Required field as every appointment must have an assigned doctor.
    /// </summary>
    public required int DoctorID { get; set; }

    /// <summary>
    /// Reference to the patient who is scheduled for the appointment.
    /// Links to the PatientID in T_PatientDetails table to identify the patient.
    /// Required field as every appointment must have an associated patient.
    /// </summary>
    public required int PatientID { get; set; }

    /// <summary>
    /// The scheduled date and time for the appointment.
    /// Critical for appointment scheduling, calendar management, and patient notifications.
    /// Required field that determines when the medical consultation will take place.
    /// </summary>
    public DateTime AppointmentDate { get; set; }

    /// <summary>
    /// The type or category of the appointment (e.g., Consultation, Follow-up, Emergency, Routine Check-up).
    /// Helps in appointment scheduling, resource allocation, and billing processes.
    /// Nullable to handle cases where appointment type is not specified during initial booking.
    /// </summary>
    public required AppointmentType_Enum AppointmentType { get; set; }

    /// <summary>
    /// Current status of the appointment (e.g., Scheduled, Confirmed, In-Progress, Completed, Cancelled, No-Show).
    /// Essential for appointment management, workflow tracking, and patient communication.
    /// Nullable but typically populated with default status when appointment is created.
    /// </summary>
    public AppointmentStatus_Enum Status { get; set; }

    /// <summary>
    /// The reason or purpose for the appointment as provided by the patient or referring doctor.
    /// Helps doctors prepare for the consultation and prioritize urgent cases.
    /// Nullable as patients may not always provide detailed reasons during booking.
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Additional notes or comments about the appointment.
    /// Can include special instructions, preparation requirements, or administrative notes.
    /// Nullable as not all appointments require additional notes or comments.
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the doctor conducting the appointment.
    /// Provides access to doctor's specialization, qualifications, and contact information.
    /// </summary>
    public virtual T_DoctorDetails? Doctor { get; set; }

    /// <summary>
    /// Navigation property to the patient scheduled for the appointment.
    /// Provides access to patient's medical history, contact details, and health records.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }

    /// <summary>
    /// Navigation property to lab test requests generated during this appointment.
    /// Contains all laboratory tests ordered by the doctor during the consultation.
    /// </summary>
    public virtual ICollection<T_LabRequests> LabRequests { get; set; } = new List<T_LabRequests>();

    /// <summary>
    /// Navigation property to prescriptions issued during this appointment.
    /// Contains all medications and treatments prescribed by the doctor during the consultation.
    /// </summary>
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();
}
