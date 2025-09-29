using System.ComponentModel.DataAnnotations;

namespace SimpleApi.Models;

public class ProjectEstimate
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string ProjectName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(150)]
    public string ClientName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string ClientEmail { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)]
    public decimal EstimatedCost { get; set; }
    
    public string Status { get; set; } = "Draft";
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    // Combined properties for both design-build and recurring service estimates
    public string EstimateKind { get; set; } = "DesignBuild"; // "DesignBuild" or "RecurringService"
    public string ProjectType { get; set; } = "Standard"; // "Standard", "Custom", "Peak", "OffSeason", "Rush", "Emergency"
    public int EstimatedDurationDays { get; set; }
    
    // Recurring service specific properties
    public string ServiceType { get; set; } = "Maintenance"; // "Maintenance", "Repair", "Installation", "Consultation"
    public bool IsRecurring { get; set; } = false;
    public int EstimatedHours { get; set; }
    public decimal MaterialCost { get; set; }
    public decimal EquipmentCost { get; set; }
    public decimal TravelCost { get; set; }
}

