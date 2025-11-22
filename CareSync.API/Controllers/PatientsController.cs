using CareSync.APIs.Controllers;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareSync.API.Controllers;

/// <summary>
/// Patient controller for patient-specific operations and dashboard
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Patient")] // Secure all patient endpoints
public class PatientsController(IPatientService patientService, ILogger<PatientsController> logger) : ControllerBase
{
    /// <summary>
    /// Get patient dashboard data including profile, statistics, visits, and reports
    /// </summary>
    /// <returns>Complete patient dashboard information</returns>
    [HttpGet("dashboard")]
    public async Task<Result<PatientDashboard_DTO>> GetDashboard()
    {
        try
        {
            // Get user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims. User may not be authenticated.");
                return Result<PatientDashboard_DTO>.Failure(
                    new PatientDashboard_DTO(),
                    "User not authenticated. Please log in.");
            }

            logger.LogInformation($"Getting dashboard for patient user: {userId}");
            return await patientService.GetPatientDashboardAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving patient dashboard");
            return Result<PatientDashboard_DTO>.Exception(ex);
        }
    }

    /// <summary>
    /// Get patient dashboard by user ID (Admin use)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Patient dashboard data</returns>
    [HttpGet("dashboard/{userId}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<Result<PatientDashboard_DTO>> GetDashboardByUserId(string userId)
    {
        logger.LogInformation($"Admin/Doctor requesting dashboard for patient: {userId}");
        return await patientService.GetPatientDashboardAsync(userId);
    }

    /// <summary>
    /// Update patient profile
    /// </summary>
    /// <param name="userUpdate_DTO">Profile update data</param>
    /// <returns>Update result</returns>
    [HttpPost("update-patient-profile")]
    public async Task<Result<GeneralResponse>> UpdateUserProfile([FromBody] UserPatientProfileUpdate_DTO userUpdate_DTO)
    {
        logger.LogInformation($"Updating patient profile for user: {userUpdate_DTO.UserId}");
        return await patientService.UpdateUserPatientAsync(userUpdate_DTO);
    }

    /// <summary>
    /// Get all patients (Admin use)
    /// </summary>
    /// <returns>List of all patients</returns>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<Result<List<GetAllPatients_DTO>>> GetAllPatients()
    {
        logger.LogInformation("Getting all patients");
        return await patientService.GetAllPatientsAsync();
    }

    /// <summary>
    /// Get patient by ID (Admin/Doctor use)
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <returns>Patient details</returns>
    [HttpGet("{patientId}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<Result<GetPatient_DTO>> GetPatientById(int patientId)
    {
        logger.LogInformation($"Getting patient by ID: {patientId}");
        return await patientService.GetPatientByIdAsync(patientId);
    }

    #region Appointment Booking

    /// <summary>
    /// Get all available doctors for appointment booking
    /// </summary>
    /// <param name="specialization">Optional specialization filter</param>
    /// <returns>List of available doctors with their schedules</returns>
    [HttpGet("doctors/available")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<DoctorBooking_DTO>>> GetAvailableDoctors([FromQuery] string? specialization = null)
    {
        logger.LogInformation($"Getting available doctors. Specialization: {specialization ?? "All"}");
        return await patientService.GetAvailableDoctorsAsync(specialization);
    }

    /// <summary>
    /// Get specific doctor details by ID
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <returns>Doctor details with availability</returns>
    [HttpGet("doctors/{doctorId}")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<DoctorBooking_DTO>> GetDoctorById(int doctorId)
    {
        logger.LogInformation($"Getting doctor details for ID: {doctorId}");
        return await patientService.GetDoctorByIdAsync(doctorId);
    }

    /// <summary>
    /// Get available time slots for a doctor on a specific date
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="date">Date to check availability (yyyy-MM-dd)</param>
    /// <returns>List of time slots with availability status</returns>
    [HttpGet("doctors/{doctorId}/timeslots")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<List<DoctorTimeSlot_DTO>>> GetDoctorTimeSlots(
        int doctorId,
        [FromQuery] DateTime date)
    {
        logger.LogInformation($"Getting time slots for doctor {doctorId} on {date:yyyy-MM-dd}");
        return await patientService.GetDoctorTimeSlotsAsync(doctorId, date);
    }

    /// <summary>
    /// Book an appointment with a doctor
    /// </summary>
    /// <param name="request">Appointment booking details</param>
    /// <returns>Booking confirmation</returns>
    [HttpPost("appointments/book")]
    [AllowAnonymous] // TODO: Remove after testing
    public async Task<Result<GeneralResponse>> BookAppointment([FromBody] BookAppointmentRequest_DTO request)
    {
        try
        {
            // Get user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID not found in claims for appointment booking");
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "User not authenticated. Please log in to book an appointment."
                });
            }

            if (!ModelState.IsValid)
            {
                return Result<GeneralResponse>.Failure(new GeneralResponse
                {
                    Success = false,
                    Message = "Invalid appointment data. Please check all required fields."
                });
            }

            logger.LogInformation($"Booking appointment for user {userId} with doctor {request.DoctorID}");
            return await patientService.BookAppointmentAsync(userId, request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error booking appointment");
            return Result<GeneralResponse>.Exception(ex);
        }
    }

    #endregion
}
