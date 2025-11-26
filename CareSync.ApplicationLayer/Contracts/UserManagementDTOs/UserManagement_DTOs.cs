using CareSync.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareSync.ApplicationLayer.Contracts.UserManagementDTOs;

// User List DTO for grid display
public class UserList_DTO
{
    public string UserId { get; set; }
    public string UserCode { get; set; } // A001, D001, P001, etc.
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string ProfilePicture { get; set; }
    public RoleType RoleType { get; set; }
    public string RoleName { get; set; }
    public string Department { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } // Active, Inactive, Suspended
    public DateTime? LastLoginDate { get; set; }
    public DateTime RegisteredDate { get; set; }
    public Gender_Enum? Gender { get; set; }
    public int? Age { get; set; }
    
    // Professional Info (for doctors)
    public string Specialization { get; set; }
    public string LicenseNumber { get; set; }
    public int? YearsOfExperience { get; set; }
    
    // Patient specific
    public string BloodGroup { get; set; }
    public int? TotalAppointments { get; set; }
    
    // Permissions
    public List<string> Permissions { get; set; }
}

// User Statistics DTO
public class UserStatistics_DTO
{
    public int TotalUsers { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public int TotalLabStaff { get; set; }
    public int TotalAdmins { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int SuspendedUsers { get; set; }
    public decimal UsersGrowthPercentage { get; set; }
    public decimal DoctorsGrowthPercentage { get; set; }
    public decimal PatientsGrowthPercentage { get; set; }
    public decimal LabStaffGrowthPercentage { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewDoctorsThisMonth { get; set; }
    public int NewPatientsThisMonth { get; set; }
    public int NewLabStaffThisMonth { get; set; }
}

// User Detail DTO
public class UserDetail_DTO
{
    public string UserId { get; set; }
    public string UserCode { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string ProfilePicture { get; set; }
    public Gender_Enum? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    
    public RoleType RoleType { get; set; }
    public string RoleName { get; set; }
    public List<string> Roles { get; set; }
    public List<string> Permissions { get; set; }
    
    public bool IsActive { get; set; }
    public string Status { get; set; }
    public DateTime RegisteredDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public int AccessFailedCount { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool TwoFactorEnabled { get; set; }
    
    // Doctor specific
    public DoctorInfo_DTO DoctorInfo { get; set; }
    
    // Patient specific
    public PatientInfo_DTO PatientInfo { get; set; }
    
    // Lab specific
    public LabInfo_DTO LabInfo { get; set; }
}

public class DoctorInfo_DTO
{
    public int DoctorId { get; set; }
    public string Specialization { get; set; }
    public string LicenseNumber { get; set; }
    public int YearsOfExperience { get; set; }
    public string Qualifications { get; set; }
    public decimal ConsultationFee { get; set; }
    public string Department { get; set; }
    public string Schedule { get; set; }
    public int TotalPatients { get; set; }
    public int CompletedAppointments { get; set; }
    public decimal Rating { get; set; }
}

public class PatientInfo_DTO
{
    public int PatientId { get; set; }
    public string BloodGroup { get; set; }
    public string MaritalStatus { get; set; }
    public string EmergencyContact { get; set; }
    public string EmergencyContactPhone { get; set; }
    public string InsuranceProvider { get; set; }
    public string InsuranceNumber { get; set; }
    public int TotalAppointments { get; set; }
    public int TotalPrescriptions { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public List<string> ChronicDiseases { get; set; }
    public List<string> Allergies { get; set; }
}

public class LabInfo_DTO
{
    public int LabId { get; set; }
    public string LabName { get; set; }
    public string Department { get; set; }
    public string LicenseNumber { get; set; }
    public string Certification { get; set; }
    public int ProcessedTests { get; set; }
    public int PendingTests { get; set; }
}

// Create/Update User DTO
public class CreateUpdateUser_DTO
{
    public string UserId { get; set; } // For update
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // For create only
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public Gender_Enum? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public RoleType RoleType { get; set; }
    public List<string> Roles { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    
    // Role specific data
    public DoctorInfo_DTO DoctorInfo { get; set; }
    public PatientInfo_DTO PatientInfo { get; set; }
    public LabInfo_DTO LabInfo { get; set; }
}

// User Filter DTO
public class UserFilter_DTO
{
    public string SearchTerm { get; set; } = string.Empty;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoleType? RoleType { get; set; }
    
    public string Status { get; set; } = string.Empty; // Active, Inactive, Suspended
    public string Department { get; set; } = string.Empty;
    public DateTime? RegisteredFrom { get; set; }
    public DateTime? RegisteredTo { get; set; }
    public string DateFilter { get; set; } = string.Empty; // today, week, month, year
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "RegisteredDate";
    public bool SortDescending { get; set; } = true;
    public bool ExcludeDeleted { get; set; } = true; // Filter out soft-deleted users
}

// Bulk Action DTO
public class BulkUserAction_DTO
{
    public List<string> UserIds { get; set; }
    public string Action { get; set; } // Activate, Deactivate, Suspend, Delete, ResetPassword
    public string Reason { get; set; }
}

// User Activity DTO
public class UserActivity_DTO
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public int LoginCount { get; set; }
    public int TotalActions { get; set; }
    public List<string> RecentActions { get; set; }
    public string MostUsedFeature { get; set; }
    public decimal AverageSessionDuration { get; set; }
}

// Password Reset DTO
public class AdminPasswordReset_DTO
{
    public string UserId { get; set; }
    public string NewPassword { get; set; }
    public bool RequirePasswordChange { get; set; }
    public bool SendNotification { get; set; }
}

// User Permission DTO
public class UserPermission_DTO
{
    public string UserId { get; set; }
    public List<string> Permissions { get; set; }
    public List<string> Roles { get; set; }
    public Dictionary<string, bool> ModuleAccess { get; set; }
}
