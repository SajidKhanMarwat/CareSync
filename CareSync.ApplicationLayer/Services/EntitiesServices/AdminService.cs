using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;
using CareSync.Shared.Enums.Patient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareSync.ApplicationLayer.Services.EntitiesServices;

public class AdminService(
    UserManager<T_Users> userManager,
    RoleManager<T_Roles> roleManager,
    IUnitOfWork uow,
    ILogger<AdminService> logger) : IAdminService
{
    public async Task<Result<AdminUser_DTO>> GetUserAdminAsync(string userId)
    {
        logger.LogInformation("Executing: GetUserAdminAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<AdminUser_DTO>.Failure(
                    null!,
                    "User not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            var role = await roleManager.FindByIdAsync(user.RoleID ?? string.Empty);

            var adminUserDto = new AdminUser_DTO
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName,
                ArabicUserName = user.ArabicUserName,
                ArabicFirstName = user.ArabicFirstName,
                ArabicLastName = user.ArabicLastName,
                ProfileImage = user.ProfileImage,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = user.Age ?? 0,
                IsActive = user.IsActive,
                RoleType = user.RoleType,
                RoleName = role?.Name ?? user.RoleType.ToString(),
                Address = user.Address,
                LastLogin = user.LastLogin,
                CreatedOn = user.CreatedOn,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LoginID = user.LoginID,
                RoleID = user.RoleID,
                CreatedBy = user.CreatedBy,
                UpdatedOn = user.UpdatedOn,
                UpdatedBy = user.UpdatedBy
            };

            return Result<AdminUser_DTO>.Success(adminUserDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting admin user {UserId}", userId);
            return Result<AdminUser_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateUserAdminAsync(UserAdminUpdate_DTO request)
    {
        logger.LogInformation("Executing: UpdateUserAdminAsync for {UserId}", request.UserId);
        try
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not found"
                });

            // Update user fields
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.UpdatedOn = DateTime.UtcNow;
            user.UpdatedBy = request.UserId;

            var response = await userManager.UpdateAsync(user);
            if (response.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = "Profile updated successfully."
                });

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = response.Errors.FirstOrDefault()?.Description ?? "Update failed"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating admin profile");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeleteUserAdminAsync(string id)
    {
        logger.LogInformation("Executing: DeleteUserAdminAsync for {UserId}", id);
        try
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not found"
                });

            // Soft delete
            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = "User deleted successfully"
                });

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault()?.Description ?? "Delete failed"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    #region Dashboard & Analytics

    public async Task<Result<GetFirstRowCardsData_DTO>> GetDashboardStatsAsync()
    {
        // ========== ROLE ID RESOLUTION ==========
        var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
        var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());


        // ========== CURRENT MONTH RANGE ==========
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
        var lastDayLastMonth = firstDayThisMonth.AddDays(-1);


        // ========== TOTAL COUNTS ==========
        var totalAppointments = await uow.AppointmentsRepo.GetCountAsync(a => true);
        var totalDoctors = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id);
        var totalPatients = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id);


        // ========== THIS MONTH COUNTS ==========
        var thisMonthAppointments = await uow.AppointmentsRepo
            .GetCountAsync(a => a.CreatedOn >= firstDayThisMonth);

        var thisMonthDoctors = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == doctorRole!.Id
                             && u.CreatedOn >= firstDayThisMonth);

        var thisMonthPatients = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == patientRole!.Id
                             && u.CreatedOn >= firstDayThisMonth);


        // ========== LAST MONTH COUNTS ==========
        var lastMonthAppointments = await uow.AppointmentsRepo
            .GetCountAsync(a => a.CreatedOn >= firstDayLastMonth
                             && a.CreatedOn <= lastDayLastMonth);

        var lastMonthDoctors = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == doctorRole!.Id
                             && u.CreatedOn >= firstDayLastMonth
                             && u.CreatedOn <= lastDayLastMonth);

        var lastMonthPatients = await uow.UserRepo
            .GetCountAsync(u => u.RoleID == patientRole!.Id
                             && u.CreatedOn >= firstDayLastMonth
                             && u.CreatedOn <= lastDayLastMonth);


        // ========== PERCENTAGE CALCULATIONS ==========
        decimal AppointmentPercent = CalculatePercentage(thisMonthAppointments, lastMonthAppointments);
        decimal DoctorPercent = CalculatePercentage(thisMonthDoctors, lastMonthDoctors);
        decimal PatientPercent = CalculatePercentage(thisMonthPatients, lastMonthPatients);

        var getFirstRowCardsData = new GetFirstRowCardsData_DTO
        {
            TotalAppointments = totalAppointments,
            ThisVsLastMonthPercentageAppointment = AppointmentPercent,

            TotalDoctors = totalDoctors,
            ThisVsLastMonthPercentageDoctors = DoctorPercent,

            TotalPatients = totalPatients,
            ThisVsLastMonthPercentagePatients = PatientPercent
        };

        return Result<GetFirstRowCardsData_DTO>.Success(getFirstRowCardsData);
    }

    private decimal CalculatePercentage(int thisMonth, int lastMonth)
    {
        if (lastMonth == 0)
        {
            if (thisMonth == 0) return 0;
            return 100;
        }

        return Math.Round(((decimal)(thisMonth - lastMonth) / lastMonth) * 100, 2);
    }

    public async Task<Result<DashboardSummary_DTO>> GetDashboardSummaryAsync()
    {
        logger.LogInformation("Executing: GetDashboardSummaryAsync");
        try
        {
            var summary = new DashboardSummary_DTO();
            
            // Get all data in parallel for performance
            var statsTask = GetDashboardStatsAsync();
            var urgentTask = GetUrgentItemsAsync();
            var performanceTask = GetTodayPerformanceAsync();
            var appointmentsTask = GetTodaysAppointmentsAsync();

            await Task.WhenAll(statsTask, urgentTask, performanceTask, appointmentsTask);

            // Map stats to cards
            var stats = statsTask.Result.Data;
            if (stats != null)
            {
                summary.AppointmentsCard = new StatsCard_DTO
                {
                    TotalCount = stats.TotalAppointments,
                    PercentageChange = stats.ThisVsLastMonthPercentageAppointment
                };
                summary.DoctorsCard = new StatsCard_DTO
                {
                    TotalCount = stats.TotalDoctors,
                    PercentageChange = stats.ThisVsLastMonthPercentageDoctors
                };
                summary.PatientsCard = new StatsCard_DTO
                {
                    TotalCount = stats.TotalPatients,
                    PercentageChange = stats.ThisVsLastMonthPercentagePatients
                };
            }

            summary.UrgentItems = urgentTask.Result.Data ?? new List<UrgentItem_DTO>();
            summary.TodayPerformance = performanceTask.Result.Data ?? new TodayPerformance_DTO();
            summary.TodaysAppointments = appointmentsTask.Result.Data ?? new List<TodayAppointment_DTO>();

            return Result<DashboardSummary_DTO>.Success(summary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dashboard summary");
            return Result<DashboardSummary_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<UrgentItem_DTO>>> GetUrgentItemsAsync()
    {
        logger.LogInformation("Executing: GetUrgentItemsAsync");
        try
        {
            var urgentItems = new List<UrgentItem_DTO>();
            var today = DateTime.UtcNow.Date;

            // Get pending appointments
            var pendingAppointments = await uow.AppointmentsRepo.GetCountAsync(a =>
                a.Status == AppointmentStatus_Enum.Pending &&
                a.AppointmentDate.Date >= today);

            if (pendingAppointments > 0)
            {
                urgentItems.Add(new UrgentItem_DTO
                {
                    Type = "Appointment",
                    Message = $"{pendingAppointments} appointment(s) pending approval",
                    Priority = "High",
                    CreatedDate = DateTime.UtcNow,
                    Url = "/Admin/Appointments"
                });
            }

            return Result<List<UrgentItem_DTO>>.Success(urgentItems);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting urgent items");
            return Result<List<UrgentItem_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<TodayPerformance_DTO>> GetTodayPerformanceAsync()
    {
        logger.LogInformation("Executing: GetTodayPerformanceAsync");
        try
        {
            var today = DateTime.UtcNow.Date;

            var completedAppointments = await uow.AppointmentsRepo.GetCountAsync(a =>
                a.AppointmentDate.Date == today &&
                a.Status == AppointmentStatus_Enum.Completed);

            var pendingAppointments = await uow.AppointmentsRepo.GetCountAsync(a =>
                a.AppointmentDate.Date == today &&
                a.Status == AppointmentStatus_Enum.Pending);

            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var newPatients = await uow.UserRepo.GetCountAsync(u =>
                u.RoleID == patientRole!.Id &&
                u.CreatedOn.Date == today);

            var performance = new TodayPerformance_DTO
            {
                AppointmentsCompleted = completedAppointments,
                AppointmentsPending = pendingAppointments,
                NewPatientRegistrations = newPatients,
                LabReportsReady = 0,
                LabReportsPending = 0
            };

            return Result<TodayPerformance_DTO>.Success(performance);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting today performance");
            return Result<TodayPerformance_DTO>.Exception(ex);
        }
    }

    public async Task<Result<UserDistribution_DTO>> GetUserDistributionAsync()
    {
        logger.LogInformation("Executing: GetUserDistributionAsync");
        try
        {
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            var adminRole = await roleManager.FindByNameAsync(RoleType.Admin.ToString());
            var labRole = await roleManager.FindByNameAsync(RoleType.Lab.ToString());

            var distribution = new UserDistribution_DTO
            {
                Patients = await GetRoleDistribution(patientRole!.Id),
                Doctors = await GetRoleDistribution(doctorRole!.Id),
                AdminStaff = await GetRoleDistribution(adminRole!.Id),
                Labs = await GetRoleDistribution(labRole!.Id)
            };

            distribution.TotalUsers = distribution.Patients.TotalCount + 
                                     distribution.Doctors.TotalCount + 
                                     distribution.AdminStaff.TotalCount + 
                                     distribution.Labs.TotalCount;

            return Result<UserDistribution_DTO>.Success(distribution);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user distribution");
            return Result<UserDistribution_DTO>.Exception(ex);
        }
    }

    private async Task<RoleDistribution_DTO> GetRoleDistribution(string roleId)
    {
        var now = DateTime.UtcNow;
        var firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
        var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);
        var lastDayLastMonth = firstDayThisMonth.AddDays(-1);

        var total = await uow.UserRepo.GetCountAsync(u => u.RoleID == roleId);
        var active = await uow.UserRepo.GetCountAsync(u => u.RoleID == roleId && u.IsActive);
        var thisMonth = await uow.UserRepo.GetCountAsync(u => u.RoleID == roleId && u.CreatedOn >= firstDayThisMonth);
        var lastMonth = await uow.UserRepo.GetCountAsync(u => u.RoleID == roleId && u.CreatedOn >= firstDayLastMonth && u.CreatedOn <= lastDayLastMonth);

        return new RoleDistribution_DTO
        {
            TotalCount = total,
            ActiveCount = active,
            InactiveCount = total - active,
            ThisMonthCount = thisMonth,
            LastMonthCount = lastMonth,
            PercentageChange = CalculatePercentage(thisMonth, lastMonth)
        };
    }

    #endregion

    #region Lab Management

    public async Task<Result<List<LabListDTO>>> GetAllLabsAsync()
    {
        logger.LogInformation("Executing: GetAllLabsAsync");
        try
        {
            var labs = await uow.LabRepo.GetAllAsync(l => !l.IsDeleted);
            
            var labList = labs.Select(lab => new LabListDTO
            {
                LabId = lab.LabID,
                LabName = lab.LabName,
                ArabicLabName = lab.ArabicLabName,
                Location = lab.Location,
                ContactNumber = lab.ContactNumber,
                Email = lab.Email,
                IsActive = !lab.IsDeleted
            }).OrderBy(l => l.LabName).ToList();

            logger.LogInformation("Retrieved {Count} laboratories", labList.Count);
            return Result<List<LabListDTO>>.Success(labList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all labs");
            return Result<List<LabListDTO>>.Exception(ex);
        }
    }

    public async Task<Result<LabDetails_DTO>> GetLabByIdAsync(int labId)
    {
        logger.LogInformation("Executing: GetLabByIdAsync for LabId {LabId}", labId);
        try
        {
            var lab = await uow.LabRepo.GetByIdAsync(labId);
            if (lab == null || lab.IsDeleted)
            {
                return Result<LabDetails_DTO>.Failure(
                    null!,
                    "Laboratory not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            var services = await uow.LabServicesRepo.GetAllAsync(s => s.LabID == labId && !s.IsDeleted);
            var assistants = await uow.UserLabAssistantRepo.GetAllAsync(a => a.LabId == labId && !a.IsDeleted);

            var labDetails = new LabDetails_DTO
            {
                LabId = lab.LabID,
                UserId = lab.UserID,
                LabName = lab.LabName,
                ArabicLabName = lab.ArabicLabName,
                LabAddress = lab.LabAddress,
                ArabicLabAddress = lab.ArabicLabAddress,
                Location = lab.Location,
                ContactNumber = lab.ContactNumber,
                Email = lab.Email,
                LicenseNumber = lab.LicenseNumber,
                OpeningTime = lab.OpeningTime,
                ClosingTime = lab.ClosingTime,
                CreatedOn = lab.CreatedOn,
                CreatedBy = lab.CreatedBy,
                ServicesCount = services.Count(),
                AssistantsCount = assistants.Count(),
                IsActive = !lab.IsDeleted
            };

            return Result<LabDetails_DTO>.Success(labDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting lab details for LabId {LabId}", labId);
            return Result<LabDetails_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> CreateLabAsync(CreateLab_DTO dto, string createdBy)
    {
        logger.LogInformation("Executing: CreateLabAsync - {LabName}", dto.LabName);
        try
        {
            var lab = new T_Lab
            {
                LabName = dto.LabName,
                ArabicLabName = dto.ArabicLabName,
                LabAddress = dto.LabAddress,
                ArabicLabAddress = dto.ArabicLabAddress,
                Location = dto.Location,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                LicenseNumber = dto.LicenseNumber,
                OpeningTime = dto.OpeningTime,
                ClosingTime = dto.ClosingTime,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await uow.LabRepo.AddAsync(lab);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab created successfully: {LabName} with ID {LabId}", dto.LabName, lab.LabID);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"Laboratory '{dto.LabName}' created successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating lab: {LabName}", dto.LabName);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateLabAsync(UpdateLab_DTO dto, string updatedBy)
    {
        logger.LogInformation("Executing: UpdateLabAsync for LabId {LabId}", dto.LabId);
        try
        {
            var lab = await uow.LabRepo.GetByIdAsync(dto.LabId);
            if (lab == null || lab.IsDeleted)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Laboratory not found" },
                    "Laboratory not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            lab.LabName = dto.LabName;
            lab.ArabicLabName = dto.ArabicLabName;
            lab.LabAddress = dto.LabAddress;
            lab.ArabicLabAddress = dto.ArabicLabAddress;
            lab.Location = dto.Location;
            lab.ContactNumber = dto.ContactNumber;
            lab.Email = dto.Email;
            lab.LicenseNumber = dto.LicenseNumber;
            lab.OpeningTime = dto.OpeningTime;
            lab.ClosingTime = dto.ClosingTime;
            lab.UpdatedBy = updatedBy;
            lab.UpdatedOn = DateTime.UtcNow;

            await uow.LabRepo.UpdateAsync(lab);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab updated successfully: LabId {LabId}", dto.LabId);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Laboratory updated successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating lab: LabId {LabId}", dto.LabId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeleteLabAsync(int labId)
    {
        logger.LogInformation("Executing: DeleteLabAsync for LabId {LabId}", labId);
        try
        {
            var lab = await uow.LabRepo.GetByIdAsync(labId);
            if (lab == null || lab.IsDeleted)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Laboratory not found" },
                    "Laboratory not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            lab.IsDeleted = true;
            lab.UpdatedOn = DateTime.UtcNow;
            await uow.LabRepo.UpdateAsync(lab);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab deleted (soft) successfully: LabId {LabId}", labId);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Laboratory deleted successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting lab: LabId {LabId}", labId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<List<LabService_DTO>>> GetLabServicesAsync(int labId)
    {
        logger.LogInformation("Executing: GetLabServicesAsync for LabId {LabId}", labId);
        try
        {
            var services = await uow.LabServicesRepo.GetAllAsync(s => s.LabID == labId && !s.IsDeleted);
            var lab = await uow.LabRepo.GetByIdAsync(labId);

            var serviceList = services.Select(s => new LabService_DTO
            {
                LabServiceId = s.LabServiceID,
                LabId = s.LabID,
                ServiceName = s.ServiceName ?? string.Empty,
                Description = s.Description,
                Category = s.Category,
                SampleType = s.SampleType,
                Instructions = s.Instructions,
                Price = s.Price,
                EstimatedTime = s.EstimatedTime,
                LabName = lab?.LabName,
                IsActive = !s.IsDeleted
            }).ToList();

            logger.LogInformation("Retrieved {Count} services for LabId {LabId}", serviceList.Count, labId);
            return Result<List<LabService_DTO>>.Success(serviceList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting lab services for LabId {LabId}", labId);
            return Result<List<LabService_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<List<LabService_DTO>>> GetAllLabServicesAsync()
    {
        logger.LogInformation("Executing: GetAllLabServicesAsync");
        try
        {
            var services = await uow.LabServicesRepo.GetAllAsync(s => !s.IsDeleted);
            var labs = await uow.LabRepo.GetAllAsync(l => !l.IsDeleted);
            var labDict = labs.ToDictionary(l => l.LabID, l => l.LabName);

            var serviceList = services.Select(s => new LabService_DTO
            {
                LabServiceId = s.LabServiceID,
                LabId = s.LabID,
                ServiceName = s.ServiceName ?? string.Empty,
                Description = s.Description,
                Category = s.Category,
                SampleType = s.SampleType,
                Instructions = s.Instructions,
                Price = s.Price,
                EstimatedTime = s.EstimatedTime,
                LabName = labDict.ContainsKey(s.LabID) ? labDict[s.LabID] : null,
                IsActive = !s.IsDeleted
            }).ToList();

            logger.LogInformation("Retrieved {Count} total lab services", serviceList.Count);
            return Result<List<LabService_DTO>>.Success(serviceList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all lab services");
            return Result<List<LabService_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> CreateLabServiceAsync(LabService_DTO dto, string createdBy)
    {
        logger.LogInformation("Executing: CreateLabServiceAsync - {ServiceName}", dto.ServiceName);
        try
        {
            var lab = await uow.LabRepo.GetByIdAsync(dto.LabId);
            if (lab == null || lab.IsDeleted)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Laboratory not found" },
                    "Laboratory not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            var service = new T_LabServices
            {
                LabID = dto.LabId,
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                Category = dto.Category,
                SampleType = dto.SampleType,
                Instructions = dto.Instructions,
                Price = dto.Price,
                EstimatedTime = dto.EstimatedTime,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await uow.LabServicesRepo.AddAsync(service);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab service created successfully: {ServiceName}", dto.ServiceName);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = $"Service '{dto.ServiceName}' created successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating lab service: {ServiceName}", dto.ServiceName);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateLabServiceAsync(LabService_DTO dto, string updatedBy)
    {
        logger.LogInformation("Executing: UpdateLabServiceAsync for ServiceId {ServiceId}", dto.LabServiceId);
        try
        {
            var service = await uow.LabServicesRepo.GetByIdAsync(dto.LabServiceId);
            if (service == null || service.IsDeleted)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Service not found" },
                    "Service not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            service.ServiceName = dto.ServiceName;
            service.Description = dto.Description;
            service.Category = dto.Category;
            service.SampleType = dto.SampleType;
            service.Instructions = dto.Instructions;
            service.Price = dto.Price;
            service.EstimatedTime = dto.EstimatedTime;
            service.UpdatedBy = updatedBy;
            service.UpdatedOn = DateTime.UtcNow;

            await uow.LabServicesRepo.UpdateAsync(service);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab service updated successfully: ServiceId {ServiceId}", dto.LabServiceId);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Service updated successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating lab service: ServiceId {ServiceId}", dto.LabServiceId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeleteLabServiceAsync(int serviceId)
    {
        logger.LogInformation("Executing: DeleteLabServiceAsync for ServiceId {ServiceId}", serviceId);
        try
        {
            var service = await uow.LabServicesRepo.GetByIdAsync(serviceId);
            if (service == null || service.IsDeleted)
            {
                return Result<GeneralResponse>.Failure(
                    new GeneralResponse { Success = false, Message = "Service not found" },
                    "Service not found",
                    System.Net.HttpStatusCode.NotFound);
            }

            service.IsDeleted = true;
            service.UpdatedOn = DateTime.UtcNow;
            await uow.LabServicesRepo.UpdateAsync(service);
            await uow.SaveChangesAsync();

            logger.LogInformation("Lab service deleted (soft) successfully: ServiceId {ServiceId}", serviceId);
            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Service deleted successfully."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting lab service: ServiceId {ServiceId}", serviceId);
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<LabServicesPagedResult_DTO>> GetLabServicesPagedAsync(LabServicesFilter_DTO filter)
    {
        logger.LogInformation("Executing: GetLabServicesPagedAsync - LabId: {LabId}, Category: {Category}, Page: {Page}, PageSize: {PageSize}, Search: {Search}", 
            filter.LabId, filter.Category, filter.Page, filter.PageSize, filter.SearchTerm);
        try
        {
            // Validate pagination parameters
            if (filter.Page < 1)
            {
                logger.LogWarning("Invalid page number: {Page}. Setting to 1.", filter.Page);
                filter.Page = 1;
            }

            if (filter.PageSize < LabServicesFilter_DTO.MinPageSize || filter.PageSize > LabServicesFilter_DTO.MaxPageSize)
            {
                logger.LogWarning("Invalid page size: {PageSize}. Setting to default: {Default}.", filter.PageSize, LabServicesFilter_DTO.DefaultPageSize);
                filter.PageSize = LabServicesFilter_DTO.DefaultPageSize;
            }

            // Get all services with filters
            var query = await uow.LabServicesRepo.GetAllAsync(s => !s.IsDeleted);
            
            // Apply filters
            if (filter.LabId.HasValue)
            {
                query = query.Where(s => s.LabID == filter.LabId.Value).ToList();
            }
            
            if (!string.IsNullOrEmpty(filter.Category))
            {
                query = query.Where(s => s.Category == filter.Category).ToList();
            }
            
            if (filter.IsActive.HasValue)
            {
                query = query.Where(s => !s.IsDeleted == filter.IsActive.Value).ToList();
            }
            
            // Apply search term
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchLower = filter.SearchTerm.ToLower();
                query = query.Where(s => 
                    (s.ServiceName != null && s.ServiceName.ToLower().Contains(searchLower)) ||
                    (s.Description != null && s.Description.ToLower().Contains(searchLower)) ||
                    (s.Category != null && s.Category.ToLower().Contains(searchLower))
                ).ToList();
            }

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply sorting
            query = !string.IsNullOrEmpty(filter.SortBy) ? filter.SortBy.ToLower() switch
            {
                "servicename" => filter.SortDescending 
                    ? query.OrderByDescending(s => s.ServiceName).ToList()
                    : query.OrderBy(s => s.ServiceName).ToList(),
                "price" => filter.SortDescending 
                    ? query.OrderByDescending(s => s.Price ?? 0).ToList()
                    : query.OrderBy(s => s.Price ?? 0).ToList(),
                "category" => filter.SortDescending 
                    ? query.OrderByDescending(s => s.Category).ToList()
                    : query.OrderBy(s => s.Category).ToList(),
                _ => query.OrderBy(s => s.ServiceName).ToList()
            } : query.OrderBy(s => s.ServiceName).ToList();

            // Apply pagination
            var pagedServices = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            // Get labs for mapping
            var labs = await uow.LabRepo.GetAllAsync(l => !l.IsDeleted);
            var labDict = labs.ToDictionary(l => l.LabID, l => l.LabName);

            // Map to DTOs
            var serviceDtos = pagedServices.Select(s => new LabService_DTO
            {
                LabServiceId = s.LabServiceID,
                LabId = s.LabID,
                ServiceName = s.ServiceName ?? string.Empty,
                Description = s.Description,
                Category = s.Category,
                SampleType = s.SampleType,
                Instructions = s.Instructions,
                Price = s.Price,
                EstimatedTime = s.EstimatedTime,
                LabName = labDict.TryGetValue(s.LabID, out var labName) ? labName : "Unknown Lab",
                IsActive = !s.IsDeleted
            }).ToList();

            // Calculate statistics
            var allServices = await uow.LabServicesRepo.GetAllAsync(s => !s.IsDeleted);
            var activeServices = allServices.Count(s => !s.IsDeleted);
            var avgPrice = allServices.Where(s => s.Price.HasValue).Average(s => s.Price ?? 0);
            
            // Category distribution
            var categoryDist = allServices
                .Where(s => !string.IsNullOrEmpty(s.Category))
                .GroupBy(s => s.Category)
                .ToDictionary(g => g.Key!, g => g.Count());

            var result = new LabServicesPagedResult_DTO
            {
                Items = serviceDtos,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                TotalServices = allServices.Count(),
                ActiveServices = activeServices,
                TotalLaboratories = labs.Select(l => l.LabID).Distinct().Count(),
                AveragePrice = avgPrice,
                CategoryDistribution = categoryDist
            };

            logger.LogInformation("Retrieved {Count} lab services (Page {Page} of {TotalPages})", 
                serviceDtos.Count, result.Page, result.TotalPages);
            return Result<LabServicesPagedResult_DTO>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged lab services");
            return Result<LabServicesPagedResult_DTO>.Exception(ex);
        }
    }

    #endregion

    #region Appointment Management

    public async Task<Result<GeneralResponse>> CreateAppointmentAsync(AddAppointment_DTO appointment)
    {
        logger.LogInformation("Executing: CreateAppointmentAsync");
        try
        {
            var newAppointment = new T_Appointments
            {
                PatientID = appointment.PatientID,
                DoctorID = appointment.DoctorID,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentType = appointment.AppointmentType,
                Status = AppointmentStatus_Enum.Pending,
                Reason = appointment.Reason ?? string.Empty,
                Notes = appointment.Notes,
                CreatedBy = "Admin", // TODO: Get from current user context
                CreatedOn = DateTime.UtcNow
            };

            await uow.AppointmentsRepo.AddAsync(newAppointment);
            await uow.SaveChangesAsync();

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Appointment created successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating appointment");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> CreatePatientAccountAsync(CreatePatient_DTO input)
    {
        logger.LogInformation("Executing: CreatePatientAccountAsync");
        try
        {
            // Get Patient role
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            
            // Create user account with a new GUID
            var userId = Guid.NewGuid().ToString();
            var user = new T_Users
            {
                Id = userId,  // Explicitly set the Id
                UserName = !string.IsNullOrWhiteSpace(input.Username) ? input.Username : input.Email,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName ?? string.Empty,
                PhoneNumber = input.PhoneNumber,
                Gender = input.Gender,
                DateOfBirth = input.DateOfBirth,
                Address = input.Address,
                RoleType = RoleType.Patient,
                RoleID = patientRole!.Id,
                IsActive = true,
                EmailConfirmed = true,
                IsPasswordResetRequired = true,  // Require password reset on first login
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                ArabicUserName = input.FirstName,
                ArabicFirstName = input.FirstName,
                LoginID = 0
            };

            logger.LogInformation("Creating user with ID: {UserId}, Username: {Username}, Email: {Email}", 
                userId, user.UserName, user.Email);

            var result = await userManager.CreateAsync(user, input.Password ?? "CareSync@123");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("User creation failed: {Errors}", errors);
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Patient registration failed"
                });
            }

            logger.LogInformation("User created successfully with ID: {UserId}", userId);

            // Create patient details
            var patientDetails = new T_PatientDetails
            {
                UserID = userId,
                BloodGroup = input.BloodGroup,
                MaritalStatus = !string.IsNullOrEmpty(input.MaritalStatus) ? Enum.Parse<MaritalStatusEnum>(input.MaritalStatus) : MaritalStatusEnum.Single,
                EmergencyContactName = input.EmergencyContactName,
                EmergencyContactNumber = input.EmergencyContactPhone,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            logger.LogInformation("Adding patient details for user: {UserId}", userId);
            await uow.PatientDetailsRepo.AddAsync(patientDetails);
            await uow.SaveChangesAsync();
            logger.LogInformation("Patient details saved with PatientID: {PatientId}", patientDetails.PatientID);

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Patient account created successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating patient account: {Message}, InnerException: {Inner}", 
                ex.Message, ex.InnerException?.Message);
            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}{(ex.InnerException != null ? $" - {ex.InnerException.Message}" : "")}"
            });
        }
    }

    public async Task<Result<GeneralResponse>> CreateAppointmentWithQuickPatientAsync(AddAppointmentWithQuickPatient_DTO input)
    {
        logger.LogInformation("Executing: CreateAppointmentWithQuickPatientAsync");
        try
        {
            // First, register the patient if needed
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            
            // Create user account with a new GUID
            var userId = Guid.NewGuid().ToString();
            var user = new T_Users
            {
                Id = userId,  // Explicitly set the Id
                UserName = !string.IsNullOrWhiteSpace(input.Username) ? input.Username : input.Email,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName ?? string.Empty,
                PhoneNumber = input.PhoneNumber,
                Gender = input.Gender,
                DateOfBirth = input.DateOfBirth,
                Address = input.Address,
                RoleType = RoleType.Patient,
                RoleID = patientRole!.Id,
                IsActive = true,
                EmailConfirmed = true,
                IsPasswordResetRequired = true,  // Require password reset on first login
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                ArabicUserName = input.FirstName,
                ArabicFirstName = input.FirstName,
                LoginID = 0
            };

            logger.LogInformation("Creating user with ID: {UserId}, Username: {Username}, Email: {Email}", 
                userId, user.UserName, user.Email);

            var result = await userManager.CreateAsync(user, input.Password ?? "Patient@123");
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("User creation failed: {Errors}", errors);
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Patient registration failed"
                });
            }

            logger.LogInformation("User created successfully with ID: {UserId}", userId);

            // Create patient details
            var patientDetails = new T_PatientDetails
            {
                UserID = userId,  // Use the explicitly set userId
                BloodGroup = input.BloodGroup,
                MaritalStatus = !string.IsNullOrEmpty(input.MaritalStatus) ? Enum.Parse<MaritalStatusEnum>(input.MaritalStatus) : MaritalStatusEnum.Single,
                EmergencyContactName = input.EmergencyContactName,
                EmergencyContactNumber = input.EmergencyContactPhone,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            logger.LogInformation("Adding patient details for user: {UserId}", userId);
            await uow.PatientDetailsRepo.AddAsync(patientDetails);
            await uow.SaveChangesAsync();
            logger.LogInformation("Patient details saved with PatientID: {PatientId}", patientDetails.PatientID);

            // Now create the appointment
            var appointment = new T_Appointments
            {
                PatientID = patientDetails.PatientID,
                DoctorID = input.DoctorID,
                AppointmentDate = input.AppointmentDate,
                AppointmentType = input.AppointmentType,
                Status = AppointmentStatus_Enum.Pending,
                Reason = input.Reason ?? string.Empty,
                Notes = input.Notes,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            await uow.AppointmentsRepo.AddAsync(appointment);
            await uow.SaveChangesAsync();

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Patient registered and appointment created successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating appointment with quick patient: {Message}, InnerException: {Inner}", 
                ex.Message, ex.InnerException?.Message);
            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}{(ex.InnerException != null ? $" - {ex.InnerException.Message}" : "")}"
            });
        }
    }

    #endregion

    #region Registration & Trends

    public async Task<Result<RegistrationTrends_DTO>> GetRegistrationTrendsAsync()
    {
        logger.LogInformation("Executing: GetRegistrationTrendsAsync");
        try
        {
            var now = DateTime.UtcNow;
            var trends = new RegistrationTrends_DTO
            {
                Last6Months = new List<MonthlyData_DTO>()
            };

            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());

            // Get last 6 months data
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var firstDay = new DateTime(monthDate.Year, monthDate.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                var patients = await uow.UserRepo.GetCountAsync(u =>
                    u.RoleID == patientRole!.Id &&
                    u.CreatedOn >= firstDay &&
                    u.CreatedOn <= lastDay);

                var doctors = await uow.UserRepo.GetCountAsync(u =>
                    u.RoleID == doctorRole!.Id &&
                    u.CreatedOn >= firstDay &&
                    u.CreatedOn <= lastDay);

                var appointments = await uow.AppointmentsRepo.GetCountAsync(a =>
                    a.CreatedOn >= firstDay &&
                    a.CreatedOn <= lastDay);

                trends.Last6Months.Add(new MonthlyData_DTO
                {
                    MonthName = monthDate.ToString("MMM"),
                    Year = monthDate.Year,
                    Patients = patients,
                    Doctors = doctors,
                    Appointments = appointments
                });
            }

            // Calculate this month totals
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            trends.NewRegistrationsThisMonth = await uow.UserRepo.GetCountAsync(u =>
                u.RoleID == patientRole!.Id && u.CreatedOn >= thisMonthStart);
            trends.TotalAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.CreatedOn >= thisMonthStart);

            return Result<RegistrationTrends_DTO>.Success(trends);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting registration trends");
            return Result<RegistrationTrends_DTO>.Exception(ex);
        }
    }

    public async Task<Result<AppointmentStatusChart_DTO>> GetAppointmentStatusChartAsync()
    {
        logger.LogInformation("Executing: GetAppointmentStatusChartAsync");
        try
        {
            var chart = new AppointmentStatusChart_DTO
            {
                ConfirmedAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Confirmed),
                PendingAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Pending),
                CompletedAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Completed),
                CancelledAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Cancelled),
                RejectedAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Rejected)
            };

            chart.TotalAppointments = chart.ConfirmedAppointments + chart.PendingAppointments +
                                     chart.CompletedAppointments + chart.CancelledAppointments + chart.RejectedAppointments;

            return Result<AppointmentStatusChart_DTO>.Success(chart);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting appointment status chart");
            return Result<AppointmentStatusChart_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<TodayAppointment_DTO>>> GetTodaysAppointmentsAsync()
    {
        logger.LogInformation("Executing: GetTodaysAppointmentsAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => a.AppointmentDate.Date == today);

            var result = new List<TodayAppointment_DTO>();

            foreach (var appointment in appointments)
            {
                var doctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == appointment.DoctorID);
                var patient = await uow.PatientDetailsRepo.GetAsync(p => p.PatientID == appointment.PatientID);
                
                var doctorUser = doctor != null ? await userManager.FindByIdAsync(doctor.UserID) : null;
                var patientUser = patient != null && patient.UserID != null ? await userManager.FindByIdAsync(patient.UserID) : null;

                result.Add(new TodayAppointment_DTO
                {
                    AppointmentID = appointment.AppointmentID,
                    DoctorName = doctorUser != null ? $"Dr. {doctorUser.FirstName} {doctorUser.LastName}" : "Unknown",
                    PatientName = patientUser != null ? $"{patientUser.FirstName} {patientUser.LastName}" : "Unknown",
                    AppointmentTime = appointment.AppointmentDate,
                    AppointmentType = appointment.AppointmentType.ToString(),
                    Status = appointment.Status.ToString(),
                    Specialization = doctor?.Specialization ?? "General"
                });
            }

            return Result<List<TodayAppointment_DTO>>.Success(result.OrderBy(a => a.AppointmentTime).ToList());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting today's appointments");
            return Result<List<TodayAppointment_DTO>>.Exception(ex);
        }
    }

    #endregion

    #region Doctor Management

    public async Task<Result<List<DoctorList_DTO>>> GetAllDoctorsAsync(string? specialization = null, bool? isActive = null)
    {
        logger.LogInformation("Executing: GetAllDoctorsAsync");
        try
        {
            var doctors = await uow.DoctorDetailsRepo.GetAllAsync();
            var result = new List<DoctorList_DTO>();

            foreach (var doctor in doctors)
            {
                var user = await userManager.FindByIdAsync(doctor.UserID);
                if (user == null) continue;

                // Apply filters
                if (specialization != null && doctor.Specialization != specialization)
                    continue;
                if (isActive.HasValue && user.IsActive != isActive.Value)
                    continue;

                var appointmentCount = await uow.AppointmentsRepo.GetCountAsync(a => a.DoctorID == doctor.DoctorID);
                var todayCount = await uow.AppointmentsRepo.GetCountAsync(a =>
                    a.DoctorID == doctor.DoctorID &&
                    a.AppointmentDate.Date == DateTime.UtcNow.Date);

                result.Add(new DoctorList_DTO
                {
                    DoctorID = doctor.DoctorID,
                    UserID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Specialization = doctor.Specialization ?? "General",
                    ArabicSpecialization = doctor.ArabicSpecialization,
                    LicenseNumber = doctor.LicenseNumber ?? string.Empty,
                    ExperienceYears = doctor.ExperienceYears,
                    HospitalAffiliation = doctor.HospitalAffiliation,
                    AvailableDays = doctor.AvailableDays,
                    IsActive = user.IsActive,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-doctor.png",
                    CreatedOn = user.CreatedOn,
                    TotalAppointments = appointmentCount,
                    TodaysAppointments = todayCount
                });
            }

            return Result<List<DoctorList_DTO>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctors");
            return Result<List<DoctorList_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<DoctorStats_DTO>> GetDoctorStatsAsync()
    {
        logger.LogInformation("Executing: GetDoctorStatsAsync");
        try
        {
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var today = now.Date;

            var stats = new DoctorStats_DTO
            {
                TotalDoctors = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id),
                ActiveDoctors = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id && u.IsActive),
                InactiveDoctors = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id && !u.IsActive),
                NewDoctorsThisMonth = await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id && u.CreatedOn >= thisMonthStart),
                AppointmentsToday = await uow.AppointmentsRepo.GetCountAsync(a => a.AppointmentDate.Date == today)
            };

            // Get doctors by specialization
            var doctors = await uow.DoctorDetailsRepo.GetAllAsync();
            stats.DoctorsBySpecialization = doctors
                .GroupBy(d => d.Specialization ?? "General")
                .ToDictionary(g => g.Key, g => g.Count());

            // Calculate average experience
            var experienceDoctors = doctors.Where(d => d.ExperienceYears.HasValue).ToList();
            stats.AverageExperience = experienceDoctors.Any() 
                ? Math.Round((decimal)experienceDoctors.Average(d => d.ExperienceYears!.Value), 1) 
                : 0;

            return Result<DoctorStats_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor stats");
            return Result<DoctorStats_DTO>.Exception(ex);
        }
    }

    public async Task<Result<DoctorList_DTO>> GetDoctorByIdAsync(int doctorId)
    {
        logger.LogInformation("Executing: GetDoctorByIdAsync for {DoctorId}", doctorId);
        try
        {
            var doctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == doctorId);
            if (doctor == null)
                return Result<DoctorList_DTO>.Failure(null!, "Doctor not found", System.Net.HttpStatusCode.NotFound);

            var user = await userManager.FindByIdAsync(doctor.UserID);
            if (user == null)
                return Result<DoctorList_DTO>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);

            var appointmentCount = await uow.AppointmentsRepo.GetCountAsync(a => a.DoctorID == doctor.DoctorID);
            var todayCount = await uow.AppointmentsRepo.GetCountAsync(a =>
                a.DoctorID == doctor.DoctorID &&
                a.AppointmentDate.Date == DateTime.UtcNow.Date);

            var dto = new DoctorList_DTO
            {
                DoctorID = doctor.DoctorID,
                UserID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Specialization = doctor.Specialization ?? "General",
                ArabicSpecialization = doctor.ArabicSpecialization,
                LicenseNumber = doctor.LicenseNumber ?? string.Empty,
                ExperienceYears = doctor.ExperienceYears,
                HospitalAffiliation = doctor.HospitalAffiliation,
                AvailableDays = doctor.AvailableDays,
                IsActive = user.IsActive,
                ProfileImage = user.ProfileImage ?? "/theme/images/default-doctor.png",
                CreatedOn = user.CreatedOn,
                TotalAppointments = appointmentCount,
                TodaysAppointments = todayCount
            };

            return Result<DoctorList_DTO>.Success(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor by ID");
            return Result<DoctorList_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> ToggleDoctorStatusAsync(string userId, bool isActive)
    {
        logger.LogInformation("Executing: ToggleDoctorStatusAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Doctor not found"
                });

            user.IsActive = isActive;
            user.UpdatedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = $"Doctor {(isActive ? "activated" : "deactivated")} successfully"
                });

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault()?.Description ?? "Update failed"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error toggling doctor status");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<DoctorInsights_DTO>> GetDoctorInsightsAsync()
    {
        logger.LogInformation("Executing: GetDoctorInsightsAsync");
        try
        {
            var insights = new DoctorInsights_DTO();

            // Get statistics
            var statsResult = await GetDoctorStatisticsSummaryAsync();
            if (statsResult.IsSuccess && statsResult.Data != null)
                insights.Statistics = statsResult.Data;

            // Get top performers
            var performanceResult = await GetDoctorPerformanceAsync(6);
            if (performanceResult.IsSuccess && performanceResult.Data != null)
                insights.TopPerformers = performanceResult.Data;

            // Get specialization distribution
            var specializationResult = await GetSpecializationDistributionAsync();
            if (specializationResult.IsSuccess && specializationResult.Data != null)
                insights.Specializations = specializationResult.Data;

            // Get availability overview
            var availabilityResult = await GetDoctorAvailabilityOverviewAsync();
            if (availabilityResult.IsSuccess && availabilityResult.Data != null)
                insights.Availability = availabilityResult.Data;

            // Get recent activities
            insights.RecentActivities = await GetRecentDoctorActivitiesAsync();

            return Result<DoctorInsights_DTO>.Success(insights);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor insights");
            return Result<DoctorInsights_DTO>.Exception(ex);
        }
    }

    private async Task<Result<DoctorStatisticsSummary_DTO>> GetDoctorStatisticsSummaryAsync()
    {
        try
        {
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            if (doctorRole == null)
                return Result<DoctorStatisticsSummary_DTO>.Failure(null!, "Doctor role not found");

            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);
            var today = now.Date;

            var allDoctors = await uow.UserRepo.GetAllAsync(u => u.RoleID == doctorRole.Id);
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            var todayAppointments = await uow.AppointmentsRepo.GetAllAsync(a => a.AppointmentDate.Date == today);

            var stats = new DoctorStatisticsSummary_DTO
            {
                TotalDoctors = allDoctors.Count(),
                ActiveDoctors = allDoctors.Count(d => d.IsActive),
                InactiveDoctors = allDoctors.Count(d => !d.IsActive),
                OnLeave = 0, // TODO: Implement leave tracking
                NewThisMonth = allDoctors.Count(d => d.CreatedOn >= thisMonthStart),
                TotalAppointmentsToday = todayAppointments.Count(),
                TotalPatientsToday = todayAppointments.Select(a => a.PatientID).Distinct().Count()
            };

            // Calculate average experience
            var experiencedDoctors = doctorDetails.Where(d => d.ExperienceYears.HasValue).ToList();
            stats.AverageExperience = experiencedDoctors.Any() 
                ? (decimal)experiencedDoctors.Average(d => d.ExperienceYears!.Value) 
                : 0;

            // Calculate average rating (placeholder - implement rating system)
            stats.AverageRating = 4.5m;

            // Calculate growth percentage
            var lastMonthCount = allDoctors.Count(d => d.CreatedOn >= lastMonthStart && d.CreatedOn < thisMonthStart);
            stats.GrowthPercentage = lastMonthCount > 0 
                ? ((stats.NewThisMonth - lastMonthCount) / (decimal)lastMonthCount) * 100 
                : 0;

            return Result<DoctorStatisticsSummary_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor statistics summary");
            return Result<DoctorStatisticsSummary_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<DoctorPerformance_DTO>>> GetDoctorPerformanceAsync(int topCount = 6)
    {
        logger.LogInformation("Executing: GetDoctorPerformanceAsync");
        try
        {
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            if (doctorRole == null)
                return Result<List<DoctorPerformance_DTO>>.Success(new List<DoctorPerformance_DTO>());

            var doctors = await uow.UserRepo.GetAllAsync(u => u.RoleID == doctorRole.Id && u.IsActive);
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            var appointments = await uow.AppointmentsRepo.GetAllAsync();

            var performanceList = new List<DoctorPerformance_DTO>();

            foreach (var doctor in doctors.Take(topCount))
            {
                var detail = doctorDetails.FirstOrDefault(d => d.UserID == doctor.Id);
                if (detail == null) continue;

                var doctorAppointments = appointments.Where(a => a.DoctorID == detail.DoctorID).ToList();
                var completedCount = doctorAppointments.Count(a => a.Status == AppointmentStatus_Enum.Completed);
                var cancelledCount = doctorAppointments.Count(a => a.Status == AppointmentStatus_Enum.Cancelled);
                var totalCount = doctorAppointments.Count;

                performanceList.Add(new DoctorPerformance_DTO
                {
                    DoctorId = doctor.Id,
                    DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                    Specialization = detail.Specialization ?? "General",
                    ProfileImage = doctor.ProfileImage ?? "/theme/images/default-doctor.png",
                    TotalPatientsTreated = doctorAppointments.Select(a => a.PatientID).Distinct().Count(),
                    AppointmentsCompleted = completedCount,
                    AppointmentsCancelled = cancelledCount,
                    Rating = 4.5m + (decimal)(new Random().NextDouble() * 0.5), // Placeholder rating
                    ReviewCount = new Random().Next(10, 100),
                    CompletionRate = totalCount > 0 ? (completedCount * 100m / totalCount) : 0,
                    PatientSatisfaction = 85 + new Random().Next(0, 15), // Placeholder
                    ExperienceYears = detail.ExperienceYears ?? 0
                });
            }

            return Result<List<DoctorPerformance_DTO>>.Success(
                performanceList.OrderByDescending(p => p.TotalPatientsTreated).ToList()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor performance");
            return Result<List<DoctorPerformance_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<List<SpecializationDistribution_DTO>>> GetSpecializationDistributionAsync()
    {
        logger.LogInformation("Executing: GetSpecializationDistributionAsync");
        try
        {
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            var appointments = await uow.AppointmentsRepo.GetAllAsync();
            var today = DateTime.UtcNow.Date;

            // Get unique specializations from database
            var uniqueSpecializations = doctorDetails
                .Where(d => !string.IsNullOrEmpty(d.Specialization))
                .Select(d => d.Specialization!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Add "General" if not present for doctors without specialization
            if (doctorDetails.Any(d => string.IsNullOrEmpty(d.Specialization)) && !uniqueSpecializations.Contains("General"))
            {
                uniqueSpecializations.Add("General");
            }

            var distribution = new List<SpecializationDistribution_DTO>();
            var totalDoctors = doctorDetails.Count();

            foreach (var specialization in uniqueSpecializations)
            {
                var specDoctors = doctorDetails.Where(d => 
                    string.IsNullOrEmpty(d.Specialization) ? specialization == "General" :
                    d.Specialization!.Equals(specialization, StringComparison.OrdinalIgnoreCase)).ToList();
                
                var doctorIds = specDoctors.Select(d => d.DoctorID).ToList();
                var specAppointments = appointments.Where(a => doctorIds.Contains(a.DoctorID)).ToList();
                var todayAppointments = specAppointments.Where(a => a.AppointmentDate.Date == today).Count();

                distribution.Add(new SpecializationDistribution_DTO
                {
                    Specialization = specialization,
                    ArabicSpecialization = specialization, // TODO: Add Arabic translation when needed
                    DoctorCount = specDoctors.Count,
                    PatientCount = specAppointments.Select(a => a.PatientID).Distinct().Count(),
                    AppointmentsToday = todayAppointments,
                    Percentage = totalDoctors > 0 ? (specDoctors.Count * 100m / totalDoctors) : 0,
                    IconClass = GetSpecializationIcon(specialization),
                    ColorClass = GetSpecializationColor(specialization)
                });
            }

            return Result<List<SpecializationDistribution_DTO>>.Success(
                distribution.OrderByDescending(d => d.DoctorCount).ToList()
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting specialization distribution");
            return Result<List<SpecializationDistribution_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<List<string>>> GetAllSpecializationsAsync()
    {
        logger.LogInformation("Executing: GetAllSpecializationsAsync");
        try
        {
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            
            // Get unique specializations from database
            var specializations = doctorDetails
                .Where(d => !string.IsNullOrEmpty(d.Specialization))
                .Select(d => d.Specialization!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            // Add "General" if there are doctors without specialization
            if (doctorDetails.Any(d => string.IsNullOrEmpty(d.Specialization)) && !specializations.Contains("General"))
            {
                specializations.Insert(0, "General");
            }

            return Result<List<string>>.Success(specializations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all specializations");
            return Result<List<string>>.Exception(ex);
        }
    }

    public async Task<Result<DoctorAvailabilityOverview_DTO>> GetDoctorAvailabilityOverviewAsync()
    {
        logger.LogInformation("Executing: GetDoctorAvailabilityOverviewAsync");
        try
        {
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            if (doctorRole == null)
                return Result<DoctorAvailabilityOverview_DTO>.Success(new DoctorAvailabilityOverview_DTO());

            var doctors = await uow.UserRepo.GetAllAsync(u => u.RoleID == doctorRole.Id && u.IsActive);
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            var todayAppointments = await uow.AppointmentsRepo.GetAllAsync(a => a.AppointmentDate.Date == DateTime.UtcNow.Date);

            var overview = new DoctorAvailabilityOverview_DTO
            {
                AvailableNow = doctors.Count() / 3, // Placeholder logic
                InConsultation = doctors.Count() / 4, // Placeholder logic
                OnBreak = doctors.Count() / 6, // Placeholder logic
                OffDuty = doctors.Count() - (doctors.Count() / 3 + doctors.Count() / 4 + doctors.Count() / 6)
            };

            // Get today's schedules
            foreach (var doctor in doctors.Take(10)) // Limit to 10 for performance
            {
                var detail = doctorDetails.FirstOrDefault(d => d.UserID == doctor.Id);
                if (detail == null) continue;

                var doctorTodayAppointments = todayAppointments.Where(a => a.DoctorID == detail.DoctorID).ToList();

                overview.TodaySchedules.Add(new DoctorSchedule_DTO
                {
                    DoctorId = doctor.Id,
                    DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                    Specialization = detail.Specialization ?? "General",
                    StartTime = detail.StartTime ?? "09:00",
                    EndTime = detail.EndTime ?? "17:00",
                    IsAvailable = true, // Placeholder
                    AppointmentsBooked = doctorTodayAppointments.Count,
                    SlotsAvailable = 20 - doctorTodayAppointments.Count // Assuming 20 slots per day
                });
            }

            return Result<DoctorAvailabilityOverview_DTO>.Success(overview);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor availability overview");
            return Result<DoctorAvailabilityOverview_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<DoctorWorkload_DTO>>> GetDoctorWorkloadAsync()
    {
        logger.LogInformation("Executing: GetDoctorWorkloadAsync");
        try
        {
            // Placeholder implementation - would need actual workload tracking
            return Result<List<DoctorWorkload_DTO>>.Success(new List<DoctorWorkload_DTO>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor workload");
            return Result<List<DoctorWorkload_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<List<DoctorGridItem_DTO>>> GetDoctorGridDataAsync(string? specialization = null, bool? isActive = null, int page = 1, int pageSize = 10)
    {
        logger.LogInformation("Executing: GetDoctorGridDataAsync");
        try
        {
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            if (doctorRole == null)
                return Result<List<DoctorGridItem_DTO>>.Success(new List<DoctorGridItem_DTO>());

            var doctors = await uow.UserRepo.GetAllAsync(u => u.RoleID == doctorRole.Id);
            var doctorDetails = await uow.DoctorDetailsRepo.GetAllAsync();
            var appointments = await uow.AppointmentsRepo.GetAllAsync();

            var gridItems = new List<DoctorGridItem_DTO>();

            foreach (var doctor in doctors)
            {
                var detail = doctorDetails.FirstOrDefault(d => d.UserID == doctor.Id);
                if (detail == null) continue;

                // Apply filters
                if (specialization != null && !detail.Specialization?.Contains(specialization, StringComparison.OrdinalIgnoreCase) == true)
                    continue;
                if (isActive.HasValue && doctor.IsActive != isActive.Value)
                    continue;

                var doctorAppointments = appointments.Where(a => a.DoctorID == detail.DoctorID).ToList();
                var thisMonth = DateTime.UtcNow.Month;
                var monthlyAppointments = doctorAppointments.Where(a => a.AppointmentDate.Month == thisMonth).ToList();

                gridItems.Add(new DoctorGridItem_DTO
                {
                    DoctorID = detail.DoctorID,
                    UserID = doctor.Id,
                    FullName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                    Email = doctor.Email ?? string.Empty,
                    PhoneNumber = doctor.PhoneNumber ?? string.Empty,
                    Specialization = detail.Specialization ?? "General",
                    Department = $"{detail.Specialization ?? "General"} Department",
                    LicenseNumber = detail.LicenseNumber ?? "N/A",
                    Qualifications = detail.QualificationSummary ?? "MBBS",
                    ExperienceYears = detail.ExperienceYears ?? 0,
                    JoinedDate = doctor.CreatedOn,
                    HospitalAffiliation = detail.HospitalAffiliation ?? "CareSync Medical Center",
                    AvailableDays = detail.AvailableDays ?? "Mon-Fri",
                    WorkingHours = $"{detail.StartTime ?? "09:00"} - {detail.EndTime ?? "17:00"}",
                    TotalPatients = doctorAppointments.Select(a => a.PatientID).Distinct().Count(),
                    MonthlyPatients = monthlyAppointments.Select(a => a.PatientID).Distinct().Count(),
                    Rating = 4.5m + (decimal)(new Random().NextDouble() * 0.5),
                    ReviewCount = new Random().Next(10, 100),
                    IsActive = doctor.IsActive,
                    Status = doctor.IsActive ? "Active" : "Inactive",
                    ProfileImage = doctor.ProfileImage ?? "/theme/images/default-doctor.png"
                });
            }

            // Apply pagination
            var paginatedItems = gridItems
                .OrderBy(d => d.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Result<List<DoctorGridItem_DTO>>.Success(paginatedItems);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor grid data");
            return Result<List<DoctorGridItem_DTO>>.Exception(ex);
        }
    }

    private async Task<List<RecentDoctorActivity_DTO>> GetRecentDoctorActivitiesAsync()
    {
        try
        {
            var activities = new List<RecentDoctorActivity_DTO>();
            var recentDoctors = await uow.UserRepo.GetAllAsync(u => 
                u.RoleType == RoleType.Doctor && 
                u.CreatedOn >= DateTime.UtcNow.AddDays(-7));

            foreach (var doctor in recentDoctors.Take(5))
            {
                activities.Add(new RecentDoctorActivity_DTO
                {
                    ActivityType = "Joined",
                    DoctorName = $"Dr. {doctor.FirstName} {doctor.LastName}",
                    Description = "New doctor joined the team",
                    ActivityDate = doctor.CreatedOn,
                    IconClass = "ri-user-add-line",
                    ColorClass = "success"
                });
            }

            return activities.OrderByDescending(a => a.ActivityDate).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting recent doctor activities");
            return new List<RecentDoctorActivity_DTO>();
        }
    }

    private string GetSpecializationIcon(string specialization)
    {
        // Return specific icons for main specializations
        return specialization?.ToLowerInvariant() switch
        {
            "cardiology" => "ri-heart-pulse-line",
            "neurology" => "ri-brain-line",
            "orthopedics" => "ri-bone-line",
            "pediatrics" => "ri-baby-line",
            "emergency" => "ri-emergency-line",
            "general" => "ri-stethoscope-line",
            "dermatology" => "ri-hand-sanitizer-line",
            "psychiatry" => "ri-mental-health-line",
            "surgery" => "ri-surgical-mask-line",
            "pharmacy" => "ri-medicine-bottle-line",
            "gynecology" => "ri-women-line",
            "ophthalmology" => "ri-eye-line",
            "ent" => "ri-voice-recognition-line",
            "radiology" => "ri-scan-line",
            "oncology" => "ri-capsule-line",
            "pathology" => "ri-microscope-line",
            "laboratory" => "ri-test-tube-line",
            _ => "ri-hospital-line" // Default icon for unknown specializations
        };
    }

    private string GetSpecializationColor(string specialization)
    {
        // Return specific colors for main specializations
        return specialization?.ToLowerInvariant() switch
        {
            "cardiology" => "primary",     // Blue
            "neurology" => "success",       // Green
            "orthopedics" => "warning",     // Yellow/Orange
            "pediatrics" => "info",         // Cyan
            "emergency" => "danger",        // Red
            "general" => "secondary",       // Gray
            "dermatology" => "primary",
            "psychiatry" => "info",
            "surgery" => "danger",
            "pharmacy" => "success",
            "gynecology" => "warning",
            "ophthalmology" => "primary",
            "ent" => "secondary",
            "radiology" => "info",
            "oncology" => "danger",
            "pathology" => "warning",
            "laboratory" => "success",
            _ => "secondary" // Default color for unknown specializations
        };
    }

    #endregion

    #region Patient Management

    public async Task<Result<List<PatientList_DTO>>> GetAllPatientsAsync(string? bloodGroup = null, bool? isActive = null)
    {
        logger.LogInformation("Executing: GetAllPatientsAsync");
        try
        {
            var patients = await uow.PatientDetailsRepo.GetAllAsync();
            var result = new List<PatientList_DTO>();

            foreach (var patient in patients)
            {
                if (patient.UserID == null) continue;

                var user = await userManager.FindByIdAsync(patient.UserID);
                if (user == null) continue;

                // Apply filters
                if (bloodGroup != null && patient.BloodGroup != bloodGroup)
                    continue;
                if (isActive.HasValue && user.IsActive != isActive.Value)
                    continue;

                var appointmentCount = await uow.AppointmentsRepo.GetCountAsync(a => a.PatientID == patient.PatientID);
                var lastAppointment = await uow.AppointmentsRepo.GetAllAsync(a => a.PatientID == patient.PatientID);
                var lastVisit = lastAppointment.OrderByDescending(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate;

                result.Add(new PatientList_DTO
                {
                    PatientID = patient.PatientID,
                    UserID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Gender = user.Gender.ToString(),
                    DateOfBirth = user.DateOfBirth,
                    Age = user.Age ?? 0,
                    BloodGroup = patient.BloodGroup,
                    MaritalStatus = patient.MaritalStatus,
                    Occupation = patient.Occupation,
                    IsActive = user.IsActive,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                    CreatedOn = user.CreatedOn,
                    Address = user.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactNumber = patient.EmergencyContactNumber,
                    RelationshipToEmergency = patient.RelationshipToEmergency,
                    TotalAppointments = appointmentCount,
                    LastVisit = lastVisit
                });
            }

            return Result<List<PatientList_DTO>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patients");
            return Result<List<PatientList_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<PatientStats_DTO>> GetPatientStatsAsync()
    {
        logger.LogInformation("Executing: GetPatientStatsAsync");
        try
        {
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var today = now.Date;

            var stats = new PatientStats_DTO
            {
                TotalPatients = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id),
                ActivePatients = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id && u.IsActive),
                InactivePatients = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id && !u.IsActive),
                NewPatientsThisMonth = await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id && u.CreatedOn >= thisMonthStart),
                AppointmentsToday = await uow.AppointmentsRepo.GetCountAsync(a => a.AppointmentDate.Date == today)
            };

            // Get patients by blood group
            var patients = await uow.PatientDetailsRepo.GetAllAsync();
            stats.PatientsByBloodGroup = patients
                .Where(p => !string.IsNullOrEmpty(p.BloodGroup))
                .GroupBy(p => p.BloodGroup!)
                .ToDictionary(g => g.Key, g => g.Count());

            // Get patients by gender
            var users = await uow.UserRepo.GetAllAsync(u => u.RoleID == patientRole!.Id);
            stats.PatientsByGender = users
                .GroupBy(u => u.Gender.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // Calculate average age
            var agePatients = users.Where(u => u.Age.HasValue).ToList();
            stats.AverageAge = agePatients.Any()
                ? Math.Round((decimal)agePatients.Average(u => u.Age!.Value), 1)
                : 0;

            return Result<PatientStats_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient stats");
            return Result<PatientStats_DTO>.Exception(ex);
        }
    }

    public async Task<Result<PatientList_DTO>> GetPatientByIdAsync(int patientId)
    {
        logger.LogInformation("Executing: GetPatientByIdAsync for {PatientId}", patientId);
        try
        {
            var patient = await uow.PatientDetailsRepo.GetAsync(p => p.PatientID == patientId);
            if (patient == null)
                return Result<PatientList_DTO>.Failure(null!, "Patient not found", System.Net.HttpStatusCode.NotFound);

            if (patient.UserID == null)
                return Result<PatientList_DTO>.Failure(null!, "Patient user not found", System.Net.HttpStatusCode.NotFound);

            var user = await userManager.FindByIdAsync(patient.UserID);
            if (user == null)
                return Result<PatientList_DTO>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);

            var appointmentCount = await uow.AppointmentsRepo.GetCountAsync(a => a.PatientID == patient.PatientID);
            var lastAppointment = await uow.AppointmentsRepo.GetAllAsync(a => a.PatientID == patient.PatientID);
            var lastVisit = lastAppointment.OrderByDescending(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate;

            var dto = new PatientList_DTO
            {
                PatientID = patient.PatientID,
                UserID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                Age = user.Age ?? 0,
                BloodGroup = patient.BloodGroup,
                MaritalStatus = patient.MaritalStatus,
                Occupation = patient.Occupation,
                IsActive = user.IsActive,
                ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                CreatedOn = user.CreatedOn,
                Address = user.Address,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactNumber = patient.EmergencyContactNumber,
                RelationshipToEmergency = patient.RelationshipToEmergency,
                TotalAppointments = appointmentCount,
                LastVisit = lastVisit
            };

            return Result<PatientList_DTO>.Success(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient by ID");
            return Result<PatientList_DTO>.Exception(ex);
        }
    }

    public async Task<Result<PatientSearchResult_DTO>> SearchPatientsComprehensiveAsync(PatientSearchRequest_DTO request)
    {
        logger.LogInformation("Executing: SearchPatientsComprehensiveAsync with filters");
        try
        {
            // Get all patients first
            var allPatients = await uow.PatientDetailsRepo.GetAllAsync(p => !p.IsDeleted);
            
            // Load related data
            var patientIds = allPatients.Select(p => p.PatientID).ToList();
            var userIds = allPatients.Where(p => p.UserID != null).Select(p => p.UserID!).ToList();
            
            // Get users
            var users = new Dictionary<string, T_Users>();
            foreach (var userId in userIds)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                    users[userId] = user;
            }
            
            // Get appointments
            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => patientIds.Contains(a.PatientID));
            var appointmentsByPatient = appointments.GroupBy(a => a.PatientID).ToDictionary(g => g.Key, g => g.ToList());
            
            // Create enriched patient list
            var enrichedPatients = allPatients
                .Where(p => p.UserID != null && users.ContainsKey(p.UserID))
                .Select(p => new 
                {
                    Patient = p,
                    User = users[p.UserID!],
                    Appointments = appointmentsByPatient.ContainsKey(p.PatientID) ? appointmentsByPatient[p.PatientID] : new List<T_Appointments>()
                })
                .ToList();

            // Apply filters
            var filteredPatients = enrichedPatients.AsEnumerable();
            
            // Apply text search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                filteredPatients = filteredPatients.Where(ep => 
                    ep.User.FirstName.ToLower().Contains(term) ||
                    (ep.User.LastName != null && ep.User.LastName.ToLower().Contains(term)) ||
                    (ep.User.Email != null && ep.User.Email.ToLower().Contains(term)) ||
                    (ep.User.PhoneNumber != null && ep.User.PhoneNumber.Contains(term)) ||
                    ep.Patient.PatientID.ToString().Contains(term)
                );
            }

            // Apply demographic filters
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                filteredPatients = filteredPatients.Where(ep => ep.User.Gender.ToString() == request.Gender);
            }

            if (request.MinAge.HasValue)
            {
                filteredPatients = filteredPatients.Where(ep => ep.User.Age >= request.MinAge.Value);
            }

            if (request.MaxAge.HasValue)
            {
                filteredPatients = filteredPatients.Where(ep => ep.User.Age <= request.MaxAge.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.BloodGroup))
            {
                filteredPatients = filteredPatients.Where(ep => ep.Patient.BloodGroup == request.BloodGroup);
            }

            if (!string.IsNullOrWhiteSpace(request.MaritalStatus))
            {
                filteredPatients = filteredPatients.Where(ep => ep.Patient.MaritalStatus.ToString() == request.MaritalStatus);
            }

            // Apply location filter
            if (!string.IsNullOrWhiteSpace(request.City))
            {
                filteredPatients = filteredPatients.Where(ep => 
                    ep.User.Address != null && ep.User.Address.ToLower().Contains(request.City.ToLower()));
            }

            // Apply status filter
            if (request.IsActive.HasValue)
            {
                filteredPatients = filteredPatients.Where(ep => ep.User.IsActive == request.IsActive.Value);
            }

            // Apply last visit filter
            if (request.LastVisitFrom.HasValue || request.LastVisitTo.HasValue)
            {
                if (request.LastVisitFrom.HasValue)
                {
                    filteredPatients = filteredPatients.Where(ep => 
                    {
                        var lastVisit = ep.Appointments.OrderByDescending(a => a.AppointmentDate).FirstOrDefault();
                        return lastVisit != null && lastVisit.AppointmentDate >= request.LastVisitFrom.Value;
                    });
                }
                
                if (request.LastVisitTo.HasValue)
                {
                    filteredPatients = filteredPatients.Where(ep => 
                    {
                        var lastVisit = ep.Appointments.OrderByDescending(a => a.AppointmentDate).FirstOrDefault();
                        return lastVisit != null && lastVisit.AppointmentDate <= request.LastVisitTo.Value;
                    });
                }
            }

            // Get total count before pagination
            var totalCount = filteredPatients.Count();

            // Apply sorting
            filteredPatients = request.SortBy?.ToLower() switch
            {
                "age" => request.SortDescending 
                    ? filteredPatients.OrderByDescending(ep => ep.User.Age ?? 0) 
                    : filteredPatients.OrderBy(ep => ep.User.Age ?? 0),
                "lastvisit" => request.SortDescending 
                    ? filteredPatients.OrderByDescending(ep => ep.Appointments.Any() 
                        ? ep.Appointments.Max(a => a.AppointmentDate) : DateTime.MinValue) 
                    : filteredPatients.OrderBy(ep => ep.Appointments.Any() 
                        ? ep.Appointments.Max(a => a.AppointmentDate) : DateTime.MinValue),
                "createddate" => request.SortDescending 
                    ? filteredPatients.OrderByDescending(ep => ep.Patient.CreatedOn) 
                    : filteredPatients.OrderBy(ep => ep.Patient.CreatedOn),
                _ => request.SortDescending 
                    ? filteredPatients.OrderByDescending(ep => ep.User.FirstName)
                           .ThenByDescending(ep => ep.User.LastName ?? "") 
                    : filteredPatients.OrderBy(ep => ep.User.FirstName)
                           .ThenBy(ep => ep.User.LastName ?? "")
            };

            // Apply pagination
            var paginatedPatients = filteredPatients
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var patientCards = new List<PatientSearchCard_DTO>();
            foreach (var ep in paginatedPatients)
            {
                var patient = ep.Patient;
                var user = ep.User;
                var patientAppointments = ep.Appointments;

                // Get doctor assignments (most frequent doctor from appointments)
                var assignedDoctor = "";
                if (patientAppointments.Any())
                {
                    var mostFrequentDoctorId = patientAppointments
                        .Where(a => !a.IsDeleted)
                        .GroupBy(a => a.DoctorID)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key;

                    if (mostFrequentDoctorId.HasValue)
                    {
                        var doctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == mostFrequentDoctorId.Value);
                        if (doctor?.UserID != null)
                        {
                            var doctorUser = await userManager.FindByIdAsync(doctor.UserID);
                            if (doctorUser != null)
                                assignedDoctor = $"Dr. {doctorUser.FirstName} {doctorUser.LastName}";
                        }
                    }
                }

                var now = DateTime.UtcNow;
                var upcomingAppointments = patientAppointments?
                    .Where(a => !a.IsDeleted && a.AppointmentDate > now)
                    .Count() ?? 0;

                patientCards.Add(new PatientSearchCard_DTO
                {
                    PatientID = patient.PatientID,
                    UserID = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Gender = user.Gender.ToString(),
                    Age = user.Age,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                    BloodGroup = patient.BloodGroup,
                    Address = user.Address,
                    City = ExtractCity(user.Address),
                    IsActive = user.IsActive,
                    DateOfBirth = user.DateOfBirth,
                    CreatedOn = patient.CreatedOn,
                    LastVisit = patientAppointments?
                        .Where(a => !a.IsDeleted && a.Status == AppointmentStatus_Enum.Completed)
                        .OrderByDescending(a => a.AppointmentDate)
                        .FirstOrDefault()?.AppointmentDate,
                    TotalAppointments = patientAppointments?.Count(a => !a.IsDeleted) ?? 0,
                    UpcomingAppointments = upcomingAppointments,
                    AssignedDoctor = assignedDoctor,
                    EmergencyContact = patient.EmergencyContactName,
                    ChronicConditions = "" // TODO: Add when chronic diseases repo is available
                });
            }

            var result = new PatientSearchResult_DTO
            {
                Patients = patientCards,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return Result<PatientSearchResult_DTO>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching patients");
            return Result<PatientSearchResult_DTO>.Exception(ex);
        }
    }

    private string? ExtractCity(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return null;
        
        // Simple extraction - in real app, would use proper address parsing
        var parts = address.Split(',');
        if (parts.Length >= 2)
            return parts[^2].Trim(); // Second to last part often contains city
        
        return null;
    }

    public async Task<Result<List<PatientSearch_DTO>>> SearchPatientsAsync(string searchTerm)
    {
        logger.LogInformation("Executing: SearchPatientsAsync for {SearchTerm}", searchTerm);
        try
        {
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var searchLower = searchTerm.ToLower();

            // Search in users
            var users = await uow.UserRepo.GetAllAsync(u => 
                u.RoleID == patientRole!.Id &&
                (u.FirstName.ToLower().Contains(searchLower) ||
                 (u.LastName != null && u.LastName.ToLower().Contains(searchLower)) ||
                 (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                 (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))));

            var result = new List<PatientSearch_DTO>();

            foreach (var user in users)
            {
                var patient = await uow.PatientDetailsRepo.GetAsync(p => p.UserID == user.Id);
                if (patient == null) continue;

                var lastAppointment = await uow.AppointmentsRepo.GetAllAsync(a => a.PatientID == patient.PatientID);
                var lastVisit = lastAppointment.OrderByDescending(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate;

                result.Add(new PatientSearch_DTO
                {
                    PatientID = patient.PatientID,
                    UserID = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Age = user.Age ?? 0,
                    BloodGroup = patient.BloodGroup,
                    LastVisit = lastVisit
                });
            }

            return Result<List<PatientSearch_DTO>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching patients");
            return Result<List<PatientSearch_DTO>>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> TogglePatientStatusAsync(string userId, bool isActive)
    {
        logger.LogInformation("Executing: TogglePatientStatusAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Patient not found"
                });

            user.IsActive = isActive;
            user.UpdatedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Result<GeneralResponse>.Success(new GeneralResponse
                {
                    Success = true,
                    Message = $"Patient {(isActive ? "activated" : "deactivated")} successfully"
                });

            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault()?.Description ?? "Update failed"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error toggling patient status");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdatePatientAsync(UserPatientProfileUpdate_DTO updateDto)
    {
        logger.LogInformation("Executing: UpdatePatientAsync for {UserId}", updateDto.UserId);
        try
        {
            // Update user information
            var user = await userManager.FindByIdAsync(updateDto.UserId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Patient not found"
                });

            // Update user properties
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.Address = updateDto.Address;
            user.UpdatedOn = DateTime.UtcNow;
            
            if (!string.IsNullOrEmpty(updateDto.Gender))
            {
                if (Enum.TryParse<Shared.Enums.Gender_Enum>(updateDto.Gender, out var gender))
                    user.Gender = gender;
            }
            
            if (updateDto.DateOfBirth.HasValue)
            {
                user.DateOfBirth = updateDto.DateOfBirth.Value;
                user.Age = CalculateAge(updateDto.DateOfBirth.Value);
            }

            var userResult = await userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = userResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user information"
                });
            }

            // Update patient details
            var patientDetails = await uow.PatientDetailsRepo.GetAsync(p => p.UserID == updateDto.UserId);
            if (patientDetails != null)
            {
                patientDetails.BloodGroup = updateDto.BloodGroup;
                patientDetails.Occupation = updateDto.Occupation;
                patientDetails.EmergencyContactName = updateDto.EmergencyContactName;
                patientDetails.EmergencyContactNumber = updateDto.EmergencyContactNumber;
                patientDetails.RelationshipToEmergency = updateDto.RelationshipToEmergency;
                patientDetails.UpdatedOn = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(updateDto.MaritalStatus))
                {
                    if (Enum.TryParse<MaritalStatusEnum>(updateDto.MaritalStatus, out var maritalStatus))
                        patientDetails.MaritalStatus = maritalStatus;
                }

                await uow.PatientDetailsRepo.UpdateAsync(patientDetails);
                await uow.SaveChangesAsync();
            }

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Patient information updated successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating patient");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<PatientProfile_DTO>> GetPatientProfileAsync(int patientId)
    {
        logger.LogInformation("Executing: GetPatientProfileAsync for {PatientId}", patientId);
        try
        {
            var patient = await uow.PatientDetailsRepo.GetAsync(p => p.PatientID == patientId);
            if (patient == null)
                return Result<PatientProfile_DTO>.Failure(null!, "Patient not found", System.Net.HttpStatusCode.NotFound);

            if (patient.UserID == null)
                return Result<PatientProfile_DTO>.Failure(null!, "Patient user not found", System.Net.HttpStatusCode.NotFound);

            var user = await userManager.FindByIdAsync(patient.UserID);
            if (user == null)
                return Result<PatientProfile_DTO>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);

            // Get appointment statistics
            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => a.PatientID == patientId);
            var now = DateTime.UtcNow;
            var upcomingAppointments = appointments.Where(a => a.AppointmentDate > now && !a.IsDeleted).ToList();
            var completedAppointments = appointments.Where(a => a.Status == Shared.Enums.Appointment.AppointmentStatus_Enum.Completed).ToList();
            var missedAppointments = appointments.Where(a => a.Status == Shared.Enums.Appointment.AppointmentStatus_Enum.Cancelled || 
                                                              a.Status == Shared.Enums.Appointment.AppointmentStatus_Enum.NoShow).ToList();

            // Get recent appointments with doctor names
            var recentAppointments = new List<Appointment_DTO>();
            var recentAppts = appointments.OrderByDescending(a => a.AppointmentDate).Take(5).ToList();
            foreach (var appt in recentAppts)
            {
                var doctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == appt.DoctorID);
                var doctorUser = doctor?.UserID != null ? await userManager.FindByIdAsync(doctor.UserID) : null;
                
                recentAppointments.Add(new Appointment_DTO
                {
                    AppointmentID = appt.AppointmentID,
                    DoctorID = appt.DoctorID,
                    PatientID = appt.PatientID,
                    AppointmentDate = appt.AppointmentDate,
                    AppointmentType = appt.AppointmentType,
                    Status = appt.Status,
                    Reason = appt.Reason ?? string.Empty,
                    Notes = appt.Notes,
                    DoctorName = doctorUser != null ? $"Dr. {doctorUser.FirstName} {doctorUser.LastName}" : "Unknown",
                    CreatedOn = appt.CreatedOn
                });
            }

            // Get recent vitals
            var vitals = await uow.PatientVitalsRepo.GetAllAsync(v => v.PatientID == patientId);
            var recentVitals = vitals.OrderByDescending(v => v.CreatedOn).Take(5).Select(v => new PatientVital_DTO
            {
                VitalID = v.VitalID,
                RecordedDate = v.CreatedOn, // Using CreatedOn as RecordedDate doesn't exist
                Weight = v.Weight,
                Height = v.Height,
                BloodPressure = v.BloodPressure,
                HeartRate = v.PulseRate, // Using PulseRate as HeartRate
                Temperature = null, // Temperature not available in T_PatientVitals
                RespiratoryRate = null, // RespiratoryRate not available in T_PatientVitals
                BMI = null, // BMI not available in T_PatientVitals - would need to calculate from Height/Weight
                RecordedBy = v.CreatedBy
            }).ToList();

            // TODO: Get chronic diseases when ChronicDiseasesRepo is available
            // var chronicDiseases = await uow.ChronicDiseasesRepo.GetAllAsync(c => c.PatientID == patientId && !c.IsDeleted);
            var diseaseList = new List<string>(); // Empty for now

            // TODO: Get allergies from medical history when MedicalHistoryRepo is available
            // var medicalHistory = await uow.MedicalHistoryRepo.GetAllAsync(m => m.PatientID == patientId && !m.IsDeleted);
            var allergies = new List<string>(); // Empty for now

            // TODO: Get active prescriptions when PrescriptionsRepo is available
            var activePrescriptions = new List<Prescription_DTO>();
            
            // Commented out until repositories are available
            /*
            var prescriptions = await uow.PrescriptionsRepo.GetAllAsync(p => p.PatientID == patientId && !p.IsDeleted);
            foreach (var prescription in prescriptions.Where(p => p.EndDate == null || p.EndDate > now))
            {
                var prescribingDoctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == prescription.DoctorID);
                var prescribingUser = prescribingDoctor?.UserID != null ? await userManager.FindByIdAsync(prescribingDoctor.UserID) : null;
                
                // Get prescription items
                var items = await uow.PrescriptionItemsRepo.GetAllAsync(pi => pi.PrescriptionID == prescription.PrescriptionID);
                foreach (var item in items)
                {
                    activePrescriptions.Add(new Prescription_DTO
                    {
                        PrescriptionID = prescription.PrescriptionID,
                        MedicationName = item.MedicationName ?? "Unknown",
                        Dosage = item.Dosage ?? "N/A",
                        Frequency = item.Frequency ?? "N/A",
                        StartDate = prescription.PrescriptionDate,
                        EndDate = prescription.EndDate,
                        PrescribedBy = prescribingUser != null ? $"Dr. {prescribingUser.FirstName} {prescribingUser.LastName}" : "Unknown",
                        Instructions = item.Instructions,
                        IsActive = prescription.EndDate == null || prescription.EndDate > now
                    });
                }
            }
            */

            // Get preferred doctor (most appointments with)
            var doctorAppointments = appointments.GroupBy(a => a.DoctorID)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();
            
            string? preferredDoctor = null;
            if (doctorAppointments != null)
            {
                var prefDoctor = await uow.DoctorDetailsRepo.GetAsync(d => d.DoctorID == doctorAppointments.Key);
                if (prefDoctor?.UserID != null)
                {
                    var prefDoctorUser = await userManager.FindByIdAsync(prefDoctor.UserID);
                    if (prefDoctorUser != null)
                        preferredDoctor = $"Dr. {prefDoctorUser.FirstName} {prefDoctorUser.LastName}";
                }
            }

            var profile = new PatientProfile_DTO
            {
                // Basic Information
                PatientID = patient.PatientID,
                UserID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Gender = user.Gender.ToString(),
                DateOfBirth = user.DateOfBirth,
                Age = user.Age,
                ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn,
                
                // Address Information
                Address = user.Address,
                
                // Medical Information
                BloodGroup = patient.BloodGroup,
                MaritalStatus = patient.MaritalStatus,
                Occupation = patient.Occupation,
                
                // Emergency Contact
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactNumber = patient.EmergencyContactNumber,
                RelationshipToEmergency = patient.RelationshipToEmergency,
                
                // Medical History Summary
                Allergies = allergies,
                ChronicDiseases = diseaseList,
                CurrentMedications = activePrescriptions.Select(p => p.MedicationName).Distinct().ToList(),
                
                // Statistics
                TotalAppointments = appointments.Count,
                CompletedAppointments = completedAppointments.Count,
                UpcomingAppointments = upcomingAppointments.Count,
                MissedAppointments = missedAppointments.Count,
                LastVisit = appointments.OrderByDescending(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate,
                NextAppointment = upcomingAppointments.OrderBy(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate,
                PreferredDoctor = preferredDoctor,
                
                // Recent Activity
                RecentAppointments = recentAppointments,
                RecentVitals = recentVitals,
                ActivePrescriptions = activePrescriptions
            };

            return Result<PatientProfile_DTO>.Success(profile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient profile");
            return Result<PatientProfile_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> DeletePatientAsync(string userId, int patientId)
    {
        logger.LogInformation("Executing: DeletePatientAsync for UserId: {UserId}, PatientId: {PatientId}", userId, patientId);
        try
        {
            // Get the user
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Patient not found"
                });

            // Soft delete the user
            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedOn = DateTime.UtcNow;

            var userResult = await userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = userResult.Errors.FirstOrDefault()?.Description ?? "Failed to delete user"
                });
            }

            // Soft delete patient details
            var patientDetails = await uow.PatientDetailsRepo.GetAsync(p => p.PatientID == patientId);
            if (patientDetails != null)
            {
                patientDetails.IsDeleted = true;
                patientDetails.UpdatedOn = DateTime.UtcNow;
                await uow.PatientDetailsRepo.UpdateAsync(patientDetails);
            }

            // Soft delete all appointments
            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => a.PatientID == patientId);
            foreach (var appointment in appointments)
            {
                appointment.IsDeleted = true;
                appointment.UpdatedOn = DateTime.UtcNow;
                await uow.AppointmentsRepo.UpdateAsync(appointment);
            }

            await uow.SaveChangesAsync();

            return Result<GeneralResponse>.Success(new GeneralResponse
            {
                Success = true,
                Message = "Patient deleted successfully"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting patient");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>> GetPatientRegistrationTrendsAsync()
    {
        // This method delegates to GetPatientRegTrendsAsync for backward compatibility
        return await GetPatientRegTrendsAsync();
    }

    public async Task<Result<PatientAgeDistribution_DTO>> GetPatientAgeDistributionAsync()
    {
        logger.LogInformation("Executing: GetPatientAgeDistributionAsync");
        try
        {
            var distribution = new PatientAgeDistribution_DTO();

            // Get patient role
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            if (patientRole == null)
            {
                logger.LogWarning("Patient role not found");
                return Result<PatientAgeDistribution_DTO>.Success(distribution);
            }

            var patients = await uow.UserRepo.GetAllAsync(u => 
                u.RoleID == patientRole.Id && !u.IsDeleted);

            foreach (var patient in patients)
            {
                var age = patient.Age ?? CalculateAge(patient.DateOfBirth);
                if (age.HasValue)
                {
                    if (age < 18) distribution.Age0To18++;
                    else if (age < 35) distribution.Age19To35++;
                    else if (age < 50) distribution.Age36To50++;
                    else if (age < 65) distribution.Age51To65++;
                    else distribution.Age65Plus++;
                }
            }

            return Result<PatientAgeDistribution_DTO>.Success(distribution);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient age distribution");
            return Result<PatientAgeDistribution_DTO>.Exception(ex);
        }
    }

    public async Task<Result<PatientDemographics_DTO>> GetPatientDemographicsAsync()
    {
        logger.LogInformation("Executing: GetPatientDemographicsAsync");
        try
        {
            var demographics = new PatientDemographics_DTO();

            // Get patient role
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            if (patientRole == null)
            {
                logger.LogWarning("Patient role not found");
                return Result<PatientDemographics_DTO>.Success(demographics);
            }

            // Get gender distribution
            var patients = await uow.UserRepo.GetAllAsync(u => 
                u.RoleID == patientRole.Id && !u.IsDeleted);

            demographics.FemaleCount = patients.Count(p => p.Gender == Shared.Enums.Gender_Enum.Female);
            demographics.MaleCount = patients.Count(p => p.Gender == Shared.Enums.Gender_Enum.Male);

            // Get marital status distribution from patient details
            var patientDetails = await uow.PatientDetailsRepo.GetAllAsync(p => !p.IsDeleted);
            
            demographics.MarriedCount = patientDetails.Count(p => p.MaritalStatus == MaritalStatusEnum.Married);
            demographics.SingleCount = patientDetails.Count(p => p.MaritalStatus == MaritalStatusEnum.Single);
            demographics.DivorcedCount = patientDetails.Count(p => p.MaritalStatus == MaritalStatusEnum.Divorced);
            demographics.WidowedCount = patientDetails.Count(p => p.MaritalStatus == MaritalStatusEnum.Widowed);

            return Result<PatientDemographics_DTO>.Success(demographics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient demographics");
            return Result<PatientDemographics_DTO>.Exception(ex);
        }
    }

    private int? CalculateAge(DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
            return null;

        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Value.Year;
        if (dateOfBirth.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }

    #endregion

    #region Doctor Availability

    public async Task<Result<DoctorAvailabilitySummary_DTO>> GetDoctorAvailabilityAsync()
    {
        logger.LogInformation("Executing: GetDoctorAvailabilityAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var currentDayName = DateTime.UtcNow.ToString("dddd"); // e.g., "Monday"
            var currentTime = DateTime.UtcNow.TimeOfDay;

            // Get all active doctors with their appointments
            var doctors = await uow.DoctorDetailsRepo.GetAllAsync();
            var availabilityList = new List<DoctorAvailability_DTO>();

            foreach (var doctor in doctors)
            {
                var user = await userManager.FindByIdAsync(doctor.UserID);
                if (user == null || !user.IsActive) continue;

                // Get today's appointments for this doctor
                var todaysAppointments = await uow.AppointmentsRepo.GetAllAsync(a =>
                    a.DoctorID == doctor.DoctorID &&
                    a.AppointmentDate.Date == today);

                var completedToday = todaysAppointments.Count(a =>
                    a.Status == AppointmentStatus_Enum.Completed &&
                    a.AppointmentDate < DateTime.UtcNow);

                // Determine doctor status
                string status = DetermineDoctorStatus(doctor, currentDayName, currentTime, todaysAppointments.Count);

                availabilityList.Add(new DoctorAvailability_DTO
                {
                    DoctorID = doctor.DoctorID,
                    DoctorName = $"Dr. {user.FirstName} {user.LastName}".Trim(),
                    Specialization = doctor.Specialization ?? "General",
                    Status = status,
                    AvailableDays = doctor.AvailableDays ?? "Not set",
                    StartTime = doctor.StartTime,
                    EndTime = doctor.EndTime,
                    TodaysAppointmentCount = todaysAppointments.Count,
                    CompletedAppointmentsToday = completedToday,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-doctor.png",
                    IsActive = user.IsActive
                });
            }

            // Calculate summary
            var summary = new DoctorAvailabilitySummary_DTO
            {
                TotalAvailable = availabilityList.Count(d => d.Status == "Available"),
                InSession = availabilityList.Count(d => d.Status == "InSession"),
                OnBreak = availabilityList.Count(d => d.Status == "OnBreak"),
                OffToday = availabilityList.Count(d => d.Status == "Off"),
                Doctors = availabilityList.OrderBy(d => d.DoctorName).ToList()
            };

            logger.LogInformation("Retrieved availability for {Count} doctors", availabilityList.Count);
            return Result<DoctorAvailabilitySummary_DTO>.Success(summary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor availability");
            return Result<DoctorAvailabilitySummary_DTO>.Exception(ex);
        }
    }

    private string DetermineDoctorStatus(T_DoctorDetails doctor, string currentDayName, TimeSpan currentTime, int appointmentCount)
    {
        // Check if doctor is available today
        if (string.IsNullOrEmpty(doctor.AvailableDays) || 
            !doctor.AvailableDays.Contains(currentDayName, StringComparison.OrdinalIgnoreCase))
        {
            return "Off";
        }

        // Check if within working hours
        if (!string.IsNullOrEmpty(doctor.StartTime) && !string.IsNullOrEmpty(doctor.EndTime))
        {
            if (TimeSpan.TryParse(doctor.StartTime, out var startTime) && 
                TimeSpan.TryParse(doctor.EndTime, out var endTime))
            {
                if (currentTime < startTime || currentTime > endTime)
                {
                    return "Off";
                }

                // Check if currently in session (has appointments right now)
                if (appointmentCount > 0)
                {
                    return "InSession";
                }
            }
        }

        return "Available";
    }

    #endregion

    #region Today's Performance

    public async Task<Result<TodayPerformanceMetrics_DTO>> GetTodayPerformanceMetricsAsync()
    {
        logger.LogInformation("Executing: GetTodayPerformanceMetricsAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var todayEnd = today.AddDays(1).AddTicks(-1);

            // Get today's appointments
            var todaysAppointments = await uow.AppointmentsRepo.GetAllAsync(a =>
                a.AppointmentDate.Date == today);

            // Patient Check-ins
            var totalScheduledToday = todaysAppointments.Count;
            var checkedInToday = todaysAppointments.Count(a =>
                a.Status == AppointmentStatus_Enum.Confirmed);
            var checkInPercentage = totalScheduledToday > 0
                ? (decimal)checkedInToday / totalScheduledToday * 100
                : 0;

            // Appointments Completed
            var completedAppointments = todaysAppointments.Count(a =>
                a.Status == AppointmentStatus_Enum.Completed);
            var completionPercentage = totalScheduledToday > 0
                ? (decimal)completedAppointments / totalScheduledToday * 100
                : 0;

            // Lab Reports (if lab entities exist)
            var labReportsReady = 0;
            var totalLabReportsRequested = 0;
            var labReportsPercentage = 0m;

            try
            {
                var todaysLabRequests = await uow.LabRepo.GetAllAsync(l =>
                    l.CreatedOn.Date == today);
                totalLabReportsRequested = todaysLabRequests.Count;
                // All lab records for today
                labReportsReady = todaysLabRequests.Count();
                labReportsPercentage = totalLabReportsRequested > 0
                    ? (decimal)labReportsReady / totalLabReportsRequested * 100
                    : 0;
            }
            catch
            {
                // Lab reports not available or error
                logger.LogWarning("Could not fetch lab reports data");
            }

            // Revenue calculation (Note: ConsultationFee not in current schema, using placeholder)
            decimal revenueToday = 0;
            decimal averageDailyRevenue = 0;
            decimal revenuePercentageChange = 0;
            
            // TODO: Implement revenue calculation when ConsultationFee field is added to T_DoctorDetails
            // For now, estimating based on completed appointments
            revenueToday = todaysAppointments.Count(a => a.Status == AppointmentStatus_Enum.Completed) * 100; // Placeholder: $100 per appointment

            // Active doctors today
            var currentDayName = DateTime.UtcNow.ToString("dddd");
            var allDoctors = await uow.DoctorDetailsRepo.GetAllAsync();
            var activeDoctorsToday = 0;

            foreach (var doctor in allDoctors)
            {
                var user = await userManager.FindByIdAsync(doctor.UserID);
                if (user != null && user.IsActive &&
                    !string.IsNullOrEmpty(doctor.AvailableDays) &&
                    doctor.AvailableDays.Contains(currentDayName, StringComparison.OrdinalIgnoreCase))
                {
                    activeDoctorsToday++;
                }
            }

            // New patients today
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var newPatientsToday = 0;
            if (patientRole != null)
            {
                var allUsers = await userManager.GetUsersInRoleAsync(patientRole.Name!);
                newPatientsToday = allUsers.Count(u => u.CreatedOn.Date == today);
            }

            // Pending and cancelled appointments
            var pendingAppointments = todaysAppointments.Count(a =>
                a.Status == AppointmentStatus_Enum.Pending ||
                a.Status == AppointmentStatus_Enum.Created);
            var cancelledAppointmentsToday = todaysAppointments.Count(a =>
                a.Status == AppointmentStatus_Enum.Cancelled);

            var metrics = new TodayPerformanceMetrics_DTO
            {
                TotalScheduledToday = totalScheduledToday,
                CheckedInToday = checkedInToday,
                CheckInPercentage = Math.Round(checkInPercentage, 1),
                CompletedAppointments = completedAppointments,
                TotalAppointmentsToday = totalScheduledToday,
                CompletionPercentage = Math.Round(completionPercentage, 1),
                LabReportsReady = labReportsReady,
                TotalLabReportsRequested = totalLabReportsRequested,
                LabReportsPercentage = Math.Round(labReportsPercentage, 1),
                RevenueToday = revenueToday,
                AverageDailyRevenue = averageDailyRevenue,
                RevenuePercentageChange = Math.Round(revenuePercentageChange, 1),
                IsRevenueAboveAverage = revenueToday >= averageDailyRevenue,
                ActiveDoctorsToday = activeDoctorsToday,
                TotalDoctors = allDoctors.Count,
                NewPatientsToday = newPatientsToday,
                PendingAppointments = pendingAppointments,
                CancelledAppointmentsToday = cancelledAppointmentsToday
            };

            logger.LogInformation("Today's performance metrics calculated successfully");
            return Result<TodayPerformanceMetrics_DTO>.Success(metrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating today's performance metrics");
            return Result<TodayPerformanceMetrics_DTO>.Exception(ex);
        }
    }

    #endregion

    #region Additional Dashboard Widgets

    public async Task<Result<UserDistributionStats_DTO>> GetUserDistributionStatsAsync()
    {
        logger.LogInformation("Executing: GetUserDistributionAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var firstDayThisMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            // Get role counts
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
            var adminRole = await roleManager.FindByNameAsync(RoleType.Admin.ToString());
            var labRole = await roleManager.FindByNameAsync(RoleType.Lab.ToString());

            var allPatients = patientRole != null ? (await userManager.GetUsersInRoleAsync(patientRole.Name!)).ToList() : new();
            var allDoctors = doctorRole != null ? (await userManager.GetUsersInRoleAsync(doctorRole.Name!)).ToList() : new();
            var allAdmins = adminRole != null ? (await userManager.GetUsersInRoleAsync(adminRole.Name!)).ToList() : new();
            var allLabs = labRole != null ? (await userManager.GetUsersInRoleAsync(labRole.Name!)).ToList() : new();

            // Calculate percentages
            var patientsThisMonth = allPatients.Count(u => u.CreatedOn >= firstDayThisMonth);
            var patientsLastMonth = allPatients.Count(u => u.CreatedOn >= firstDayLastMonth && u.CreatedOn < firstDayThisMonth);
            var patientChange = patientsLastMonth > 0 ? ((decimal)(patientsThisMonth - patientsLastMonth) / patientsLastMonth) * 100 : 0;

            var doctorsThisMonth = allDoctors.Count(u => u.CreatedOn >= firstDayThisMonth);
            var doctorsLastMonth = allDoctors.Count(u => u.CreatedOn >= firstDayLastMonth && u.CreatedOn < firstDayThisMonth);
            var doctorChange = doctorsLastMonth > 0 ? ((decimal)(doctorsThisMonth - doctorsLastMonth) / doctorsLastMonth) * 100 : 0;

            var adminsThisMonth = allAdmins.Count(u => u.CreatedOn >= firstDayThisMonth);
            var adminsLastMonth = allAdmins.Count(u => u.CreatedOn >= firstDayLastMonth && u.CreatedOn < firstDayThisMonth);
            var adminChange = adminsLastMonth > 0 ? ((decimal)(adminsThisMonth - adminsLastMonth) / adminsLastMonth) * 100 : 0;

            var labsThisMonth = allLabs.Count(u => u.CreatedOn >= firstDayThisMonth);
            var labsLastMonth = allLabs.Count(u => u.CreatedOn >= firstDayLastMonth && u.CreatedOn < firstDayThisMonth);
            var labChange = labsLastMonth > 0 ? ((decimal)(labsThisMonth - labsLastMonth) / labsLastMonth) * 100 : 0;

            var stats = new UserDistributionStats_DTO
            {
                TotalPatients = allPatients.Count,
                PatientsPercentageChange = Math.Round(patientChange, 1),
                TotalDoctors = allDoctors.Count,
                DoctorsPercentageChange = Math.Round(doctorChange, 1),
                TotalAdminStaff = allAdmins.Count,
                AdminPercentageChange = Math.Round(adminChange, 1),
                TotalLabStaff = allLabs.Count,
                LabPercentageChange = Math.Round(labChange, 1),
                TotalUsers = allPatients.Count + allDoctors.Count + allAdmins.Count + allLabs.Count
            };

            return Result<UserDistributionStats_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user distribution");
            return Result<UserDistributionStats_DTO>.Exception(ex);
        }
    }

    public async Task<Result<MonthlyStatistics_DTO>> GetMonthlyStatsAsync()
    {
        logger.LogInformation("Executing: GetMonthlyStatisticsAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var firstDayThisMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayLastMonth = firstDayThisMonth.AddMonths(-1);

            // New registrations
            var allUsers = await userManager.Users.ToListAsync();
            var newRegsThisMonth = allUsers.Count(u => u.CreatedOn >= firstDayThisMonth);
            var newRegsLastMonth = allUsers.Count(u => u.CreatedOn >= firstDayLastMonth && u.CreatedOn < firstDayThisMonth);
            var regChange = newRegsLastMonth > 0 ? ((decimal)(newRegsThisMonth - newRegsLastMonth) / newRegsLastMonth) * 100 : 0;

            // Appointments
            var apptThisMonth = await uow.AppointmentsRepo.GetAllAsync(a => a.AppointmentDate >= firstDayThisMonth);
            var apptLastMonth = await uow.AppointmentsRepo.GetAllAsync(a =>
                a.AppointmentDate >= firstDayLastMonth && a.AppointmentDate < firstDayThisMonth);
            var apptChange = apptLastMonth.Count > 0 ? ((decimal)(apptThisMonth.Count - apptLastMonth.Count) / apptLastMonth.Count) * 100 : 0;

            // Lab tests (if available)
            int labTestsThisMonth = 0;
            int labTestsLastMonth = 0;
            decimal labChange = 0;
            try
            {
                var labsThisMonth = await uow.LabRepo.GetAllAsync(l => l.CreatedOn >= firstDayThisMonth);
                var labsLastMonth = await uow.LabRepo.GetAllAsync(l =>
                    l.CreatedOn >= firstDayLastMonth && l.CreatedOn < firstDayThisMonth);
                labTestsThisMonth = labsThisMonth.Count;
                labTestsLastMonth = labsLastMonth.Count;
                labChange = labTestsLastMonth > 0 ? ((decimal)(labTestsThisMonth - labTestsLastMonth) / labTestsLastMonth) * 100 : 0;
            }
            catch { logger.LogWarning("Lab services not available"); }

            // Revenue (Placeholder calculation - TODO: Add ConsultationFee to T_DoctorDetails)
            decimal revenueThisMonth = apptThisMonth.Count(a => a.Status == AppointmentStatus_Enum.Completed) * 100;
            decimal revenueLastMonth = apptLastMonth.Count(a => a.Status == AppointmentStatus_Enum.Completed) * 100;
            var revenueChange = revenueLastMonth > 0 ? ((revenueThisMonth - revenueLastMonth) / revenueLastMonth) * 100 : 0;

            var stats = new MonthlyStatistics_DTO
            {
                NewRegistrationsThisMonth = newRegsThisMonth,
                RegistrationPercentageChange = Math.Round(regChange, 1),
                TotalAppointmentsThisMonth = apptThisMonth.Count,
                AppointmentPercentageChange = Math.Round(apptChange, 1),
                LabTestsCompletedThisMonth = labTestsThisMonth,
                LabTestsPercentageChange = Math.Round(labChange, 1),
                RevenueThisMonth = revenueThisMonth,
                RevenuePercentageChange = Math.Round(revenueChange, 1)
            };

            return Result<MonthlyStatistics_DTO>.Success(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting monthly statistics");
            return Result<MonthlyStatistics_DTO>.Exception(ex);
        }
    }

    public async Task<Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>> GetPatientRegTrendsAsync()
    {
        logger.LogInformation("Executing: GetPatientRegistrationTrendsAsync");
        try
        {
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            if (patientRole == null)
                return Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>.Success(new Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO());

            var allPatients = await userManager.GetUsersInRoleAsync(patientRole.Name!);
            var today = DateTime.UtcNow;
            var monthlyData = new List<MonthlyRegistration>();

            for (int i = 11; i >= 0; i--)
            {
                var month = today.AddMonths(-i);
                var firstDay = new DateTime(month.Year, month.Month, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);

                var count = allPatients.Count(p => p.CreatedOn >= firstDay && p.CreatedOn <= lastDay);

                monthlyData.Add(new MonthlyRegistration
                {
                    MonthName = month.ToString("MMM"),
                    Year = month.Year,
                    Count = count,
                    FormattedMonth = month.ToString("MMM yyyy")
                });
            }

            var totalNewPatients = allPatients.Count(p => p.CreatedOn >= today.AddMonths(-12));
            var avgPerMonth = monthlyData.Count > 0 ? monthlyData.Average(m => m.Count) : 0;

            // Determine trend
            var lastThree = monthlyData.TakeLast(3).Select(m => m.Count).ToList();
            var trend = lastThree.Count >= 3 && lastThree[2] > lastThree[0] ? "Up" :
                       lastThree.Count >= 3 && lastThree[2] < lastThree[0] ? "Down" : "Stable";

            var trends = new Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO
            {
                MonthlyData = monthlyData,
                TotalNewPatients = totalNewPatients,
                AveragePerMonth = Math.Round((decimal)avgPerMonth, 1),
                TrendDirection = trend
            };

            return Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>.Success(trends);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting patient registration trends");
            return Result<Contracts.AdminDashboardDTOs.PatientRegistrationTrends_DTO>.Exception(ex);
        }
    }

    public async Task<Result<AppointmentStatusBreakdown_DTO>> GetAppointmentStatusAsync()
    {
        logger.LogInformation("Executing: GetAppointmentStatusBreakdownAsync");
        try
        {
            var allAppointments = await uow.AppointmentsRepo.GetAllAsync();

            var created = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Created);
            var pending = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Pending);
            var approved = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Approved);
            var rejected = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Rejected);
            var completed = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Completed);
            var cancelled = allAppointments.Count(a => a.Status == AppointmentStatus_Enum.Cancelled);
            var total = allAppointments.Count;

            var breakdown = new AppointmentStatusBreakdown_DTO
            {
                ScheduledCount = created,
                ApprovedCount = approved,
                CompletedCount = completed,
                CancelledCount = cancelled,
                PendingCount = pending,
                TotalAppointments = total,
                ScheduledPercentage = total > 0 ? Math.Round((decimal)created / total * 100, 1) : 0,
                ApprovedPercentage = total > 0 ? Math.Round((decimal)approved / total * 100, 1) : 0,
                CompletedPercentage = total > 0 ? Math.Round((decimal)completed / total * 100, 1) : 0,
                CancelledPercentage = total > 0 ? Math.Round((decimal)cancelled / total * 100, 1) : 0,
                PendingPercentage = total > 0 ? Math.Round((decimal)pending / total * 100, 1) : 0
            };

            return Result<AppointmentStatusBreakdown_DTO>.Success(breakdown);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting appointment status breakdown");
            return Result<AppointmentStatusBreakdown_DTO>.Exception(ex);
        }
    }

    public async Task<Result<TodaysAppointmentsList_DTO>> GetTodaysApptsListAsync()
    {
        logger.LogInformation("Executing: GetTodaysAppointmentsListAsync");
        try
        {
            var today = DateTime.UtcNow.Date;
            var todaysAppointments = await uow.AppointmentsRepo.GetAllAsync(a => a.AppointmentDate.Date == today);

            var appointmentItems = new List<TodayAppointmentItem>();

            foreach (var appt in todaysAppointments.OrderBy(a => a.AppointmentDate).Take(10))
            {
                var patientDetails = await uow.PatientDetailsRepo.GetByIdAsync(appt.PatientID);
                var doctorDetails = await uow.DoctorDetailsRepo.GetByIdAsync(appt.DoctorID);

                var patientUser = patientDetails != null ? await userManager.FindByIdAsync(patientDetails.UserID) : null;
                var doctorUser = doctorDetails != null ? await userManager.FindByIdAsync(doctorDetails.UserID) : null;

                appointmentItems.Add(new TodayAppointmentItem
                {
                    AppointmentID = appt.AppointmentID,
                    PatientID = appt.PatientID,
                    PatientName = patientUser != null ? $"{patientUser.FirstName} {patientUser.LastName}".Trim() : "Unknown",
                    DoctorID = appt.DoctorID,
                    DoctorName = doctorUser != null ? $"Dr. {doctorUser.FirstName} {doctorUser.LastName}".Trim() : "Unknown",
                    DoctorSpecialization = doctorDetails?.Specialization,
                    AppointmentDate = appt.AppointmentDate.Date,
                    AppointmentTime = appt.AppointmentDate,
                    Status = appt.Status.ToString(),
                    AppointmentType = appt.AppointmentType.ToString(),
                    Reason = appt.Reason
                });
            }

            var list = new TodaysAppointmentsList_DTO
            {
                Appointments = appointmentItems,
                TotalToday = todaysAppointments.Count,
                CompletedToday = todaysAppointments.Count(a => a.Status == AppointmentStatus_Enum.Completed),
                PendingToday = todaysAppointments.Count(a => a.Status == AppointmentStatus_Enum.Pending || a.Status == AppointmentStatus_Enum.Created)
            };

            return Result<TodaysAppointmentsList_DTO>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting today's appointments list");
            return Result<TodaysAppointmentsList_DTO>.Exception(ex);
        }
    }

    public async Task<Result<TodaysAppointmentsList_DTO>> GetAllAppointmentsAsync()
    {
        logger.LogInformation("Executing: GetAllAppointmentsAsync");
        try
        {
            // Get all appointments, not just today's
            var allAppointments = await uow.AppointmentsRepo.GetAllAsync();

            var appointmentItems = new List<TodayAppointmentItem>();

            foreach (var appt in allAppointments.OrderByDescending(a => a.AppointmentDate))
            {
                var patientDetails = await uow.PatientDetailsRepo.GetByIdAsync(appt.PatientID);
                var doctorDetails = await uow.DoctorDetailsRepo.GetByIdAsync(appt.DoctorID);

                var patientUser = patientDetails != null ? await userManager.FindByIdAsync(patientDetails.UserID) : null;
                var doctorUser = doctorDetails != null ? await userManager.FindByIdAsync(doctorDetails.UserID) : null;

                appointmentItems.Add(new TodayAppointmentItem
                {
                    AppointmentID = appt.AppointmentID,
                    PatientID = appt.PatientID,
                    PatientName = patientUser != null ? $"{patientUser.FirstName} {patientUser.LastName}".Trim() : "Unknown",
                    DoctorID = appt.DoctorID,
                    DoctorName = doctorUser != null ? $"Dr. {doctorUser.FirstName} {doctorUser.LastName}".Trim() : "Unknown",
                    DoctorSpecialization = doctorDetails?.Specialization,
                    AppointmentDate = appt.AppointmentDate.Date,
                    AppointmentTime = appt.AppointmentDate,
                    Status = appt.Status.ToString(),
                    AppointmentType = appt.AppointmentType.ToString(),
                    Reason = appt.Reason
                });
            }

            var list = new TodaysAppointmentsList_DTO
            {
                Appointments = appointmentItems,
                TotalToday = appointmentItems.Count(a => a.AppointmentDate.Date == DateTime.UtcNow.Date),
                CompletedToday = appointmentItems.Count(a => a.AppointmentDate.Date == DateTime.UtcNow.Date && a.Status == "Approved"),
                PendingToday = appointmentItems.Count(a => a.AppointmentDate.Date == DateTime.UtcNow.Date && (a.Status == "Pending" || a.Status == "Created"))
            };

            return Result<TodaysAppointmentsList_DTO>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all appointments list");
            return Result<TodaysAppointmentsList_DTO>.Exception(ex);
        }
    }

    public async Task<Result<RecentLabResults_DTO>> GetRecentLabsAsync()
    {
        logger.LogInformation("Executing: GetRecentLabResultsAsync");
        try
        {
            var labResults = new List<LabResultItem>();
            int totalPending = 0;
            int totalCompletedToday = 0;

            try
            {
                var today = DateTime.UtcNow.Date;
                // Note: T_Lab entity represents lab facilities, not lab tests/results
                // Using placeholder data until proper lab results schema is confirmed
                var recentLabs = await uow.LabRepo.GetAllAsync();
                recentLabs = recentLabs.OrderByDescending(l => l.CreatedOn).Take(10).ToList();

                foreach (var lab in recentLabs)
                {
                    // Placeholder lab data
                    labResults.Add(new LabResultItem
                    {
                        LabServiceID = lab.LabID,
                        PatientName = "Lab Facility",
                        TestName = lab.LabName ?? "Lab Test",
                        RequestDate = lab.CreatedOn,
                        CompletionDate = lab.UpdatedOn,
                        Status = "Active",
                        IsUrgent = false,
                        Notes = $"Location: {lab.Location}"
                    });
                }

                var allLabs = await uow.LabRepo.GetAllAsync();
                totalPending = 0;
                totalCompletedToday = allLabs.Count(l => l.UpdatedOn?.Date == today);
            }
            catch
            {
                logger.LogWarning("Lab services not available");
            }

            var results = new RecentLabResults_DTO
            {
                Results = labResults,
                TotalPendingResults = totalPending,
                TotalCompletedToday = totalCompletedToday
            };

            return Result<RecentLabResults_DTO>.Success(results);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting recent lab results");
            return Result<RecentLabResults_DTO>.Exception(ex);
        }
    }

    #endregion

    #region Doctor Profile Management

    public async Task<Result<DoctorProfile_DTO>> GetDoctorProfileAsync(string userId)
    {
        logger.LogInformation("Executing: GetDoctorProfileAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<DoctorProfile_DTO>.Failure(null!, "Doctor not found", System.Net.HttpStatusCode.NotFound);

            var doctorDetails = await uow.DoctorDetailsRepo.GetAsync(d => d.UserID == userId);
            if (doctorDetails == null)
                return Result<DoctorProfile_DTO>.Failure(null!, "Doctor details not found", System.Net.HttpStatusCode.NotFound);

            var appointments = await uow.AppointmentsRepo.GetAllAsync(a => a.DoctorID == doctorDetails.DoctorID);
            var today = DateTime.UtcNow.Date;
            
            var profile = new DoctorProfile_DTO
            {
                UserId = user.Id,
                DoctorId = doctorDetails.DoctorID,
                FirstName = user.FirstName,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage ?? "/theme/images/default-doctor.png",
                DateOfBirth = user.DateOfBirth,
                Age = user.Age ?? 0,
                Gender = user.Gender.ToString(),
                Address = user.Address,
                
                // Professional Information
                Specialization = doctorDetails.Specialization,
                LicenseNumber = doctorDetails.LicenseNumber,
                ExperienceYears = doctorDetails.ExperienceYears,
                HospitalAffiliation = doctorDetails.HospitalAffiliation,
                Qualifications = doctorDetails.QualificationSummary,
                About = null, // Not available in current model
                
                // Schedule
                AvailableDays = doctorDetails.AvailableDays,
                StartTime = doctorDetails.StartTime,
                EndTime = doctorDetails.EndTime,
                ConsultationFee = null, // Not available in current model
                
                // Statistics
                TotalPatients = appointments.Select(a => a.PatientID).Distinct().Count(),
                TotalAppointments = appointments.Count,
                CompletedAppointments = appointments.Count(a => a.Status == AppointmentStatus_Enum.Completed),
                TodayAppointments = appointments.Count(a => a.AppointmentDate.Date == today),
                Rating = 4.5m, // Placeholder
                ReviewCount = 50, // Placeholder
                
                // Status
                IsActive = user.IsActive,
                CreatedOn = user.CreatedOn,
                UpdatedOn = user.UpdatedOn
            };

            return Result<DoctorProfile_DTO>.Success(profile);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor profile");
            return Result<DoctorProfile_DTO>.Exception(ex);
        }
    }

    public async Task<Result<GeneralResponse>> UpdateDoctorAsync(string userId, UpdateDoctor_DTO updateDto)
    {
        logger.LogInformation("Executing: UpdateDoctorAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Doctor not found" });

            // Update user information
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.DateOfBirth = updateDto.DateOfBirth;
            user.Address = updateDto.Address;
            user.UpdatedOn = DateTime.UtcNow;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<GeneralResponse>.Failure(new GeneralResponse { Success = false, Message = "Failed to update user information" });

            // Update doctor details
            var doctorDetails = await uow.DoctorDetailsRepo.GetAsync(d => d.UserID == userId);
            if (doctorDetails != null)
            {
                doctorDetails.Specialization = updateDto.Specialization;
                doctorDetails.LicenseNumber = updateDto.LicenseNumber;
                doctorDetails.ExperienceYears = updateDto.ExperienceYears;
                doctorDetails.HospitalAffiliation = updateDto.HospitalAffiliation;
                doctorDetails.QualificationSummary = updateDto.Qualifications;
                doctorDetails.AvailableDays = updateDto.AvailableDays ?? "Monday,Tuesday,Wednesday,Thursday,Friday";
                doctorDetails.StartTime = updateDto.StartTime;
                doctorDetails.EndTime = updateDto.EndTime;
                // ConsultationFee not available in current model
                doctorDetails.UpdatedOn = DateTime.UtcNow;

                await uow.DoctorDetailsRepo.UpdateAsync(doctorDetails);
                await uow.SaveChangesAsync();
            }

            return Result<GeneralResponse>.Success(new GeneralResponse { Success = true, Message = "Doctor updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating doctor");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    public async Task<Result<DoctorSchedule_DTO>> GetDoctorScheduleAsync(string userId)
    {
        logger.LogInformation("Executing: GetDoctorScheduleAsync for {UserId}", userId);
        try
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<DoctorSchedule_DTO>.Failure(null!, "Doctor not found", System.Net.HttpStatusCode.NotFound);

            var doctorDetails = await uow.DoctorDetailsRepo.GetAsync(d => d.UserID == userId);
            if (doctorDetails == null)
                return Result<DoctorSchedule_DTO>.Failure(null!, "Doctor details not found", System.Net.HttpStatusCode.NotFound);

            var today = DateTime.UtcNow.Date;
            var weekEnd = today.AddDays(7);
            
            var appointments = await uow.AppointmentsRepo.GetAllAsync(
                a => a.DoctorID == doctorDetails.DoctorID && 
                     a.AppointmentDate >= today && 
                     a.AppointmentDate <= weekEnd);
            
            // Load patient data separately
            var patientIds = appointments.Select(a => a.PatientID).Distinct().ToList();
            var patients = await uow.PatientDetailsRepo.GetAllAsync(p => patientIds.Contains(p.PatientID));
            var patientUsers = new Dictionary<int, T_Users>();
            foreach (var patient in patients)
            {
                var patientUser = await userManager.FindByIdAsync(patient.UserID);
                if (patientUser != null)
                    patientUsers[patient.PatientID] = patientUser;
            }

            var schedule = new DoctorSchedule_DTO
            {
                UserId = userId,
                DoctorName = $"Dr. {user.FirstName} {user.LastName}",
                Specialization = doctorDetails.Specialization,
                AvailableDays = doctorDetails.AvailableDays,
                StartTime = doctorDetails.StartTime,
                EndTime = doctorDetails.EndTime,
                TodaySlots = appointments
                    .Where(a => a.AppointmentDate.Date == today)
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a => new DoctorAppointmentSlot
                    {
                        SlotTime = a.AppointmentDate,
                        PatientName = patientUsers.ContainsKey(a.PatientID) ? 
                            $"{patientUsers[a.PatientID].FirstName} {patientUsers[a.PatientID].LastName}" : "Unknown",
                        Status = a.Status.ToString(),
                        Reason = a.Reason
                    }).ToList(),
                WeekSlots = appointments
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a => new DoctorAppointmentSlot
                    {
                        SlotTime = a.AppointmentDate,
                        PatientName = patientUsers.ContainsKey(a.PatientID) ? 
                            $"{patientUsers[a.PatientID].FirstName} {patientUsers[a.PatientID].LastName}" : "Unknown",
                        Status = a.Status.ToString(),
                        Reason = a.Reason
                    }).ToList()
            };

            return Result<DoctorSchedule_DTO>.Success(schedule);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor schedule");
            return Result<DoctorSchedule_DTO>.Exception(ex);
        }
    }

    public async Task<Result<List<PatientList_DTO>>> GetDoctorPatientsAsync(string userId)
    {
        logger.LogInformation("Executing: GetDoctorPatientsAsync for {UserId}", userId);
        try
        {
            var doctorDetails = await uow.DoctorDetailsRepo.GetAsync(d => d.UserID == userId);
            if (doctorDetails == null)
                return Result<List<PatientList_DTO>>.Failure(null!, "Doctor not found", System.Net.HttpStatusCode.NotFound);

            var appointments = await uow.AppointmentsRepo.GetAllAsync(
                a => a.DoctorID == doctorDetails.DoctorID);

            var uniquePatientIds = appointments.Select(a => a.PatientID).Distinct().ToList();
            var patients = await uow.PatientDetailsRepo.GetAllAsync(
                p => uniquePatientIds.Contains(p.PatientID));

            var result = new List<PatientList_DTO>();
            
            foreach (var patient in patients)
            {
                var user = await userManager.FindByIdAsync(patient.UserID);
                if (user == null) continue;

                var patientAppointments = appointments.Where(a => a.PatientID == patient.PatientID).ToList();
                var lastVisit = patientAppointments
                    .Where(a => a.Status == AppointmentStatus_Enum.Completed)
                    .OrderByDescending(a => a.AppointmentDate)
                    .FirstOrDefault()?.AppointmentDate;

                result.Add(new PatientList_DTO
                {
                    PatientID = patient.PatientID,
                    UserID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Gender = user.Gender.ToString(),
                    DateOfBirth = user.DateOfBirth,
                    Age = user.Age ?? 0,
                    BloodGroup = patient.BloodGroup,
                    MaritalStatus = patient.MaritalStatus,
                    Occupation = patient.Occupation,
                    IsActive = user.IsActive,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                    CreatedOn = user.CreatedOn,
                    Address = user.Address,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactNumber = patient.EmergencyContactNumber,
                    RelationshipToEmergency = patient.RelationshipToEmergency,
                    TotalAppointments = patientAppointments.Count,
                    LastVisit = lastVisit
                });
            }

            return Result<List<PatientList_DTO>>.Success(result.OrderBy(p => p.FirstName).ToList());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctor patients");
            return Result<List<PatientList_DTO>>.Exception(ex);
        }
    }

    #endregion
}
