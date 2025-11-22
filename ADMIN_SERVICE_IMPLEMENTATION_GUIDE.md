# Admin Service Complete Implementation Guide

## 🎯 Overview
Complete implementation guide for AdminService with all required endpoints and DTOs for the CareSync Admin Dashboard.

---

## ✅ Completed Work

### 1. **DTOs Created** (4 new files)

#### `DashboardSummary_DTO.cs`
- Complete dashboard summary with all metrics
- `StatsCard_DTO` - Statistical cards with percentage changes
- `UrgentItem_DTO` - Urgent items requiring attention
- `TodayPerformance_DTO` - Today's performance metrics
- `TodayAppointment_DTO` - Today's appointment details

#### `UserDistribution_DTO.cs`
- User distribution across all roles
- `RoleDistribution_DTO` - Distribution per role
- `RegistrationTrends_DTO` - Registration trends for charts
- `MonthlyData_DTO` - Monthly trend data points
- `AppointmentStatusChart_DTO` - Appointment status distribution

#### `DoctorList_DTO.cs`
- Doctor list with essential information
- `DoctorStats_DTO` - Doctor statistics summary
- Includes appointment counts and specialization

#### `PatientList_DTO.cs`
- Patient list with essential information
- `PatientStats_DTO` - Patient statistics summary
- `PatientSearch_DTO` - Patient search results
- Includes demographics and visit history

### 2. **IAdminService Interface** ✅ Updated

Complete interface with 22 methods organized into:
- **Dashboard & Analytics** (8 methods)
- **Doctor Management** (4 methods)
- **Patient Management** (5 methods)
- **Appointment Management** (2 methods)
- **Admin User Management** (3 methods)

---

## 🔧 AdminService Implementation

### Replace the existing AdminService.cs with this structure:

```csharp
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
    #region Dashboard & Analytics Methods
    
    // 1. GetDashboardStatsAsync() - ALREADY IMPLEMENTED (GetAllAppointmentsAsyn)
    //    Just rename it to match interface
    
    // 2. GetDashboardSummaryAsync() - Combines all dashboard data
    public async Task<Result<DashboardSummary_DTO>> GetDashboardSummaryAsync()
    {
        // Fetch all dashboard data in parallel
        // Combine stats, urgent items, performance, appointments
        // Return complete summary
    }
    
    // 3. GetUrgentItemsAsync() - Get pending approvals, critical lab results
    // 4. GetTodayPerformanceAsync() - Today's metrics
    // 5. GetUserDistributionAsync() - User counts by role
    // 6. GetRegistrationTrendsAsync() - 6-month trends
    // 7. GetAppointmentStatusChartAsync() - Status distribution
    // 8. GetTodaysAppointmentsAsync() - Today's appointment list
    
    #endregion
    
    #region Doctor Management
    
    // 1. GetAllDoctorsAsync() - List with filtering
    // 2. GetDoctorStatsAsync() - Statistics
    // 3. GetDoctorByIdAsync() - Single doctor details
    // 4. ToggleDoctorStatusAsync() - Activate/deactivate
    
    #endregion
    
    #region Patient Management
    
    // 1. GetAllPatientsAsync() - List with filtering
    // 2. GetPatientStatsAsync() - Statistics
    // 3. GetPatientByIdAsync() - Single patient details
    // 4. SearchPatientsAsync() - Search by name/email/phone
    // 5. TogglePatientStatusAsync() - Activate/deactivate
    
    #endregion
    
    #region Appointment Management
    
    // 1. CreateAppointmentAsync() - Create new appointment
    // 2. CreateAppointmentWithQuickPatientAsync() - Quick registration
    
    #endregion
    
    #region Admin User Management
    
    // 1. GetUserAdminAsync() - NEEDS userId parameter
    // 2. UpdateUserAdminAsync() - UNCOMMENT and complete
    // 3. DeleteUserAdminAsync() - Implement soft delete
    
    #endregion
    
    #region Helper Methods
    
    private decimal CalculatePercentage(int thisMonth, int lastMonth)
    {
        if (lastMonth == 0)
            return thisMonth == 0 ? 0 : 100;
        return Math.Round(((decimal)(thisMonth - lastMonth) / lastMonth) * 100, 2);
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
    
    private async Task<int> GetNewPatientsTodayCount()
    {
        var today = DateTime.UtcNow.Date;
        var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
        return await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id && u.CreatedOn.Date == today);
    }
    
    private async Task<int> GetPatientCountForMonth(DateTime start, DateTime end)
    {
        var patientRole = await roleManager.FindByNameAsync(RoleType.Patient.ToString());
        return await uow.UserRepo.GetCountAsync(u => u.RoleID == patientRole!.Id && u.CreatedOn >= start && u.CreatedOn <= end);
    }
    
    private async Task<int> GetDoctorCountForMonth(DateTime start, DateTime end)
    {
        var doctorRole = await roleManager.FindByNameAsync(RoleType.Doctor.ToString());
        return await uow.UserRepo.GetCountAsync(u => u.RoleID == doctorRole!.Id && u.CreatedOn >= start && u.CreatedOn <= end);
    }
    
    private async Task<int> GetAppointmentCountForMonth(DateTime start, DateTime end)
    {
        return await uow.AppointmentsRepo.GetCountAsync(a => a.CreatedOn >= start && a.CreatedOn <= end);
    }
    
    #endregion
}
```

---

## 📋 Implementation Steps

### Step 1: Fix Existing Methods

**Update `GetUserAdminAsync`:**
```csharp
public async Task<Result<T_Users>> GetUserAdminAsync(string userId)
{
    logger.LogInformation("Executing: GetUserAdminAsync for {UserId}", userId);
    var result = await uow.UserRepo.GetByIdAsync(userId);
    if (result == null)
        return Result<T_Users>.Failure(null!, "User not found", System.Net.HttpStatusCode.NotFound);
    return Result<T_Users>.Success(result);
}
```

**Uncomment and Fix `UpdateUserAdminAsync`:**
```csharp
public async Task<Result<GeneralResponse>> UpdateUserAdminAsync(UserAdminUpdate_DTO request)
{
    logger.LogInformation("Executing: UpdateUserAdminAsync");
    try
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result<GeneralResponse>.Failure(new GeneralResponse
            {
                Success = false,
                Message = "User not found"
            });

        mapper.Map(request, user);
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
```

**Implement `DeleteUserAdminAsync`:**
```csharp
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
```

**Rename existing method:**
```csharp
// Change this:
public async Task<Result<GetFirstRowCardsData_DTO>> GetAllAppointmentsAsyn()

// To this:
public async Task<Result<GetFirstRowCardsData_DTO>> GetDashboardStatsAsync()
```

---

### Step 2: Add Helper Methods

Add all the private helper methods shown above to the AdminService class.

---

### Step 3: Implement Dashboard Methods

All dashboard methods follow a similar pattern:
1. Calculate date ranges (today, this month, last month)
2. Query repositories with appropriate filters
3. Transform to DTO
4. Return Result<T>

Example template:
```csharp
public async Task<Result<SomeDTO>> GetSomeDataAsync()
{
    logger.LogInformation("Executing: GetSomeDataAsync");
    try
    {
        // Query data
        var data = await uow.SomeRepo.GetAsync(filter, includeProperties);
        
        // Transform to DTO
        var result = data.Select(x => new SomeDTO { ... }).ToList();
        
        return Result<SomeDTO>.Success(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error getting data");
        return Result<SomeDTO>.Exception(ex);
    }
}
```

---

### Step 4: Implement Doctor Management

```csharp
public async Task<Result<List<DoctorList_DTO>>> GetAllDoctorsAsync(string? specialization = null, bool? isActive = null)
{
    var doctors = await uow.DoctorDetailsRepo.GetAsync(
        d => (specialization == null || d.Specialization == specialization),
        includeProperties: "User,Appointments");

    var result = doctors
        .Where(d => isActive == null || d.User!.IsActive == isActive)
        .Select(d => new DoctorList_DTO
        {
            DoctorID = d.DoctorID,
            FirstName = d.User?.FirstName ?? "",
            // ... map all fields
        }).ToList();

    return Result<List<DoctorList_DTO>>.Success(result);
}
```

---

### Step 5: Implement Patient Management

Similar pattern to Doctor Management but using PatientDetailsRepo.

---

### Step 6: Implement Appointment Methods

```csharp
public async Task<Result<GeneralResponse>> CreateAppointmentAsync(AddAppointment_DTO appointment)
{
    try
    {
        var entity = mapper.Map<T_Appointments>(appointment);
        await uow.AppointmentsRepo.AddAsync(entity);
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
```

---

## 📡 AdminController Updates

### Add These Endpoints:

```csharp
[Route("api/[controller]")]
[ApiController]
public class AdminController(IAdminService adminService) : ControllerBase
{
    // ========== Dashboard ==========
    
    [HttpGet("dashboard/stats")]
    public async Task<Result<GetFirstRowCardsData_DTO>> GetDashboardStats()
        => await adminService.GetDashboardStatsAsync();
    
    [HttpGet("dashboard/summary")]
    public async Task<Result<DashboardSummary_DTO>> GetDashboardSummary()
        => await adminService.GetDashboardSummaryAsync();
    
    [HttpGet("dashboard/urgent-items")]
    public async Task<Result<List<UrgentItem_DTO>>> GetUrgentItems()
        => await adminService.GetUrgentItemsAsync();
    
    [HttpGet("dashboard/today-performance")]
    public async Task<Result<TodayPerformance_DTO>> GetTodayPerformance()
        => await adminService.GetTodayPerformanceAsync();
    
    [HttpGet("dashboard/user-distribution")]
    public async Task<Result<UserDistribution_DTO>> GetUserDistribution()
        => await adminService.GetUserDistributionAsync();
    
    [HttpGet("dashboard/registration-trends")]
    public async Task<Result<RegistrationTrends_DTO>> GetRegistrationTrends()
        => await adminService.GetRegistrationTrendsAsync();
    
    [HttpGet("dashboard/appointment-status-chart")]
    public async Task<Result<AppointmentStatusChart_DTO>> GetAppointmentStatusChart()
        => await adminService.GetAppointmentStatusChartAsync();
    
    [HttpGet("dashboard/todays-appointments")]
    public async Task<Result<List<TodayAppointment_DTO>>> GetTodaysAppointments()
        => await adminService.GetTodaysAppointmentsAsync();
    
    // ========== Doctors ==========
    
    [HttpGet("doctors")]
    public async Task<Result<List<DoctorList_DTO>>> GetAllDoctors(
        [FromQuery] string? specialization = null, 
        [FromQuery] bool? isActive = null)
        => await adminService.GetAllDoctorsAsync(specialization, isActive);
    
    [HttpGet("doctors/stats")]
    public async Task<Result<DoctorStats_DTO>> GetDoctorStats()
        => await adminService.GetDoctorStatsAsync();
    
    [HttpGet("doctors/{id}")]
    public async Task<Result<DoctorList_DTO>> GetDoctorById(int id)
        => await adminService.GetDoctorByIdAsync(id);
    
    [HttpPatch("doctors/{userId}/toggle-status")]
    public async Task<Result<GeneralResponse>> ToggleDoctorStatus(string userId, [FromQuery] bool isActive)
        => await adminService.ToggleDoctorStatusAsync(userId, isActive);
    
    // ========== Patients ==========
    
    [HttpGet("patients")]
    public async Task<Result<List<PatientList_DTO>>> GetAllPatients(
        [FromQuery] string? bloodGroup = null, 
        [FromQuery] bool? isActive = null)
        => await adminService.GetAllPatientsAsync(bloodGroup, isActive);
    
    [HttpGet("patients/stats")]
    public async Task<Result<PatientStats_DTO>> GetPatientStats()
        => await adminService.GetPatientStatsAsync();
    
    [HttpGet("patients/{id}")]
    public async Task<Result<PatientList_DTO>> GetPatientById(int id)
        => await adminService.GetPatientByIdAsync(id);
    
    [HttpGet("patients/search")]
    public async Task<Result<List<PatientSearch_DTO>>> SearchPatients([FromQuery] string searchTerm)
        => await adminService.SearchPatientsAsync(searchTerm);
    
    [HttpPatch("patients/{userId}/toggle-status")]
    public async Task<Result<GeneralResponse>> TogglePatientStatus(string userId, [FromQuery] bool isActive)
        => await adminService.TogglePatientStatusAsync(userId, isActive);
    
    // ========== Appointments ==========
    
    [HttpPost("appointments")]
    public async Task<Result<GeneralResponse>> CreateAppointment([FromBody] AddAppointment_DTO appointment)
        => await adminService.CreateAppointmentAsync(appointment);
    
    [HttpPost("appointments/quick-patient")]
    public async Task<Result<GeneralResponse>> CreateAppointmentWithQuickPatient([FromBody] AddAppointmentWithQuickPatient_DTO input)
        => await adminService.CreateAppointmentWithQuickPatientAsync(input);
}
```

---

## 🎯 Testing Endpoints

### Test Dashboard:
```powershell
# Get dashboard stats
curl http://localhost:5157/api/Admin/dashboard/stats

# Get complete summary
curl http://localhost:5157/api/Admin/dashboard/summary

# Get urgent items
curl http://localhost:5157/api/Admin/dashboard/urgent-items
```

### Test Doctors:
```powershell
# Get all doctors
curl http://localhost:5157/api/Admin/doctors

# Filter by specialization
curl "http://localhost:5157/api/Admin/doctors?specialization=Cardiology"

# Get doctor stats
curl http://localhost:5157/api/Admin/doctors/stats

# Get specific doctor
curl http://localhost:5157/api/Admin/doctors/1
```

### Test Patients:
```powershell
# Get all patients
curl http://localhost:5157/api/Admin/patients

# Search patients
curl "http://localhost:5157/api/Admin/patients/search?searchTerm=John"

# Get patient stats
curl http://localhost:5157/api/Admin/patients/stats
```

---

## ✅ Summary

### Files Created:
1. ✅ `DashboardSummary_DTO.cs`
2. ✅ `UserDistribution_DTO.cs`
3. ✅ `DoctorList_DTO.cs`
4. ✅ `PatientList_DTO.cs`

### Files Updated:
1. ✅ `IAdminService.cs` - Complete interface with 22 methods
2. 🔄 `AdminService.cs` - Needs implementation (follow guide above)
3. 🔄 `AdminController.cs` - Needs endpoint additions

### Total Methods: 22
- Dashboard & Analytics: 8
- Doctor Management: 4  
- Patient Management: 5
- Appointment Management: 2
- Admin User Management: 3

### Total Endpoints: 22+
All matching the service methods with proper routing.

**Status: READY FOR IMPLEMENTATION** 🚀

Follow the implementation guide above to complete the AdminService!
