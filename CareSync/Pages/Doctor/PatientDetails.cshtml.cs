using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor
{
    public class PatientDetailsModel : BasePageModel
    {
        private readonly ILogger<PatientDetailsModel> _logger;

        public PatientDetailsModel(ILogger<PatientDetailsModel> logger)
        {
            _logger = logger;
        }

        // Patient Basic Information
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactNumber { get; set; } = string.Empty;
        public string RelationshipToEmergency { get; set; } = string.Empty;

        // Statistics
        public int TotalAppointments { get; set; }
        public int TotalPrescriptions { get; set; }
        public DateTime LastVisitDate { get; set; }

        // Medical Data
        public List<VitalsHistoryDto> VitalsHistory { get; set; } = new();
        public List<ChronicDiseaseDetailDto> ChronicDiseases { get; set; } = new();
        public List<PrescriptionDetailDto> PrescriptionsHistory { get; set; } = new();
        public List<AppointmentHistoryDto> AppointmentsHistory { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int patientId)
        {
            // Check if user is authenticated and has Doctor role
            var authResult = RequireRole("Doctor");
            if (authResult != null) return authResult;

            try
            {
                await LoadPatientData(patientId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patient details for patient {PatientId}", patientId);
                TempData["Error"] = "Unable to load patient details. Please try again.";
                return RedirectToPage("/Doctor/Patients");
            }
        }

        private async Task LoadPatientData(int patientId)
        {
            // Load patient basic information
            PatientId = patientId;
            PatientName = "Willian Mathews";
            Age = 21;
            Gender = "Male";
            BloodGroup = "O+";
            MaritalStatus = "Single";
            Occupation = "Software Engineer";
            EmergencyContactName = "Sarah Mathews";
            EmergencyContactNumber = "+1234567890";
            RelationshipToEmergency = "Mother";

            // Statistics
            TotalAppointments = 15;
            TotalPrescriptions = 12;
            LastVisitDate = DateTime.Now.AddDays(-7);

            // Load vitals history
            VitalsHistory = new List<VitalsHistoryDto>
            {
                new VitalsHistoryDto
                {
                    RecordedDate = DateTime.Now.AddDays(-7),
                    Height = 175.5m,
                    Weight = 75.0m,
                    BMI = 24.4m,
                    BloodPressure = "120/80",
                    PulseRate = 72,
                    IsDiabetic = false,
                    HasHighBloodPressure = false
                },
                new VitalsHistoryDto
                {
                    RecordedDate = DateTime.Now.AddDays(-30),
                    Height = 175.5m,
                    Weight = 74.5m,
                    BMI = 24.2m,
                    BloodPressure = "118/78",
                    PulseRate = 70,
                    IsDiabetic = false,
                    HasHighBloodPressure = false
                },
                new VitalsHistoryDto
                {
                    RecordedDate = DateTime.Now.AddDays(-60),
                    Height = 175.5m,
                    Weight = 74.0m,
                    BMI = 24.0m,
                    BloodPressure = "115/75",
                    PulseRate = 68,
                    IsDiabetic = false,
                    HasHighBloodPressure = false
                }
            };

            // Load chronic diseases
            ChronicDiseases = new List<ChronicDiseaseDetailDto>
            {
                new ChronicDiseaseDetailDto
                {
                    ChronicDiseaseID = 1,
                    DiseaseName = "Asthma",
                    DiagnosedDate = DateTime.Now.AddYears(-2),
                    CurrentStatus = "Controlled"
                }
            };

            // Load prescriptions history
            PrescriptionsHistory = new List<PrescriptionDetailDto>
            {
                new PrescriptionDetailDto
                {
                    PrescriptionID = 1,
                    DoctorName = "Dr. Jane Smith",
                    CreatedOn = DateTime.Now.AddDays(-30),
                    Notes = "For seasonal allergies",
                    Items = new List<PrescriptionItemDto>
                    {
                        new PrescriptionItemDto
                        {
                            MedicineName = "Cetirizine",
                            Dosage = "10mg",
                            Frequency = "Once Daily",
                            DurationDays = 10,
                            Instructions = "Take before bedtime"
                        }
                    }
                },
                new PrescriptionDetailDto
                {
                    PrescriptionID = 2,
                    DoctorName = "Dr. John Doe",
                    CreatedOn = DateTime.Now.AddDays(-60),
                    Notes = "Common cold treatment",
                    Items = new List<PrescriptionItemDto>
                    {
                        new PrescriptionItemDto
                        {
                            MedicineName = "Paracetamol",
                            Dosage = "500mg",
                            Frequency = "Three Times Daily",
                            DurationDays = 5,
                            Instructions = "Take after meals"
                        },
                        new PrescriptionItemDto
                        {
                            MedicineName = "Vitamin C",
                            Dosage = "1000mg",
                            Frequency = "Once Daily",
                            DurationDays = 7,
                            Instructions = "Take with water"
                        }
                    }
                }
            };

            // Load appointments history
            AppointmentsHistory = new List<AppointmentHistoryDto>
            {
                new AppointmentHistoryDto
                {
                    AppointmentID = 1,
                    AppointmentDate = DateTime.Now.AddDays(-7),
                    AppointmentType = "General Consultation",
                    Reason = "Routine checkup",
                    Status = "Completed"
                },
                new AppointmentHistoryDto
                {
                    AppointmentID = 2,
                    AppointmentDate = DateTime.Now.AddDays(-30),
                    AppointmentType = "Follow-up",
                    Reason = "Asthma management",
                    Status = "Completed"
                },
                new AppointmentHistoryDto
                {
                    AppointmentID = 3,
                    AppointmentDate = DateTime.Now.AddDays(7),
                    AppointmentType = "General Consultation",
                    Reason = "Annual physical exam",
                    Status = "Scheduled"
                }
            };
        }

        public async Task<IActionResult> OnPostAddVitalsAsync(
            int patientId,
            decimal height,
            decimal weight,
            string bloodPressure,
            int pulseRate,
            bool isDiabetic,
            bool hasHighBloodPressure)
        {
            try
            {
                _logger.LogInformation("Adding vitals record for patient {PatientId}", patientId);
                
                // TODO: Save to T_PatientVitals table
                // Calculate BMI: weight (kg) / (height (m))^2
                decimal heightInMeters = height / 100;
                decimal bmi = weight / (heightInMeters * heightInMeters);
                
                TempData["Success"] = "Vitals record added successfully!";
                return RedirectToPage("/Doctor/PatientDetails", new { patientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vitals for patient {PatientId}", patientId);
                TempData["Error"] = "Failed to add vitals record. Please try again.";
                return RedirectToPage("/Doctor/PatientDetails", new { patientId });
            }
        }

        public async Task<IActionResult> OnPostAddChronicDiseaseAsync(
            int patientId,
            string diseaseName,
            DateTime diagnosedDate,
            string currentStatus)
        {
            try
            {
                _logger.LogInformation("Adding chronic disease for patient {PatientId}: {DiseaseName}", patientId, diseaseName);
                
                // TODO: Save to T_ChronicDiseases table
                
                TempData["Success"] = "Chronic disease added successfully!";
                return RedirectToPage("/Doctor/PatientDetails", new { patientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding chronic disease for patient {PatientId}", patientId);
                TempData["Error"] = "Failed to add chronic disease. Please try again.";
                return RedirectToPage("/Doctor/PatientDetails", new { patientId });
            }
        }
    }

    // DTOs for patient details
    public class VitalsHistoryDto
    {
        public DateTime RecordedDate { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal BMI { get; set; }
        public string? BloodPressure { get; set; }
        public int? PulseRate { get; set; }
        public bool IsDiabetic { get; set; }
        public bool HasHighBloodPressure { get; set; }
    }

    public class ChronicDiseaseDetailDto
    {
        public int ChronicDiseaseID { get; set; }
        public string? DiseaseName { get; set; }
        public DateTime? DiagnosedDate { get; set; }
        public string? CurrentStatus { get; set; }
    }

    public class PrescriptionDetailDto
    {
        public int PrescriptionID { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? Notes { get; set; }
        public List<PrescriptionItemDto> Items { get; set; } = new();
    }

    public class PrescriptionItemDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public string? Instructions { get; set; }
    }

    public class AppointmentHistoryDto
    {
        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
