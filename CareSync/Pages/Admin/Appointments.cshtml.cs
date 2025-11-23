using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using Microsoft.AspNetCore.Authorization;

namespace CareSync.Pages.Admin;

// [Authorize(Roles = "Admin")] // TODO: Enable after implementing proper authentication flow
public class AppointmentsModel : PageModel
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<AppointmentsModel> _logger;

    public AppointmentsModel(AdminApiService adminApiService, ILogger<AppointmentsModel> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    // Statistics properties
    public int ConfirmedCount { get; set; }
    public int PendingCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int TotalAppointments { get; set; }

    // Appointments list
    public List<TodayAppointmentItem> Appointments { get; set; } = new();
    
    // Chart data
    public AppointmentStatusBreakdown_DTO? AppointmentStatusData { get; set; }
    public List<MonthlyAppointmentData> MonthlyTrends { get; set; } = new();
    public Dictionary<string, int> DepartmentDistribution { get; set; } = new();

    // Pagination
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public string? SearchTerm { get; set; }
    public string? StatusFilter { get; set; }

    public async Task<IActionResult> OnGetAsync(int? page, string? search, string? status)
    {
        try
        {
            CurrentPage = page ?? 1;
            SearchTerm = search;
            StatusFilter = status;

            // Fetch appointment status breakdown for statistics
            var statusTask = _adminApiService.GetAppointmentStatusBreakdownAsync<Result<AppointmentStatusBreakdown_DTO>>();
            
            // Fetch ALL appointments, not just today's
            var appointmentsTask = _adminApiService.GetAllAppointmentsAsync<Result<TodaysAppointmentsList_DTO>>();
            
            // Fetch dashboard stats for additional data
            var statsTask = _adminApiService.GetDashboardStatsAsync<Result<GetFirstRowCardsData_DTO>>();

            await Task.WhenAll(statusTask, appointmentsTask, statsTask);

            // Process appointment status data
            var statusResult = await statusTask;
            if (statusResult?.IsSuccess == true && statusResult.Data != null)
            {
                AppointmentStatusData = statusResult.Data;
                ConfirmedCount = statusResult.Data.ApprovedCount;
                PendingCount = statusResult.Data.PendingCount;
                CompletedCount = statusResult.Data.CompletedCount;
                CancelledCount = statusResult.Data.CancelledCount;
                TotalAppointments = statusResult.Data.TotalAppointments;
            }

            // Process appointments list
            var appointmentsResult = await appointmentsTask;
            if (appointmentsResult?.IsSuccess == true && appointmentsResult.Data != null)
            {
                Appointments = appointmentsResult.Data.Appointments ?? new List<TodayAppointmentItem>();
                
                // Apply filters if needed
                if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "all")
                {
                    Appointments = Appointments.Where(a => 
                        a.Status?.ToLower() == StatusFilter.ToLower()).ToList();
                }
                
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Appointments = Appointments.Where(a => 
                        a.PatientName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                        a.DoctorName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                        a.AppointmentID.ToString().Contains(SearchTerm)).ToList();
                }

                // Calculate department distribution from appointments
                DepartmentDistribution = Appointments
                    .GroupBy(a => a.DoctorSpecialization ?? "General")
                    .ToDictionary(g => g.Key, g => g.Count());
            }

            // Generate monthly trends (mock data for now, should come from backend)
            GenerateMonthlyTrends();

            // Calculate pagination
            TotalPages = (int)Math.Ceiling(Appointments.Count / (double)PageSize);
            Appointments = Appointments
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointments page");
            TempData["Error"] = "Failed to load appointments data";
            return Page();
        }
    }

    private void GenerateMonthlyTrends()
    {
        // Generate last 12 months of data
        var random = new Random();
        for (int i = 11; i >= 0; i--)
        {
            var date = DateTime.Now.AddMonths(-i);
            MonthlyTrends.Add(new MonthlyAppointmentData
            {
                Month = date.ToString("MMM"),
                Year = date.Year,
                Count = random.Next(40, 100)
            });
        }
    }

    public class MonthlyAppointmentData
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Count { get; set; }
    }
}
