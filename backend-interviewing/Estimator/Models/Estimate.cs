using System.ComponentModel.DataAnnotations;

namespace Estimator.Models;

public enum EstimateStatus { Draft, Approved, Rejected }

public class Estimate
{
    public int Id { get; set; }

    [Required]
    public string ClientName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ClientEmail { get; set; } = string.Empty;

    [Required]
    public EstimateStatus Status { get; set; } = EstimateStatus.Draft;

    [Required]
    public decimal TotalPrice { get; set; }
}