using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using System.Text.Json;

namespace CareSync.Pages.Patient;

[Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryToken]
public class BookAppointmentModel : BasePageModel
{
    private readonly ILogger<BookAppointmentModel> _logger;
    private readonly PatientApiService _patientApiService;

    public BookAppointmentModel(
        ILogger<BookAppointmentModel> logger,
        PatientApiService patientApiService)
    {
        _logger = logger;
        _patientApiService = patientApiService;
    }

    [FromQuery(Name = "doctorId")]
    public int? DoctorId { get; set; }

    [FromQuery(Name = "date")]
    public DateTime? PreferredDate { get; set; }

    [FromQuery(Name = "time")]
    public string? PreferredTime { get; set; }

    // Properties for the page
    public List<DoctorBooking_DTO> AvailableDoctors { get; set; } = new();
    public DoctorBooking_DTO? SelectedDoctor { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated and has Patient role
        var authResult = RequireRole("Patient");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading book appointment page");

            // Load all available doctors
            var doctorsResult = await _patientApiService.GetAvailableDoctorsAsync();
            
            if (doctorsResult.IsSuccess && doctorsResult.Data != null)
            {
                AvailableDoctors = doctorsResult.Data;
                _logger.LogInformation($"Loaded {AvailableDoctors.Count} available doctors");

                // If doctorId is provided, load that specific doctor
                if (DoctorId.HasValue)
                {
                    _logger.LogInformation($"Pre-selecting doctor ID: {DoctorId}");
                    SelectedDoctor = AvailableDoctors.FirstOrDefault(d => d.DoctorID == DoctorId.Value);
                    
                    if (SelectedDoctor == null)
                    {
                        // Try to load from API if not in the list
                        var doctorResult = await _patientApiService.GetDoctorByIdAsync(DoctorId.Value);
                        if (doctorResult.IsSuccess && doctorResult.Data != null)
                        {
                            SelectedDoctor = doctorResult.Data;
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning($"Failed to load doctors: {doctorsResult.GetError()}");
                TempData["ErrorMessage"] = "Unable to load available doctors. Please try again later.";  
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading book appointment page");
            TempData["ErrorMessage"] = "An error occurred while loading the page.";
        }

        return Page();
    }

    /// <summary>
    /// API endpoint to get available doctors (called from JavaScript)
    /// </summary>
    public async Task<IActionResult> OnGetDoctorsAsync([FromQuery] string? specialization = null)
    {
        try
        {
            var result = await _patientApiService.GetAvailableDoctorsAsync(specialization);
            
            if (result.IsSuccess)
            {
                return new JsonResult(result.Data);
            }
            
            return new JsonResult(new { error = result.GetError() }) { StatusCode = 400 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors");
            return new JsonResult(new { error = "Failed to load doctors" }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// API endpoint to get doctor time slots (called from JavaScript)
    /// </summary>
    public async Task<IActionResult> OnGetTimeSlotsAsync([FromQuery] int doctorId, [FromQuery] DateTime date)
    {
        try
        {
            var result = await _patientApiService.GetDoctorTimeSlotsAsync(doctorId, date);
            
            if (result.IsSuccess)
            {
                return new JsonResult(result.Data);
            }
            
            return new JsonResult(new { error = result.GetError() }) { StatusCode = 400 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time slots");
            return new JsonResult(new { error = "Failed to load time slots" }) { StatusCode = 500 };
        }
    }

    /// <summary>
    /// Handle appointment booking submission
    /// </summary>
    public async Task<IActionResult> OnPostBookAppointmentAsync([FromBody] BookAppointmentRequest_DTO request)
    {
        try
        {
            _logger.LogInformation($"Received booking request: DoctorID={request.DoctorID}, Date={request.AppointmentDate}, Time={request.AppointmentTime}");
            
            if (request == null)
            {
                _logger.LogError("Booking request is null");
                return new JsonResult(new { success = false, message = "Invalid request data" }) { StatusCode = 400 };
            }

            var result = await _patientApiService.BookAppointmentAsync(request);
            
            if (result.IsSuccess && result.Data != null && result.Data.Success)
            {
                _logger.LogInformation("Appointment booked successfully");
                return new JsonResult(new 
                { 
                    success = true, 
                    message = result.Data.Message 
                });
            }
            
            _logger.LogWarning($"Booking failed: {result.GetError()}");
            return new JsonResult(new 
            { 
                success = false, 
                message = result.Data?.Message ?? result.GetError() 
            }) { StatusCode = 400 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            return new JsonResult(new 
            { 
                success = false, 
                message = "An error occurred while booking the appointment. Please try again." 
            }) { StatusCode = 500 };
        }
    }
}
