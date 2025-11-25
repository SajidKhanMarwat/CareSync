namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// DTO for patient age distribution
/// </summary>
public class PatientAgeDistribution_DTO
{
    public int Age0To18 { get; set; }
    public int Age19To35 { get; set; }
    public int Age36To50 { get; set; }
    public int Age51To65 { get; set; }
    public int Age65Plus { get; set; }
    
    public List<int> GetSeriesData()
    {
        return new List<int> { Age0To18, Age19To35, Age36To50, Age51To65, Age65Plus };
    }
}

/// <summary>
/// DTO for patient demographics (gender and marital status)
/// </summary>
public class PatientDemographics_DTO
{
    public int FemaleCount { get; set; }
    public int MaleCount { get; set; }
    public int MarriedCount { get; set; }
    public int SingleCount { get; set; }
    public int DivorcedCount { get; set; }
    public int WidowedCount { get; set; }
    
    public List<int> GetGenderMaritalData()
    {
        return new List<int> { FemaleCount, MaleCount, MarriedCount, SingleCount, DivorcedCount, WidowedCount };
    }
}
