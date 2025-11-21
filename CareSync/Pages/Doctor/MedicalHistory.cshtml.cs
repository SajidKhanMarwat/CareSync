using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor;

public class MedicalHistoryModel : BasePageModel
{
    private readonly ILogger<MedicalHistoryModel> _logger;

    public MedicalHistoryModel(ILogger<MedicalHistoryModel> logger)
    {
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public int PatientId { get; set; }

    // Patient Information
    public PatientInfoDto Patient { get; set; } = new();

    // Statistics
    public int TotalVisits { get; set; }
    public int TotalPrescriptions { get; set; }
    public int TotalLabTests { get; set; }
    public int TotalReports { get; set; }

    // Medical Data
    public VitalsDto? CurrentVitals { get; set; }
    public List<VisitHistoryDto> VisitHistory { get; set; } = new();
    public List<PrescriptionDto> Prescriptions { get; set; } = new();
    public List<LabTestDto> LabTests { get; set; } = new();
    public List<MedicalHistoryChronicDiseaseDto> ChronicDiseases { get; set; } = new();
    public List<AllergyDto> Allergies { get; set; } = new();
    public string FamilyHistory { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated and has Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (PatientId <= 0)
        {
            TempData["Error"] = "Invalid patient ID.";
            return RedirectToPage("/Doctor/Patients");
        }

        try
        {
            await LoadPatientMedicalHistory();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient medical history for patient {PatientId}", PatientId);
            TempData["Error"] = "Unable to load patient medical history. Please try again.";
            return RedirectToPage("/Doctor/Patients");
        }
    }

    public async Task<IActionResult> OnPostQuickLabRequestAsync(int patientId, int labId, List<int> selectedTests, string priority, string clinicalNotes)
    {
        if (labId <= 0)
        {
            TempData["Error"] = "Please select a laboratory.";
            return RedirectToPage(new { patientId });
        }

        if (selectedTests == null || !selectedTests.Any())
        {
            TempData["Error"] = "Please select at least one test.";
            return RedirectToPage(new { patientId });
        }

        if (string.IsNullOrWhiteSpace(clinicalNotes))
        {
            TempData["Error"] = "Clinical notes are required.";
            return RedirectToPage(new { patientId });
        }

        try
        {
            // Get current doctor ID (mock - replace with actual user context)
            var doctorId = 1; // TODO: Get from authentication context

            // Create lab requests for each selected test
            foreach (var testId in selectedTests)
            {
                await CreateQuickLabRequest(doctorId, patientId, labId, testId, priority, clinicalNotes);
            }

            _logger.LogInformation("Quick lab request created successfully for patient {PatientId} by doctor {DoctorId}. Lab: {LabId}, Tests: {TestCount}", 
                patientId, doctorId, labId, selectedTests.Count);

            TempData["Success"] = $"Lab test request submitted successfully! {selectedTests.Count} test(s) requested.";
            return RedirectToPage(new { patientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quick lab request for patient {PatientId}", patientId);
            TempData["Error"] = "Failed to submit lab request. Please try again.";
            return RedirectToPage(new { patientId });
        }
    }

    private async Task CreateQuickLabRequest(int doctorId, int patientId, int labId, int testId, string priority, string clinicalNotes)
    {
        // TODO: Replace with actual database insert
        // This would create a record in T_LabRequests table
        
        _logger.LogInformation("Creating quick lab request - Doctor: {DoctorId}, Patient: {PatientId}, Lab: {LabId}, Test: {TestId}, Priority: {Priority}",
            doctorId, patientId, labId, testId, priority);

        /* Example database insert:
        var labRequest = new T_LabRequests
        {
            AppointmentID = 0,
            LabServiceID = testId,
            RequestedByDoctorID = doctorId,
            RequestedByPatientID = null,
            Status = "Pending",
            Remarks = $"Lab ID: {labId}\n\nPriority: {priority}\n\nClinical Notes: {clinicalNotes}",
            CreatedOn = DateTime.Now,
            CreatedBy = doctorId.ToString()
        };
        
        await _context.T_LabRequests.AddAsync(labRequest);
        await _context.SaveChangesAsync();
        */

        await Task.CompletedTask;
    }

    private async Task LoadPatientMedicalHistory()
    {
        // Load patient information (mock data - replace with actual database queries)
        Patient = new PatientInfoDto
        {
            PatientID = PatientId,
            Name = "John Smith",
            Age = 45,
            Gender = "Male",
            BloodGroup = "O+",
            Phone = "+1 (555) 123-4567",
            Email = "john.smith@email.com",
            Address = "123 Main Street, New York, NY 10001"
        };

        // Load statistics
        TotalVisits = 24;
        TotalPrescriptions = 18;
        TotalLabTests = 12;
        TotalReports = 8;

        // Load current vitals
        CurrentVitals = new VitalsDto
        {
            BloodPressure = "120/80",
            HeartRate = 72,
            Temperature = 98.6m,
            Weight = 75.5m,
            Height = 175,
            BMI = 24.6m,
            LastUpdated = DateTime.Now.AddDays(-7)
        };

        // Load visit history
        VisitHistory = new List<VisitHistoryDto>
        {
            new VisitHistoryDto
            {
                VisitID = 1,
                VisitDate = DateTime.Now.AddDays(-7),
                VisitType = "Regular Checkup",
                ChiefComplaint = "Annual physical examination",
                Diagnosis = "Patient in good health. Mild hypertension noted.",
                Treatment = "Continue current medication. Monitor blood pressure.",
                Notes = "Patient reports feeling well. No new concerns."
            },
            new VisitHistoryDto
            {
                VisitID = 2,
                VisitDate = DateTime.Now.AddDays(-45),
                VisitType = "Follow-up",
                ChiefComplaint = "Follow-up for hypertension management",
                Diagnosis = "Hypertension controlled with medication",
                Treatment = "Continue Lisinopril 10mg daily. Lifestyle modifications advised.",
                Notes = "Blood pressure improved. Patient adhering to medication."
            },
            new VisitHistoryDto
            {
                VisitID = 3,
                VisitDate = DateTime.Now.AddDays(-90),
                VisitType = "Consultation",
                ChiefComplaint = "Persistent headaches and dizziness",
                Diagnosis = "Hypertension diagnosed. No other abnormalities found.",
                Treatment = "Started on Lisinopril 10mg daily. Advised to reduce salt intake.",
                Notes = "Initial diagnosis of hypertension. Patient counseled on lifestyle changes."
            }
        };

        // Load prescriptions
        Prescriptions = new List<PrescriptionDto>
        {
            new PrescriptionDto
            {
                PrescriptionID = 1,
                PrescriptionDate = DateTime.Now.AddDays(-7),
                Diagnosis = "Hypertension",
                Medications = new List<string> { "Lisinopril 10mg", "Aspirin 81mg" },
                IsActive = true
            },
            new PrescriptionDto
            {
                PrescriptionID = 2,
                PrescriptionDate = DateTime.Now.AddDays(-45),
                Diagnosis = "Hypertension",
                Medications = new List<string> { "Lisinopril 10mg" },
                IsActive = false
            },
            new PrescriptionDto
            {
                PrescriptionID = 3,
                PrescriptionDate = DateTime.Now.AddDays(-90),
                Diagnosis = "Hypertension - Initial",
                Medications = new List<string> { "Lisinopril 5mg" },
                IsActive = false
            }
        };

        // Load lab tests
        LabTests = new List<LabTestDto>
        {
            new LabTestDto
            {
                TestID = 1,
                TestDate = DateTime.Now.AddDays(-14),
                TestType = "Blood Test",
                Status = "Completed",
                IsAbnormal = false
            },
            new LabTestDto
            {
                TestID = 2,
                TestDate = DateTime.Now.AddDays(-30),
                TestType = "Urine Test",
                Status = "Completed",
                IsAbnormal = false
            },
            new LabTestDto
            {
                TestID = 3,
                TestDate = DateTime.Now.AddDays(-60),
                TestType = "X-Ray",
                Status = "Completed",
                IsAbnormal = false
            }
        };

        // Load chronic diseases
        ChronicDiseases = new List<MedicalHistoryChronicDiseaseDto>
        {
            new MedicalHistoryChronicDiseaseDto
            {
                DiseaseID = 1,
                DiseaseName = "Hypertension",
                DiagnosedDate = DateTime.Now.AddMonths(-3)
            },
            new MedicalHistoryChronicDiseaseDto
            {
                DiseaseID = 2,
                DiseaseName = "Type 2 Diabetes",
                DiagnosedDate = DateTime.Now.AddYears(-2)
            }
        };

        // Load allergies
        Allergies = new List<AllergyDto>
        {
            new AllergyDto
            {
                AllergyID = 1,
                AllergyName = "Penicillin",
                Severity = "Severe"
            },
            new AllergyDto
            {
                AllergyID = 2,
                AllergyName = "Peanuts",
                Severity = "Moderate"
            }
        };

        // Family history
        FamilyHistory = "Father: Hypertension, Diabetes. Mother: Healthy. Brother: Asthma.";

        // Doctor notes
        DoctorNotes = "Patient is cooperative and follows medical advice. Regular monitoring required for hypertension management.";
    }
}

// DTOs
public class PatientInfoDto
{
    public int PatientID { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class VitalsDto
{
    public string BloodPressure { get; set; } = string.Empty;
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }
    public decimal Weight { get; set; }
    public int Height { get; set; }
    public decimal BMI { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class VisitHistoryDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitType { get; set; } = string.Empty;
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class PrescriptionDto
{
    public int PrescriptionID { get; set; }
    public DateTime PrescriptionDate { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public List<string> Medications { get; set; } = new();
    public bool IsActive { get; set; }
}

public class LabTestDto
{
    public int TestID { get; set; }
    public DateTime TestDate { get; set; }
    public string TestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsAbnormal { get; set; }
}

public class MedicalHistoryChronicDiseaseDto
{
    public int DiseaseID { get; set; }
    public string DiseaseName { get; set; } = string.Empty;
    public DateTime DiagnosedDate { get; set; }
}

public class AllergyDto
{
    public int AllergyID { get; set; }
    public string AllergyName { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}
