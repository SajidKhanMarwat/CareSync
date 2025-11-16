using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor
{
    public class LabRequestModel : BasePageModel
    {
        private readonly ILogger<LabRequestModel> _logger;

        public LabRequestModel(ILogger<LabRequestModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public LabRequestDto Request { get; set; } = new();

        public List<PatientSelectDto> Patients { get; set; } = new();
        public List<LabDto> AvailableLabs { get; set; } = new();
        public List<LabTestServiceDto> AvailableTests { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? patientId, int? appointmentId)
        {
            // Check if user is authenticated and has Doctor role
            var authResult = RequireRole("Doctor");
            if (authResult != null) return authResult;

            try
            {
                await LoadFormData();
                
                // Pre-select patient if provided
                if (patientId.HasValue)
                {
                    Request.PatientID = patientId.Value;
                }
                
                // Pre-select appointment if provided
                if (appointmentId.HasValue)
                {
                    Request.AppointmentID = appointmentId.Value;
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lab request form");
                TempData["Error"] = "Unable to load form. Please try again.";
                return RedirectToPage("/Doctor/Dashboard");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadFormData();
                return Page();
            }

            try
            {
                // Validate lab selection
                if (!Request.LabID.HasValue || Request.LabID.Value <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Please select a laboratory.");
                    await LoadFormData();
                    return Page();
                }

                // Validate at least one test is selected
                if (Request.SelectedTestIds == null || !Request.SelectedTestIds.Any())
                {
                    ModelState.AddModelError(string.Empty, "Please select at least one test.");
                    await LoadFormData();
                    return Page();
                }

                // Get current doctor ID (mock - replace with actual user context)
                var doctorId = 1; // TODO: Get from authentication context

                // Create lab requests for each selected test
                foreach (var testId in Request.SelectedTestIds)
                {
                    await CreateLabRequest(doctorId, Request.LabID.Value, testId);
                }

                _logger.LogInformation("Lab request created successfully for patient {PatientId} by doctor {DoctorId}. Lab: {LabId}, Tests: {TestCount}", 
                    Request.PatientID, doctorId, Request.LabID.Value, Request.SelectedTestIds.Count);

                TempData["Success"] = $"Lab test request submitted successfully! {Request.SelectedTestIds.Count} test(s) requested.";
                
                // Redirect based on context
                if (Request.AppointmentID.HasValue)
                {
                    return RedirectToPage("/Doctor/Appointments");
                }
                else
                {
                    return RedirectToPage("/Doctor/MedicalHistory", new { patientId = Request.PatientID });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lab request for patient {PatientId}", Request.PatientID);
                ModelState.AddModelError(string.Empty, "Failed to submit lab request. Please try again.");
                await LoadFormData();
                return Page();
            }
        }

        public async Task<IActionResult> OnGetLabServicesAsync(int labId)
        {
            // AJAX endpoint to get lab services for a specific lab
            var services = await GetLabServices(labId);
            return new JsonResult(services);
        }

        private async Task LoadFormData()
        {
            // Load patients (mock data - replace with actual database query)
            Patients = new List<PatientSelectDto>
            {
                new PatientSelectDto { PatientID = 1, Name = "John Smith", Age = 45, Gender = "Male" },
                new PatientSelectDto { PatientID = 2, Name = "Mary Johnson", Age = 38, Gender = "Female" },
                new PatientSelectDto { PatientID = 3, Name = "Robert Davis", Age = 52, Gender = "Male" },
                new PatientSelectDto { PatientID = 4, Name = "Lisa Anderson", Age = 29, Gender = "Female" },
                new PatientSelectDto { PatientID = 5, Name = "Michael Brown", Age = 67, Gender = "Male" }
            };

            // Load available labs (mock data - replace with actual database query)
            AvailableLabs = new List<LabDto>
            {
                new LabDto
                {
                    LabID = 1,
                    LabName = "City Central Laboratory",
                    Location = "123 Medical Center Dr, Downtown",
                    ContactNumber = "+1 (555) 100-2000",
                    OpeningTime = "08:00 AM",
                    ClosingTime = "08:00 PM"
                },
                new LabDto
                {
                    LabID = 2,
                    LabName = "HealthFirst Diagnostics",
                    Location = "456 Health Plaza, Medical District",
                    ContactNumber = "+1 (555) 200-3000",
                    OpeningTime = "07:00 AM",
                    ClosingTime = "10:00 PM"
                },
                new LabDto
                {
                    LabID = 3,
                    LabName = "Advanced Medical Lab",
                    Location = "789 Laboratory Ave, Science Park",
                    ContactNumber = "+1 (555) 300-4000",
                    OpeningTime = "24/7",
                    ClosingTime = "24/7"
                }
            };

            // Initially load tests for first lab or none
            // Tests will be loaded via AJAX when lab is selected
            AvailableTests = new List<LabTestServiceDto>();
        }

        private async Task<List<LabTestServiceDto>> GetLabServices(int labId)
        {
            // Mock data - replace with actual database query filtered by LabID
            // SELECT * FROM T_LabServices WHERE LabID = @labId AND IsDeleted = false
            
            var allServices = new List<LabTestServiceDto>
            {
                new LabTestServiceDto
                {
                    LabServiceID = 1,
                    ServiceName = "Complete Blood Count (CBC)",
                    Description = "Comprehensive blood test measuring red cells, white cells, and platelets",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 45.00m,
                    EstimatedTime = "2-4 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 2,
                    ServiceName = "Lipid Profile",
                    Description = "Cholesterol and triglyceride levels",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 55.00m,
                    EstimatedTime = "4-6 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 3,
                    ServiceName = "Blood Glucose (Fasting)",
                    Description = "Measures blood sugar levels",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 25.00m,
                    EstimatedTime = "1-2 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 4,
                    ServiceName = "HbA1c",
                    Description = "Average blood sugar over 3 months",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 40.00m,
                    EstimatedTime = "2-4 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 5,
                    ServiceName = "Liver Function Test (LFT)",
                    Description = "Checks liver enzymes and function",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 60.00m,
                    EstimatedTime = "4-6 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 6,
                    ServiceName = "Kidney Function Test (KFT)",
                    Description = "Checks kidney health and function",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 50.00m,
                    EstimatedTime = "4-6 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 7,
                    ServiceName = "Thyroid Panel (TSH, T3, T4)",
                    Description = "Comprehensive thyroid hormone testing",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 75.00m,
                    EstimatedTime = "6-8 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 8,
                    ServiceName = "Urinalysis",
                    Description = "Complete urine examination",
                    Category = "Urine Test",
                    SampleType = "Urine",
                    Price = 30.00m,
                    EstimatedTime = "1-2 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 9,
                    ServiceName = "Urine Culture",
                    Description = "Detects bacteria in urine",
                    Category = "Microbiology",
                    SampleType = "Urine",
                    Price = 65.00m,
                    EstimatedTime = "24-48 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 10,
                    ServiceName = "Chest X-Ray",
                    Description = "Imaging of chest and lungs",
                    Category = "Imaging",
                    SampleType = "N/A",
                    Price = 85.00m,
                    EstimatedTime = "30 minutes"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 11,
                    ServiceName = "ECG (Electrocardiogram)",
                    Description = "Heart rhythm and electrical activity",
                    Category = "Imaging",
                    SampleType = "N/A",
                    Price = 70.00m,
                    EstimatedTime = "15 minutes"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 12,
                    ServiceName = "Vitamin D Test",
                    Description = "Measures Vitamin D levels",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 55.00m,
                    EstimatedTime = "4-6 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 13,
                    ServiceName = "COVID-19 PCR Test",
                    Description = "PCR test for COVID-19 detection",
                    Category = "Microbiology",
                    SampleType = "Nasal Swab",
                    Price = 95.00m,
                    EstimatedTime = "6-12 hours"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 14,
                    ServiceName = "Blood Group & Rh Type",
                    Description = "Determines blood type",
                    Category = "Blood Test",
                    SampleType = "Blood",
                    Price = 20.00m,
                    EstimatedTime = "1 hour"
                },
                new LabTestServiceDto
                {
                    LabServiceID = 15,
                    ServiceName = "Stool Culture",
                    Description = "Detects infections in digestive system",
                    Category = "Microbiology",
                    SampleType = "Stool",
                    Price = 70.00m,
                    EstimatedTime = "48-72 hours"
                }
            };

            // Filter services by lab
            // Lab 1 (City Central Laboratory) - General tests
            // Lab 2 (HealthFirst Diagnostics) - Comprehensive tests
            // Lab 3 (Advanced Medical Lab) - All tests including specialized
            
            if (labId == 1)
            {
                // City Central Laboratory - Basic tests
                return allServices.Where(s => new[] { 1, 3, 8, 14 }.Contains(s.LabServiceID)).ToList();
            }
            else if (labId == 2)
            {
                // HealthFirst Diagnostics - Most tests
                return allServices.Where(s => !new[] { 13, 15 }.Contains(s.LabServiceID)).ToList();
            }
            else if (labId == 3)
            {
                // Advanced Medical Lab - All tests
                return allServices;
            }
            
            return new List<LabTestServiceDto>();
        }

        private async Task CreateLabRequest(int doctorId, int labId, int testId)
        {
            // TODO: Replace with actual database insert
            // This would create a record in T_LabRequests table
            
            _logger.LogInformation("Creating lab request - Doctor: {DoctorId}, Patient: {PatientId}, Lab: {LabId}, Test: {TestId}, Priority: {Priority}",
                doctorId, Request.PatientID, labId, testId, Request.Priority);

            /* Example database insert:
            var labRequest = new T_LabRequests
            {
                AppointmentID = Request.AppointmentID ?? 0,
                LabServiceID = testId,
                RequestedByDoctorID = doctorId,
                RequestedByPatientID = null,
                Status = "Pending",
                Remarks = $"Lab ID: {labId}\n\n{Request.ClinicalNotes}\n\nPriority: {Request.Priority}\n\nSpecial Instructions: {Request.SpecialInstructions}",
                CreatedOn = DateTime.Now,
                CreatedBy = doctorId.ToString()
            };
            
            await _context.T_LabRequests.AddAsync(labRequest);
            await _context.SaveChangesAsync();
            */

            await Task.CompletedTask;
        }
    }

    // DTOs
    public class LabRequestDto
    {
        public int? LabID { get; set; }
        public int PatientID { get; set; }
        public int? AppointmentID { get; set; }
        public List<int> SelectedTestIds { get; set; } = new();
        public string Priority { get; set; } = "Routine";
        public DateTime? PreferredDate { get; set; }
        public string ClinicalNotes { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public bool SendCopyToPatient { get; set; }
    }

    public class PatientSelectDto
    {
        public int PatientID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
    }

    public class LabTestServiceDto
    {
        public int LabServiceID { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string EstimatedTime { get; set; } = string.Empty;
    }

    public class LabDto
    {
        public int LabID { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string OpeningTime { get; set; } = string.Empty;
        public string ClosingTime { get; set; } = string.Empty;
    }
}
