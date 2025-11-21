using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient;

public class AppointmentDetailsModel : BasePageModel
{
    private readonly ILogger<AppointmentDetailsModel> _logger;

    public AppointmentDetailsModel(ILogger<AppointmentDetailsModel> logger)
    {
        _logger = logger;
    }

    [FromQuery(Name = "id")]
    public int AppointmentId { get; set; }

    // Appointment Information
    public string AppointmentNumber { get; set; } = "#APT-2024-001";
    public DateTime AppointmentDate { get; set; } = new DateTime(2024, 11, 10, 10, 0, 0);
    public string AppointmentType { get; set; } = "General Consultation";
    public string Status { get; set; } = "Completed";
    public string ReasonForVisit { get; set; } = "Regular checkup and discussion about recent lab results.";

    // Doctor Information
    public string DoctorName { get; set; } = "Dr. Sarah Johnson";
    public string DoctorSpecialty { get; set; } = "Cardiologist";
    public string DoctorExperience { get; set; } = "15 years";
    public decimal DoctorRating { get; set; } = 4.9m;
    public int ReviewCount { get; set; } = 127;
    public string DoctorLocation { get; set; } = "City Medical Center, Downtown";
    public string DoctorPhone { get; set; } = "+1 (555) 123-4567";
    public string DoctorEmail { get; set; } = "dr.sarah.johnson@caresync.com";

    // Medical Notes
    public string ChiefComplaint { get; set; } = "Patient reports mild persistent headaches over the past 2 weeks, accompanied by occasional fatigue.";
    public string Diagnosis { get; set; } = "Tension-type headache (Primary); Mild stress-related symptoms; Slightly elevated blood pressure";
    public string TreatmentPlan { get; set; } = "Prescribed pain relief medication, stress management techniques, and follow-up blood pressure monitoring.";
    public string DoctorNotes { get; set; } = "Patient appears generally healthy. Blood pressure slightly elevated but not concerning at this stage.";

    // Vitals
    public string BloodPressure { get; set; } = "140/90";
    public int HeartRate { get; set; } = 72;
    public decimal Temperature { get; set; } = 98.6m;
    public int OxygenLevel { get; set; } = 98;

    // Payment Information
    public decimal ConsultationFee { get; set; } = 150.00m;
    public decimal LabTestsFee { get; set; } = 85.00m;
    public decimal PrescriptionsFee { get; set; } = 45.00m;
    public decimal TotalAmount { get; set; } = 280.00m;
    public string PaymentStatus { get; set; } = "Paid";
    public string PaymentMethod { get; set; } = "Insurance + Credit Card";

    // Follow-up
    public DateTime? NextAppointmentDate { get; set; } = new DateTime(2024, 11, 24, 10, 30, 0);
    public string NextAppointmentDoctor { get; set; } = "Dr. Sarah Johnson";

    public IActionResult OnGet()
    {
        // Check if user is authenticated and has Patient role
        var authResult = RequireRole("Patient");
        if (authResult != null) return authResult;

        if (AppointmentId <= 0)
        {
            _logger.LogWarning("Appointment ID not provided or invalid");
            TempData["ErrorMessage"] = "Invalid appointment ID";
            return RedirectToPage("/Patient/Appointments");
        }

        _logger.LogInformation($"Loading appointment details for ID: {AppointmentId}");

        // TODO: Load actual appointment data from database
        // var appointment = await _context.T_Appointments
        //     .Include(a => a.Doctor)
        //     .Include(a => a.Patient)
        //     .Include(a => a.Prescriptions)
        //         .ThenInclude(p => p.PrescriptionItems)
        //     .Include(a => a.LabRequests)
        //         .ThenInclude(lr => lr.LabReports)
        //     .Where(a => a.AppointmentID == AppointmentId && !a.IsDeleted)
        //     .FirstOrDefaultAsync();
        //
        // if (appointment == null)
        // {
        //     TempData["ErrorMessage"] = "Appointment not found";
        //     return RedirectToPage("/Patient/Appointments");
        // }
        //
        // // Verify that the appointment belongs to the current patient
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (appointment.Patient.UserID != userId)
        // {
        //     _logger.LogWarning($"Unauthorized access attempt to appointment {AppointmentId}");
        //     return Forbid();
        // }
        //
        // // Populate model properties from database
        // AppointmentNumber = $"#APT-{appointment.AppointmentID.ToString().PadLeft(6, '0')}";
        // AppointmentDate = appointment.AppointmentDate;
        // AppointmentType = appointment.AppointmentType.ToString();
        // Status = appointment.Status.ToString();
        // ReasonForVisit = appointment.Reason;
        // DoctorName = $"Dr. {appointment.Doctor.User.FirstName} {appointment.Doctor.User.LastName}";
        // DoctorSpecialty = appointment.Doctor.Specialization;
        // ... map other properties

        return Page();
    }
}
