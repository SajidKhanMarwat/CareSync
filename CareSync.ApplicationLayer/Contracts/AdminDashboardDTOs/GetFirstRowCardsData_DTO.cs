namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

public class GetFirstRowCardsData_DTO
{
    public int TotalAppointments { get; set; }
    public decimal ThisVsLastMonthPercentageAppointment { get; set; }
    public int TotalDoctors { get; set; }
    public decimal ThisVsLastMonthPercentageDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal ThisVsLastMonthPercentagePatients { get; set; }
}
