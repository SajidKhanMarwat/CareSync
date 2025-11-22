using AutoMapper;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
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
    IMapper mapper,
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
                Age = user.Age,
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
                a.Status == AppointmentStatus_Enum.Approved);

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

    public async Task<Result<GeneralResponse>> CreateAppointmentWithQuickPatientAsync(AddAppointmentWithQuickPatient_DTO input)
    {
        logger.LogInformation("Executing: CreateAppointmentWithQuickPatientAsync");
        try
        {
            // First, register the patient if needed
            var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
            
            // Create user account
            var user = new T_Users
            {
                UserName = input.Email,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName ?? string.Empty,
                PhoneNumber = input.PhoneNumber,
                Gender = input.Gender,
                DateOfBirth = input.DateOfBirth,
                RoleType = RoleType.Patient,
                RoleID = patientRole!.Id,
                IsActive = true,
                EmailConfirmed = true,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow,
                ArabicUserName = input.FirstName,
                ArabicFirstName = input.FirstName,
                LoginID = 0
            };

            var result = await userManager.CreateAsync(user, input.Password ?? "Patient@123");
            if (!result.Succeeded)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "Patient registration failed"
                });
            }

            // Create patient details
            var patientDetails = new T_PatientDetails
            {
                UserID = user.Id,
                BloodGroup = input.BloodGroup,
                MaritalStatus = !string.IsNullOrEmpty(input.MaritalStatus) ? Enum.Parse<MaritalStatusEnum>(input.MaritalStatus) : MaritalStatusEnum.Single,
                CreatedBy = "Admin",
                CreatedOn = DateTime.UtcNow
            };

            await uow.PatientDetailsRepo.AddAsync(patientDetails);
            await uow.SaveChangesAsync();

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
            logger.LogError(ex, "Error creating appointment with quick patient");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    #endregion

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
                ConfirmedAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Approved),
                PendingAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Pending),
                CompletedAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Approved),
                CancelledAppointments = await uow.AppointmentsRepo.GetCountAsync(a => a.Status == AppointmentStatus_Enum.Rejected),
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
                    Age = user.Age,
                    BloodGroup = patient.BloodGroup,
                    MaritalStatus = patient.MaritalStatus,
                    Occupation = patient.Occupation,
                    IsActive = user.IsActive,
                    ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                    CreatedOn = user.CreatedOn,
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
                Age = user.Age,
                BloodGroup = patient.BloodGroup,
                MaritalStatus = patient.MaritalStatus,
                Occupation = patient.Occupation,
                IsActive = user.IsActive,
                ProfileImage = user.ProfileImage ?? "/theme/images/default-patient.png",
                CreatedOn = user.CreatedOn,
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
                    Age = user.Age,
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

    #endregion
}
