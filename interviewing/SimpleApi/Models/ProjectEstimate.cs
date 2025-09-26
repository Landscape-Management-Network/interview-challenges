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
    
    // Design-build specific properties
    public List<string> SelectedPackages { get; set; } = new List<string>();
    public List<string> SelectedAddOns { get; set; } = new List<string>();
    public string ClientTier { get; set; } = "Standard";
    public bool IsRushOrder { get; set; }
    public bool IsRepeatClient { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PackagePrice { get; set; }
    public decimal AddOnPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }
    
    // Recurring service specific properties
    public string ServiceType { get; set; } = "Maintenance"; // "Maintenance", "Repair", "Installation", "Consultation"
    public int ServiceFrequency { get; set; } = 1; // Times per month
    public bool IsRecurring { get; set; } = false;
    public decimal HourlyRate { get; set; }
    public int EstimatedHours { get; set; }
    public decimal MaterialCost { get; set; }
    public decimal LaborCost { get; set; }
    public decimal EquipmentCost { get; set; }
    public decimal TravelCost { get; set; }
    public bool RequiresPermits { get; set; }
    public bool HasEnvironmentalConcerns { get; set; }
    public string Location { get; set; } = "Local";
    public string Season { get; set; } = "Spring";
    public int TeamSize { get; set; } = 1;
}

/// <summary>
/// Discount rule model - this needs to be extracted to a separate file
/// </summary>
public class DiscountRule
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public string DiscountType { get; set; } = "Percentage"; // "Percentage" or "FixedAmount"
    public int Priority { get; set; }
    public decimal MinimumOrderValue { get; set; }
    public int? MaximumDurationDays { get; set; }
    public bool RequiresRushOrder { get; set; }
    public bool RequiresRepeatClient { get; set; }
    public List<string> ApplicableProjectTypes { get; set; } = new List<string>();
    public List<string> ApplicableClientTiers { get; set; } = new List<string>();
    public List<string> RequiredPackages { get; set; } = new List<string>();
    public List<string> RequiredAddOns { get; set; } = new List<string>();
    public bool IsStackable { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}
