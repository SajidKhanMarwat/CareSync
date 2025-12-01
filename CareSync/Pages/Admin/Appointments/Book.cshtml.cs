using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Patient;
using CareSync.Shared.Enums.Appointment;

namespace CareSync.Pages.Admin.Appointments;

public class BookAppointmentModel : BasePageModel
{
    private readonly ILogger<BookAppointmentModel> _logger;
    private readonly AdminApiService _adminApiService;

    public BookAppointmentModel(ILogger<BookAppointmentModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    // Available doctors and patients for dropdowns
    public List<DoctorList_DTO> AvailableDoctors { get; set; } = new();
    public List<PatientList_DTO> AllPatients { get; set; } = new();
    
    // Filter properties
    [BindProperty(SupportsGet = true)]
    public string? SpecializationFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public bool? ActiveOnlyFilter { get; set; } = true;
    
    // Available specializations for filter dropdown
    public List<string> AvailableSpecializations { get; set; } = new();
    
    // Form Models
    [BindProperty]
    public AddAppointmentWithQuickPatient_DTO QuickPatientAppointment { get; set; } = new()
    {
        FirstName = "",
        Email = "",
        DoctorID = 0,
        AppointmentDate = DateTime.Now.AddDays(1),
        AppointmentType = AppointmentType_Enum.InPerson
    };

    [BindProperty]
    public AddAppointment_DTO ExistingPatientAppointment { get; set; } = new()
    {
        DoctorID = 0,
        PatientID = 0,
        AppointmentDate = DateTime.Now.AddDays(1),
        AppointmentType = AppointmentType_Enum.InPerson,
        Reason = ""
    };

    [BindProperty]
    public bool UseQuickRegistration { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        _logger.LogInformation("Loading appointment booking page. Filters: Specialization={Specialization}, ActiveOnly={ActiveOnly}", 
            SpecializationFilter, ActiveOnlyFilter);

        await LoadDropdownDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAppointmentAsync()
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Creating appointment. UseQuickRegistration={UseQuickRegistration}", UseQuickRegistration);

            Result<GeneralResponse>? result;

            if (UseQuickRegistration)
            {
                // Validate quick patient form
                if (string.IsNullOrWhiteSpace(QuickPatientAppointment.FirstName) || 
                    string.IsNullOrWhiteSpace(QuickPatientAppointment.Email))
                {
                    ErrorMessage = "First name and email are required for quick registration.";
                    await LoadDropdownDataAsync();
                    return Page();
                }

                result = await _adminApiService.CreateAppointmentWithQuickPatientAsync<Result<GeneralResponse>>(QuickPatientAppointment);
            }
            else
            {
                // Validate existing patient form
                if (ExistingPatientAppointment.PatientID == 0 || ExistingPatientAppointment.DoctorID == 0)
                {
                    ErrorMessage = "Please select both patient and doctor.";
                    await LoadDropdownDataAsync();
                    return Page();
                }

                result = await _adminApiService.CreateAppointmentAsync<Result<GeneralResponse>>(ExistingPatientAppointment);
            }

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToPage("/Admin/Appointments");
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to create appointment.";
                await LoadDropdownDataAsync();
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            ErrorMessage = "An error occurred while creating the appointment.";
            await LoadDropdownDataAsync();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostSearchPatientAsync([FromForm] string searchTerm)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Received search term: {SearchTerm}", searchTerm);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new JsonResult(new { success = false, message = "Search term is required" });
            }

            _logger.LogInformation("Searching for patient with term: {SearchTerm}", searchTerm);

            var result = await _adminApiService.SearchPatientsAsync<Result<List<PatientSearch_DTO>>>(searchTerm);

            if (result?.IsSuccess == true && result.Data != null && result.Data.Any())
            {
                var patient = result.Data.First(); // Take first match
                _logger.LogInformation("Patient found: {Name}", patient.FullName);
                
                return new JsonResult(new
                {
                    success = true,
                    patient = new
                    {
                        patientId = patient.PatientID,
                        userId = patient.UserID,
                        fullName = patient.FullName,
                        email = patient.Email,
                        phoneNumber = patient.PhoneNumber,
                        age = patient.Age,
                        bloodGroup = patient.BloodGroup,
                        lastVisit = patient.LastVisit?.ToString("yyyy-MM-dd")
                    }
                });
            }
            else
            {
                _logger.LogInformation("No patient found for search term: {SearchTerm}", searchTerm);
                return new JsonResult(new { success = false, message = "No patient found with the provided information" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patient: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> OnPostBookAppointmentAsync(
        [FromForm] int doctorId,
        [FromForm] int patientId,
        [FromForm] string appointmentDate,
        [FromForm] string appointmentTime,
        [FromForm] string appointmentType,
        [FromForm] string reason,
        [FromForm] string? notes)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Booking appointment - Doctor: {DoctorId}, Patient: {PatientId}, Date: {Date}, Time: {Time}", 
                doctorId, patientId, appointmentDate, appointmentTime);

            // Validate required fields
            if (doctorId == 0 || patientId == 0 || string.IsNullOrWhiteSpace(appointmentDate) || 
                string.IsNullOrWhiteSpace(appointmentTime) || string.IsNullOrWhiteSpace(appointmentType))
            {
                return new JsonResult(new { success = false, message = "All required fields must be provided" });
            }

            // Parse date and time
            if (!DateTime.TryParse(appointmentDate, out var parsedDate))
            {
                return new JsonResult(new { success = false, message = "Invalid appointment date" });
            }

            // Combine date and time
            if (TimeSpan.TryParse(appointmentTime, out var time))
            {
                parsedDate = parsedDate.Date.Add(time);
            }

            // Parse appointment type enum
            if (!Enum.TryParse<AppointmentType_Enum>(appointmentType, out var type))
            {
                return new JsonResult(new { success = false, message = "Invalid appointment type" });
            }

            // Create appointment DTO
            var appointmentDto = new AddAppointment_DTO
            {
                DoctorID = doctorId,
                PatientID = patientId,
                AppointmentDate = parsedDate,
                AppointmentType = type,
                Status = AppointmentStatus_Enum.Scheduled,
                Reason = reason ?? "",
                Notes = notes
            };

            _logger.LogInformation("Calling API to create appointment");

            // Call API to create appointment
            var result = await _adminApiService.CreateAppointmentAsync<Result<GeneralResponse>>(appointmentDto);

            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Appointment created successfully");
                return new JsonResult(new 
                { 
                    success = true, 
                    message = "Appointment booked successfully!",
                    appointmentDate = parsedDate.ToString("yyyy-MM-dd"),
                    appointmentTime = parsedDate.ToString("hh:mm tt")
                });
            }
            else
            {
                var errorMessage = result?.GetError() ?? "Failed to create appointment";
                _logger.LogWarning("Appointment creation failed: {Error}", errorMessage);
                return new JsonResult(new { success = false, message = errorMessage });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> OnPostCreatePatientAsync(
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string username,
        [FromForm] string email,
        [FromForm] string phoneNumber,
        [FromForm] string dateOfBirth,
        [FromForm] string gender,
        [FromForm] string? bloodGroup,
        [FromForm] string? address,
        [FromForm] string? emergencyContactName,
        [FromForm] string? emergencyContactPhone)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Creating patient account - Name: {FirstName} {LastName}, Email: {Email}", 
                firstName, lastName, email);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(username))
            {
                return new JsonResult(new { success = false, message = "Required fields are missing" });
            }

            // Parse date of birth
            if (!DateTime.TryParse(dateOfBirth, out var dob))
            {
                return new JsonResult(new { success = false, message = "Invalid date of birth" });
            }

            // Parse gender
            if (!Enum.TryParse<Gender_Enum>(gender, out var genderEnum))
            {
                return new JsonResult(new { success = false, message = "Invalid gender" });
            }

            // Create DTO for quick patient creation using the existing registration endpoint
            var patientRegistration = new UserRegisteration_DTO
            {
                FirstName = firstName,
                LastName = lastName,
                ArabicFirstName = firstName, // Default to English for now
                ArabicLastName = lastName ?? "", // Default to English for now
                UserName = username,
                ArabicUserName = username, // Default to English for now
                Email = email,
                PhoneNumber = phoneNumber,
                Password = "CareSync@123", // Default password
                ConfirmPassword = "CareSync@123",
                Gender = genderEnum,
                DateOfBirth = dob,
                Address = address,
                RoleType = RoleType.Patient,
                IsActive = true,
                
                // Nested patient-specific details
                RegisterPatient = new RegisterPatient_DTO
                {
                    BloodGroup = bloodGroup,
                    MaritalStatus = MaritalStatusEnum.Single, // Default
                    EmergencyContactName = emergencyContactName,
                    EmergencyContactNumber = emergencyContactPhone,
                    CreatedBy = GetCurrentUserId() ?? "admin"
                }
            };

            _logger.LogInformation("Calling API to create patient account");

            // Call API to create patient using the existing registration endpoint
            var result = await _adminApiService.RegisterPatientAsync<Result<GeneralResponse>>(patientRegistration);

            _logger.LogInformation("API Result - IsSuccess: {IsSuccess}, Data: {Data}", result?.IsSuccess, result?.Data);
            
            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Patient created successfully");
                
                // Wait a moment for database to update
                await Task.Delay(100);
                
                // Search for the newly created patient to get their ID
                var searchResult = await _adminApiService.SearchPatientsAsync<Result<List<PatientSearch_DTO>>>(email);
                
                PatientSearch_DTO? patient = null;
                if (searchResult?.IsSuccess == true && searchResult.Data != null && searchResult.Data.Count > 0)
                {
                    patient = searchResult.Data[0];
                    _logger.LogInformation("Found patient with ID: {PatientID}", patient.PatientID);
                }
                else
                {
                    _logger.LogWarning("Could not find newly created patient with email: {Email}", email);
                }

                if (patient == null || patient.PatientID == 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Patient account was created but could not be retrieved. Please refresh and search for the patient."
                    });
                }

                return new JsonResult(new 
                { 
                    success = true, 
                    message = "Patient account created successfully!",
                    patientName = $"{firstName} {lastName}",
                    defaultPassword = "CareSync@123",
                    patient = new
                    {
                        patientId = patient.PatientID,
                        loginId = username,
                        email = email,
                        firstName = firstName,
                        lastName = lastName,
                        phone = phoneNumber
                    }
                });
            }
            else
            {
                var errorMessage = result?.GetError() ?? "Failed to create patient account";
                var detailedError = result?.Data?.Message ?? errorMessage;
                _logger.LogWarning("Patient creation failed: {Error}, Detailed: {DetailedError}", 
                    errorMessage, detailedError);
                return new JsonResult(new { success = false, message = detailedError });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    public async Task<IActionResult> OnPostCreatePatientAndBookAsync(
        [FromForm] string firstName,
        [FromForm] string lastName,
        [FromForm] string username,
        [FromForm] string email,
        [FromForm] string phoneNumber,
        [FromForm] string dateOfBirth,
        [FromForm] string gender,
        [FromForm] string? bloodGroup,
        [FromForm] string? address,
        [FromForm] string? emergencyContactName,
        [FromForm] string? emergencyContactPhone,
        [FromForm] int doctorId,
        [FromForm] string appointmentDate,
        [FromForm] string appointmentTime,
        [FromForm] string appointmentType,
        [FromForm] string? reason)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Creating patient and booking appointment - Name: {FirstName} {LastName}, Email: {Email}", 
                firstName, lastName, email);

            // Validate required fields
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(username) || doctorId == 0)
            {
                return new JsonResult(new { success = false, message = "Required fields are missing" });
            }

            // Parse date of birth
            if (!DateTime.TryParse(dateOfBirth, out var dob))
            {
                return new JsonResult(new { success = false, message = "Invalid date of birth" });
            }

            // Parse gender
            if (!Enum.TryParse<Gender_Enum>(gender, out var genderEnum))
            {
                return new JsonResult(new { success = false, message = "Invalid gender" });
            }

            // Parse appointment date and time
            if (!DateTime.TryParse(appointmentDate, out var apptDate))
            {
                return new JsonResult(new { success = false, message = "Invalid appointment date" });
            }

            // Combine date and time
            if (TimeSpan.TryParse(appointmentTime, out var time))
            {
                apptDate = apptDate.Date.Add(time);
            }

            // Parse appointment type
            if (!Enum.TryParse<AppointmentType_Enum>(appointmentType, out var apptType))
            {
                return new JsonResult(new { success = false, message = "Invalid appointment type" });
            }

            // Create quick patient with appointment DTO
            var quickPatientDto = new AddAppointmentWithQuickPatient_DTO
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                Email = email,
                PhoneNumber = phoneNumber,
                Gender = genderEnum,
                DateOfBirth = dob,
                BloodGroup = bloodGroup,
                Address = address,
                EmergencyContactName = emergencyContactName,
                EmergencyContactPhone = emergencyContactPhone,
                Password = "CareSync@123", // Default password
                DoctorID = doctorId,
                AppointmentDate = apptDate,
                AppointmentType = apptType,
                Reason = reason ?? "Walk-in appointment",
                Notes = "Created by admin during quick registration"
            };

            _logger.LogInformation("Calling API to create patient and appointment");

            // Call API to create patient and appointment
            var result = await _adminApiService.CreateAppointmentWithQuickPatientAsync<Result<GeneralResponse>>(quickPatientDto);

            _logger.LogInformation("API Result - IsSuccess: {IsSuccess}, Data: {Data}", result?.IsSuccess, result?.Data);
            
            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Patient and appointment created successfully");
                return new JsonResult(new 
                { 
                    success = true, 
                    message = "Patient account created and appointment booked successfully!",
                    patientName = $"{firstName} {lastName}",
                    appointmentDate = apptDate.ToString("yyyy-MM-dd"),
                    appointmentTime = apptDate.ToString("hh:mm tt"),
                    defaultPassword = "CareSync@123"
                });
            }
            else
            {
                var errorMessage = result?.GetError() ?? "Failed to create patient and appointment";
                var detailedError = result?.Data?.Message ?? errorMessage;
                _logger.LogWarning("Patient creation failed: {Error}, Detailed: {DetailedError}, Result: {Result}", 
                    errorMessage, detailedError, System.Text.Json.JsonSerializer.Serialize(result));
                return new JsonResult(new { success = false, message = detailedError });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient and appointment: {Message}", ex.Message);
            return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    private async Task LoadDropdownDataAsync()
    {
        try
        {
            // Load doctors with filters
            var doctorsResult = await _adminApiService.GetAllDoctorsAsync<Result<List<DoctorList_DTO>>>(
                SpecializationFilter, 
                ActiveOnlyFilter);
                
            if (doctorsResult?.IsSuccess == true && doctorsResult.Data != null)
            {
                AvailableDoctors = doctorsResult.Data;
                
                // Extract unique specializations from all doctors for filter dropdown
                AvailableSpecializations = AvailableDoctors
                    .Select(d => d.Specialization)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();
            }

            // Load all patients for selection
            var patientsResult = await _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(null, true);
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
                AllPatients = patientsResult.Data;

            _logger.LogInformation("Loaded {DoctorCount} doctors and {PatientCount} patients for appointment booking", 
                AvailableDoctors.Count, AllPatients.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dropdown data");
        }
    }
    
    private string? GetCurrentUserId()
    {
        return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
