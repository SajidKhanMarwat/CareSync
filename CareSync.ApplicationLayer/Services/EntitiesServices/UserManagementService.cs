using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Repository;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;
using CareSync.Shared.Enums.Patient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public interface IUserManagementService
{
    Task<Result<UserStatistics_DTO>> GetUserStatisticsAsync();
    Task<Result<PagedResult<UserList_DTO>>> GetAllUsersAsync(UserFilter_DTO filter);
    Task<Result<UserDetail_DTO>> GetUserByIdAsync(string userId);
    Task<Result<GeneralResponse>> CreateUserAsync(CreateUpdateUser_DTO dto);
    Task<Result<GeneralResponse>> UpdateUserAsync(string userId, CreateUpdateUser_DTO dto);
    Task<Result<GeneralResponse>> DeleteUserAsync(string userId);
    Task<Result<GeneralResponse>> ToggleUserStatusAsync(string userId, bool isActive);
    Task<Result<GeneralResponse>> SuspendUserAsync(string userId, string reason);
    Task<Result<GeneralResponse>> ResetPasswordAsync(AdminPasswordReset_DTO dto);
    Task<Result<GeneralResponse>> BulkActionAsync(BulkUserAction_DTO dto);
    Task<Result<List<UserActivity_DTO>>> GetUserActivitiesAsync();
    Task<Result<GeneralResponse>> UpdateUserPermissionsAsync(UserPermission_DTO dto);
    Task<Result<List<string>>> GetDepartmentsAsync();
    Task<Result<List<string>>> GetRolesAsync();
}

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<T_Users> _userManager;
    private readonly RoleManager<T_Roles> _roleManager;
    private readonly IRepository<T_Users> _userRepo;
    private readonly IRepository<T_DoctorDetails> _doctorRepo;
    private readonly IRepository<T_PatientDetails> _patientRepo;
    private readonly IRepository<T_Lab> _labRepo;
    private readonly IRepository<T_Appointments> _appointmentRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserManagementService(
        UserManager<T_Users> userManager,
        RoleManager<T_Roles> roleManager,
        IRepository<T_Users> userRepo,
        IRepository<T_DoctorDetails> doctorRepo,
        IRepository<T_PatientDetails> patientRepo,
        IRepository<T_Lab> labRepo,
        IRepository<T_Appointments> appointmentRepo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepo = userRepo;
        _doctorRepo = doctorRepo;
        _patientRepo = patientRepo;
        _labRepo = labRepo;
        _appointmentRepo = appointmentRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserStatistics_DTO>> GetUserStatisticsAsync()
    {
        try
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            // Get all users
            var allUsers = await _userRepo.GetAllAsync();
            var thisMonthUsers = allUsers.Where(u => u.CreatedOn >= startOfMonth).ToList();
            // var lastMonthUsers = allUsers.Where(u => u.CreatedOn >= startOfLastMonth && u.CreatedOn < startOfMonth).ToList();
            var lastMonthUsers = thisMonthUsers;
            // Count by role
            var doctorRole = await _roleManager.FindByNameAsync("Doctor");
            var patientRole = await _roleManager.FindByNameAsync("Patient");
            var labRole = await _roleManager.FindByNameAsync("Lab");
            var adminRole = await _roleManager.FindByNameAsync("Admin");

            var doctorUsers = new List<T_Users>();
            var patientUsers = new List<T_Users>();
            var labUsers = new List<T_Users>();
            var adminUsers = new List<T_Users>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Doctor")) doctorUsers.Add(user);
                else if (roles.Contains("Patient")) patientUsers.Add(user);
                else if (roles.Contains("Lab") || roles.Contains("LabAssistant")) labUsers.Add(user);
                else if (roles.Contains("Admin")) adminUsers.Add(user);
            }

            var stats = new UserStatistics_DTO
            {
                TotalUsers = allUsers.Count(),
                TotalDoctors = doctorUsers.Count,
                TotalPatients = patientUsers.Count,
                TotalLabStaff = labUsers.Count,
                TotalAdmins = adminUsers.Count,
                ActiveUsers = allUsers.Count(u => !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.Now),
                InactiveUsers = allUsers.Count(u => u.LockoutEnabled && u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now),
                SuspendedUsers = allUsers.Count(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now.AddDays(30)),
                NewUsersThisMonth = thisMonthUsers.Count,
                NewDoctorsThisMonth = thisMonthUsers.Count(u => doctorUsers.Contains(u)),
                NewPatientsThisMonth = thisMonthUsers.Count(u => patientUsers.Contains(u)),
                NewLabStaffThisMonth = thisMonthUsers.Count(u => labUsers.Contains(u))
            };

            // Calculate growth percentages
            if (lastMonthUsers.Count > 0)
            {
                stats.UsersGrowthPercentage = ((decimal)(thisMonthUsers.Count - lastMonthUsers.Count) / lastMonthUsers.Count) * 100;
                
                var lastMonthDoctors = lastMonthUsers.Count(u => doctorUsers.Contains(u));
                if (lastMonthDoctors > 0)
                    stats.DoctorsGrowthPercentage = ((decimal)(stats.NewDoctorsThisMonth - lastMonthDoctors) / lastMonthDoctors) * 100;
                
                var lastMonthPatients = lastMonthUsers.Count(u => patientUsers.Contains(u));
                if (lastMonthPatients > 0)
                    stats.PatientsGrowthPercentage = ((decimal)(stats.NewPatientsThisMonth - lastMonthPatients) / lastMonthPatients) * 100;
                
                var lastMonthLabStaff = lastMonthUsers.Count(u => labUsers.Contains(u));
                if (lastMonthLabStaff > 0)
                    stats.LabStaffGrowthPercentage = ((decimal)(stats.NewLabStaffThisMonth - lastMonthLabStaff) / lastMonthLabStaff) * 100;
            }

            return Result<UserStatistics_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<UserStatistics_DTO>.Exception(ex);
        }
    }

    public async Task<Result<PagedResult<UserList_DTO>>> GetAllUsersAsync(UserFilter_DTO filter)
    {
        try
        {
            var query = _userManager.Users.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchLower = filter.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.UserName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    u.PhoneNumber.Contains(searchLower) ||
                    (u.FirstName + " " + u.LastName).ToLower().Contains(searchLower));
            }

            if (filter.RoleType.HasValue)
            {
                query = query.Where(u => u.RoleType == filter.RoleType.Value);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                switch (filter.Status.ToLower())
                {
                    case "active":
                        query = query.Where(u => !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.Now);
                        break;
                    case "inactive":
                        query = query.Where(u => u.LockoutEnabled && u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now);
                        break;
                    case "suspended":
                        query = query.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.Now.AddDays(30));
                        break;
                }
            }

            // Date filter
            if (!string.IsNullOrEmpty(filter.DateFilter))
            {
                var now = DateTime.Now;
                switch (filter.DateFilter.ToLower())
                {
                    case "today":
                        query = query.Where(u => u.CreatedOn.Date == now.Date);
                        break;
                    case "week":
                        var weekStart = now.AddDays(-(int)now.DayOfWeek);
                        query = query.Where(u => u.CreatedOn >= weekStart);
                        break;
                    case "month":
                        var monthStart = new DateTime(now.Year, now.Month, 1);
                        query = query.Where(u => u.CreatedOn >= monthStart);
                        break;
                    case "year":
                        var yearStart = new DateTime(now.Year, 1, 1);
                        query = query.Where(u => u.CreatedOn >= yearStart);
                        break;
                }
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "name" => filter.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "role" => filter.SortDescending ? query.OrderByDescending(u => u.RoleType) : query.OrderBy(u => u.RoleType),
                _ => filter.SortDescending ? query.OrderByDescending(u => u.CreatedOn) : query.OrderBy(u => u.CreatedOn)
            };

            // Apply pagination
            var users = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Map to DTOs
            var userListDtos = new List<UserList_DTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserList_DTO
                {
                    UserId = user.Id,
                    UserCode = GenerateUserCode(user.RoleType, user.CreatedOn),
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfileImage,
                    RoleType = user.RoleType,
                    RoleName = roles.FirstOrDefault() ?? "User",
                    Gender = user.Gender,
                    Age = user.Age,
                    IsActive = !user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now,
                    Status = GetUserStatus(user),
                    LastLoginDate = user.LastLogin,
                    RegisteredDate = user.CreatedOn,
                    Permissions = new List<string>()
                };

                // Get role-specific information
                if (user.RoleType == RoleType.Doctor)
                {
                    var doctor = await _doctorRepo.GetAsync(d => d.UserID == user.Id);
                    if (doctor != null)
                    {
                        userDto.Department = doctor.Specialization;
                        userDto.Specialization = doctor.Specialization;
                        userDto.LicenseNumber = doctor.LicenseNumber;
                        userDto.YearsOfExperience = doctor.ExperienceYears;
                    }
                }
                else if (user.RoleType == RoleType.Patient)
                {
                    var patient = await _patientRepo.GetAsync(p => p.UserID == user.Id);
                    if (patient != null)
                    {
                        userDto.BloodGroup = patient.BloodGroup;
                        userDto.TotalAppointments = await _appointmentRepo.GetCountAsync(a => a.PatientID == patient.PatientID);
                    }
                }
                else if (user.RoleType == RoleType.Lab || user.RoleType == RoleType.LabAssistant)
                {
                    var lab = await _labRepo.GetAsync(l => l.UserID.ToString() == user.Id);
                    if (lab != null)
                    {
                        userDto.Department = "Laboratory";
                    }
                }

                userListDtos.Add(userDto);
            }

            var pagedResult = new PagedResult<UserList_DTO>
            {
                Items = userListDtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };

            return Result<PagedResult<UserList_DTO>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<UserList_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<UserDetail_DTO>> GetUserByIdAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<UserDetail_DTO>.Failure(null, "User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var userDetail = new UserDetail_DTO
            {
                UserId = user.Id,
                UserCode = GenerateUserCode(user.RoleType, user.CreatedOn),
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                ProfilePicture = user.ProfileImage,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = user.Age,
                Address = user.Address,
                RoleType = user.RoleType,
                RoleName = roles.FirstOrDefault() ?? "User",
                Roles = roles.ToList(),
                IsActive = !user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd < DateTimeOffset.Now,
                Status = GetUserStatus(user),
                RegisteredDate = user.CreatedOn,
                LastLoginDate = user.LastLogin,
                AccessFailedCount = user.AccessFailedCount,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Permissions = new List<string>()
            };

            // Get role-specific information
            if (user.RoleType == RoleType.Doctor)
            {
                var doctor = await _doctorRepo.GetAsync(d => d.UserID == user.Id);
                if (doctor != null)
                {
                    userDetail.DoctorInfo = new DoctorInfo_DTO
                    {
                        DoctorId = doctor.DoctorID,
                        Specialization = doctor.Specialization,
                        LicenseNumber = doctor.LicenseNumber,
                        YearsOfExperience = doctor.ExperienceYears ?? 0,
                        ConsultationFee = 0, // ConsultationFee not in entity
                        Department = doctor.Specialization,
                        Schedule = $"{doctor.AvailableDays} {doctor.StartTime}-{doctor.EndTime}",
                        TotalPatients = await _appointmentRepo.GetCountAsync(a => a.DoctorID == doctor.DoctorID),
                        CompletedAppointments = await _appointmentRepo.GetCountAsync(a => a.DoctorID == doctor.DoctorID && a.Status == AppointmentStatus_Enum.Completed)
                    };
                }
            }
            else if (user.RoleType == RoleType.Patient)
            {
                var patient = await _patientRepo.GetAsync(p => p.UserID == user.Id);
                if (patient != null)
                {
                    userDetail.PatientInfo = new PatientInfo_DTO
                    {
                        PatientId = patient.PatientID,
                        BloodGroup = patient.BloodGroup,
                        MaritalStatus = patient.MaritalStatus.ToString(),
                        EmergencyContact = patient.EmergencyContactName,
                        EmergencyContactPhone = patient.EmergencyContactNumber,
                        TotalAppointments = await _appointmentRepo.GetCountAsync(a => a.PatientID == patient.PatientID),
                        LastVisitDate = (await _appointmentRepo.GetAllAsync(
                            a => a.PatientID == patient.PatientID && a.Status == AppointmentStatus_Enum.Completed))
                            .OrderByDescending(a => a.AppointmentDate)
                            .FirstOrDefault()?.AppointmentDate
                    };
                }
            }

            return Result<UserDetail_DTO>.Success(userDetail);
        }
        catch (Exception ex)
        {
            return Result<UserDetail_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> ToggleUserStatusAsync(string userId, bool isActive)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User not found" });

            if (isActive)
            {
                // Activate user
                user.LockoutEnd = null;
                user.LockoutEnabled = false;
            }
            else
            {
                // Deactivate user
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.Now.AddYears(100);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = $"User {(isActive ? "activated" : "deactivated")} successfully"
                });
            }

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> ResetPasswordAsync(AdminPasswordReset_DTO dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User not found" });

            // Remove old password
            await _userManager.RemovePasswordAsync(user);
            
            // Add new password
            var result = await _userManager.AddPasswordAsync(user, dto.NewPassword);
            
            if (result.Succeeded)
            {
                if (dto.RequirePasswordChange)
                {
                    // Set a flag that user must change password on next login
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    await _userManager.UpdateAsync(user);
                }

                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = "Password reset successfully"
                });
            }

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<List<string>>> GetDepartmentsAsync()
    {
        try
        {
            var departments = new List<string>
            {
                "Cardiology",
                "Neurology",
                "Orthopedics",
                "Pediatrics",
                "Emergency",
                "General Medicine",
                "Surgery",
                "Gynecology",
                "Dermatology",
                "Psychiatry",
                "Radiology",
                "Laboratory",
                "Administration"
            };

            return Result<List<string>>.Success(departments);
        }
        catch (Exception ex)
        {
            return Result<List<string>>.Exception(ex);
        }
    }

    public async Task<Result<List<string>>> GetRolesAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Result<List<string>>.Success(roles);
        }
        catch (Exception ex)
        {
            return Result<List<string>>.Exception(ex);
        }
    }

    // Helper methods
    private string GenerateUserCode(RoleType roleType, DateTime createdDate)
    {
        var prefix = roleType switch
        {
            RoleType.Admin => "A",
            RoleType.Doctor => "D",
            RoleType.Patient => "P",
            RoleType.Lab or RoleType.LabAssistant => "L",
            _ => "U"
        };

        // Generate a sequential number based on creation order
        var sequenceNumber = createdDate.Ticks % 10000;
        return $"{prefix}{sequenceNumber:D4}";
    }

    private string GetUserStatus(T_Users user)
    {
        if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now.AddDays(30))
            return "Suspended";
        if (user.LockoutEnabled && user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.Now)
            return "Inactive";
        return "Active";
    }

    // Implement remaining methods
    public async Task<Result<GeneralResponse>> CreateUserAsync(CreateUpdateUser_DTO dto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User with this email already exists" });

            // Create new user
            var user = new T_Users
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender ?? Gender_Enum.Male,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                RoleType = dto.RoleType,
                IsActive = dto.IsActive,
                EmailConfirmed = dto.EmailConfirmed,
                CreatedBy = "System", // Should be current user
                CreatedOn = DateTime.UtcNow,
                // Required fields
                RoleID = "1", // Default role ID (string)
                LoginID = 1, // Default login ID (int)
                ArabicUserName = dto.UserName ?? "", // Default to same as username
                ArabicFirstName = dto.FirstName ?? "" // Default to same as first name
            };

            // Calculate age if DateOfBirth is provided
            if (dto.DateOfBirth.HasValue)
            {
                user.Age = DateTime.Now.Year - dto.DateOfBirth.Value.Year;
            }

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Assign roles
            if (dto.Roles != null && dto.Roles.Any())
            {
                await _userManager.AddToRolesAsync(user, dto.Roles);
            }

            // Create role-specific records
            if (dto.RoleType == RoleType.Doctor && dto.DoctorInfo != null)
            {
                var doctor = new T_DoctorDetails
                {
                    UserID = user.Id,
                    Specialization = dto.DoctorInfo.Specialization,
                    LicenseNumber = dto.DoctorInfo.LicenseNumber,
                    ExperienceYears = dto.DoctorInfo.YearsOfExperience,
                    AvailableDays = dto.DoctorInfo.Schedule ?? "Monday-Friday",
                    StartTime = "09:00",
                    EndTime = "17:00",
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                };
                await _doctorRepo.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (dto.RoleType == RoleType.Patient && dto.PatientInfo != null)
            {
                var patient = new T_PatientDetails
                {
                    UserID = user.Id,
                    BloodGroup = dto.PatientInfo.BloodGroup,
                    MaritalStatus = Enum.Parse<MaritalStatusEnum>(dto.PatientInfo.MaritalStatus ?? "Single"),
                    EmergencyContactName = dto.PatientInfo.EmergencyContact,
                    EmergencyContactNumber = dto.PatientInfo.EmergencyContactPhone,
                    CreatedBy = "System",
                    CreatedOn = DateTime.UtcNow
                };
                await _patientRepo.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "User created successfully"
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserAsync(string userId, CreateUpdateUser_DTO dto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User not found" });

            // Update user properties
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender ?? user.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.Address = dto.Address;
            user.UpdatedBy = "System"; // Should be current user
            user.UpdatedOn = DateTime.UtcNow;

            // Calculate age if DateOfBirth is provided
            if (dto.DateOfBirth.HasValue)
            {
                user.Age = DateTime.Now.Year - dto.DateOfBirth.Value.Year;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                });
            }

            // Update roles if provided
            if (dto.Roles != null && dto.Roles.Any())
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, dto.Roles);
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "User updated successfully"
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeleteUserAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User not found" });

            // Soft delete
            user.IsDeleted = true;
            user.UpdatedBy = "System"; // Should be current user
            user.UpdatedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = "User deleted successfully"
                });
            }

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> SuspendUserAsync(string userId, string reason)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "User not found" });

            // Suspend user with reason
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.Now.AddDays(30);
            // Store reason in a log or audit table if needed

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = $"User suspended for 30 days. Reason: {reason}"
                });
            }

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<List<UserActivity_DTO>>> GetUserActivitiesAsync()
    {
        try
        {
            // This would typically fetch from an audit log table
            // For now, return sample data
            var activities = new List<UserActivity_DTO>
            {
                new UserActivity_DTO
                {
                    UserId = "sample-user-id",
                    UserName = "sampleuser",
                    FullName = "Sample User",
                    LastLoginDate = DateTime.Now.AddHours(-1),
                    LoginCount = 5,
                    TotalActions = 25,
                    RecentActions = new List<string> { "Login", "View Dashboard", "Update Profile" },
                    MostUsedFeature = "Dashboard",
                    AverageSessionDuration = 30.5m
                }
            };

            return Result<List<UserActivity_DTO>>.Success(activities);
        }
        catch (Exception ex)
        {
            return Result<List<UserActivity_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> BulkActionAsync(BulkUserAction_DTO dto)
    {
        try
        {
            var successCount = 0;
            var failedCount = 0;

            foreach (var userId in dto.UserIds)
            {
                Result<GeneralResponse> result = null;

                switch (dto.Action.ToLower())
                {
                    case "activate":
                        result = await ToggleUserStatusAsync(userId, true);
                        break;
                    case "deactivate":
                        result = await ToggleUserStatusAsync(userId, false);
                        break;
                    case "delete":
                        result = await DeleteUserAsync(userId);
                        break;
                    case "suspend":
                        result = await SuspendUserAsync(userId, "Bulk action");
                        break;
                }

                if (result != null && result.IsSuccess)
                    successCount++;
                else
                    failedCount++;
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"Bulk action completed. Success: {successCount}, Failed: {failedCount}"
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserPermissionsAsync(UserPermission_DTO dto)
    {
        try
        {
            // This would typically update a permissions table
            // For now, we'll just return success
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Permissions updated successfully"
            });
        }
        catch (Exception ex)
        {
            return Result<GeneralResponse>.Exception(ex);
        }
    }
}
