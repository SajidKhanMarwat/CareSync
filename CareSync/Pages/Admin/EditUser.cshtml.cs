using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.Services;
using CareSync.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin;

[Authorize(Roles = "Admin")]
public class EditUserModel : PageModel
{
    private readonly UserManagementApiService _userManagementService;
    private readonly ILogger<EditUserModel> _logger;

    public EditUserModel(UserManagementApiService userManagementService, ILogger<EditUserModel> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; } = string.Empty;

    public UserDetail_DTO UserDetail { get; set; } = new();
    public List<string> Departments { get; set; } = new();
    public List<string> Roles { get; set; } = new();

    [BindProperty]
    public EditUserInput Input { get; set; } = new();

    public class EditUserInput
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public Gender_Enum? Gender { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        public List<string>? Roles { get; set; }

        // Doctor-specific fields
        public DoctorInfoInput DoctorInfo { get; set; } = new();

        // Patient-specific fields
        public PatientInfoInput PatientInfo { get; set; } = new();
    }

    public class DoctorInfoInput
    {
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Department { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? Schedule { get; set; }
    }

    public class PatientInfoInput
    {
        public string? BloodGroup { get; set; }
        public string? MaritalStatus { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user ID.";
            return RedirectToPage("/Admin/Users");
        }

        try
        {
            UserId = userId;

            // Load departments and roles
            var deptResult = await _userManagementService.GetDepartmentsAsync();
            if (deptResult.IsSuccess && deptResult.Data != null)
            {
                Departments = deptResult.Data;
            }

            var rolesResult = await _userManagementService.GetRolesAsync();
            if (rolesResult.IsSuccess && rolesResult.Data != null)
            {
                Roles = rolesResult.Data;
            }

            // Load user details
            var result = await _userManagementService.GetUserByIdAsync(userId);
            
            if (result.IsSuccess && result.Data != null)
            {
                UserDetail = result.Data;
                
                // Populate input model
                Input = new EditUserInput
                {
                    FirstName = UserDetail.FirstName ?? string.Empty,
                    LastName = UserDetail.LastName,
                    Email = UserDetail.Email ?? string.Empty,
                    UserName = UserDetail.UserName ?? string.Empty,
                    PhoneNumber = UserDetail.PhoneNumber,
                    DateOfBirth = UserDetail.DateOfBirth,
                    Gender = UserDetail.Gender,
                    Address = UserDetail.Address,
                    IsActive = UserDetail.IsActive,
                    EmailConfirmed = UserDetail.EmailConfirmed,
                    Roles = UserDetail.Roles
                };

                // Populate doctor info if applicable
                if (UserDetail.DoctorInfo != null)
                {
                    Input.DoctorInfo = new DoctorInfoInput
                    {
                        Specialization = UserDetail.DoctorInfo.Specialization,
                        LicenseNumber = UserDetail.DoctorInfo.LicenseNumber,
                        YearsOfExperience = UserDetail.DoctorInfo.YearsOfExperience,
                        Department = UserDetail.DoctorInfo.Department,
                        ConsultationFee = UserDetail.DoctorInfo.ConsultationFee,
                        Schedule = UserDetail.DoctorInfo.Schedule
                    };
                }

                // Populate patient info if applicable
                if (UserDetail.PatientInfo != null)
                {
                    Input.PatientInfo = new PatientInfoInput
                    {
                        BloodGroup = UserDetail.PatientInfo.BloodGroup,
                        MaritalStatus = UserDetail.PatientInfo.MaritalStatus,
                        EmergencyContact = UserDetail.PatientInfo.EmergencyContact,
                        EmergencyContactPhone = UserDetail.PatientInfo.EmergencyContactPhone
                    };
                }

                return Page();
            }
            else
            {
                TempData["Error"] = result.Error ?? "User not found.";
                return RedirectToPage("/Admin/Users");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit: {UserId}", userId);
            TempData["Error"] = "An error occurred while loading the user.";
            return RedirectToPage("/Admin/Users");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload data for display
            await LoadPageData();
            return Page();
        }

        try
        {
            var updateDto = new CreateUpdateUser_DTO
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                UserName = Input.UserName,
                PhoneNumber = Input.PhoneNumber,
                DateOfBirth = Input.DateOfBirth,
                Gender = Input.Gender,
                Address = Input.Address,
                IsActive = Input.IsActive,
                EmailConfirmed = Input.EmailConfirmed,
                Roles = Input.Roles,
                RoleType = UserDetail.RoleType // Keep existing role type
            };

            // Add doctor info if applicable
            if (UserDetail.RoleType == RoleType.Doctor)
            {
                updateDto.DoctorInfo = new DoctorInfo_DTO
                {
                    Specialization = Input.DoctorInfo.Specialization,
                    LicenseNumber = Input.DoctorInfo.LicenseNumber,
                    YearsOfExperience = Input.DoctorInfo.YearsOfExperience,
                    Department = Input.DoctorInfo.Department,
                    ConsultationFee = Input.DoctorInfo.ConsultationFee,
                    Schedule = Input.DoctorInfo.Schedule
                };
            }

            // Add patient info if applicable
            if (UserDetail.RoleType == RoleType.Patient)
            {
                updateDto.PatientInfo = new PatientInfo_DTO
                {
                    BloodGroup = Input.PatientInfo.BloodGroup,
                    MaritalStatus = Input.PatientInfo.MaritalStatus,
                    EmergencyContact = Input.PatientInfo.EmergencyContact,
                    EmergencyContactPhone = Input.PatientInfo.EmergencyContactPhone
                };
            }

            var result = await _userManagementService.UpdateUserAsync(UserId, updateDto);
            
            if (result.IsSuccess)
            {
                TempData["Success"] = "User updated successfully.";
                return RedirectToPage("/Admin/ViewUserProfile", new { userId = UserId });
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to update user.";
                await LoadPageData();
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", UserId);
            TempData["Error"] = "An error occurred while updating the user.";
            await LoadPageData();
            return Page();
        }
    }

    private async Task LoadPageData()
    {
        // Load departments and roles
        var deptResult = await _userManagementService.GetDepartmentsAsync();
        if (deptResult.IsSuccess && deptResult.Data != null)
        {
            Departments = deptResult.Data;
        }

        var rolesResult = await _userManagementService.GetRolesAsync();
        if (rolesResult.IsSuccess && rolesResult.Data != null)
        {
            Roles = rolesResult.Data;
        }

        // Reload user details
        var result = await _userManagementService.GetUserByIdAsync(UserId);
        if (result.IsSuccess && result.Data != null)
        {
            UserDetail = result.Data;
        }
    }
}
