using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Patient
{
    public class BookAppointmentModel : BasePageModel
    {
        private readonly ILogger<BookAppointmentModel> _logger;

        public BookAppointmentModel(ILogger<BookAppointmentModel> logger)
        {
            _logger = logger;
        }

        [FromQuery(Name = "doctorId")]
        public int? DoctorId { get; set; }

        [FromQuery(Name = "date")]
        public DateTime? PreferredDate { get; set; }

        [FromQuery(Name = "time")]
        public string? PreferredTime { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            // If doctorId is provided via URL, it will be available in DoctorId property
            // This allows pre-selection from dashboard doctor cards
            if (DoctorId.HasValue)
            {
                _logger.LogInformation($"Booking page loaded with pre-selected doctor ID: {DoctorId}");
            }

            // TODO: Load available doctors from database
            // TODO: Load doctor availability schedules
            // TODO: If doctorId provided, load that specific doctor's details

            return Page();
        }

        public IActionResult OnPost()
        {
            // Check if user is authenticated and has Patient role
            var authResult = RequireRole("Patient");
            if (authResult != null) return authResult;

            // TODO: Handle appointment booking
            // TODO: Create T_Appointments record
            // TODO: Send confirmation email/SMS
            // TODO: Add to patient's appointment list

            return RedirectToPage("/Patient/Appointments");
        }
    }
}
