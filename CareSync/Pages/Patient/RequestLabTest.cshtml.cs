using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class RequestLabTestModel : BasePageModel
    {
        private readonly ILogger<RequestLabTestModel> _logger;

        public RequestLabTestModel(ILogger<RequestLabTestModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string TestCategory { get; set; } = string.Empty;

        [BindProperty]
        public string SpecificTest { get; set; } = string.Empty;

        [BindProperty]
        public string Reason { get; set; } = string.Empty;

        [BindProperty]
        public string? Symptoms { get; set; }

        [BindProperty]
        public string RequestedBy { get; set; } = "self";

        [BindProperty]
        public int? DoctorID { get; set; }

        [BindProperty]
        public int? PreferredLabCenterID { get; set; }

        [BindProperty]
        public DateTime? PreferredDate { get; set; }

        [BindProperty]
        public string? PreferredTime { get; set; }

        [BindProperty]
        public string? SpecialInstructions { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            return Page();
        }

        public IActionResult OnPost()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // TODO: Implement lab test request logic
                // 1. Get current patient ID from User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                // 2. Create T_LabRequests entity
                // 3. If appointment needed, create T_Appointments
                // 4. Save to database
                // 5. Send notification to patient and lab
                // 6. Send confirmation email

                _logger.LogInformation($"Lab test request created: {SpecificTest} by patient");

                TempData["SuccessMessage"] = "Lab test request submitted successfully! We will contact you to schedule your appointment.";
                return RedirectToPage("/Patient/LabResults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lab test request");
                ModelState.AddModelError(string.Empty, "An error occurred while submitting your request. Please try again.");
                return Page();
            }
        }
    }
}
