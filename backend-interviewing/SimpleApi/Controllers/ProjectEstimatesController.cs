using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectEstimatesController(ProjectContext context) : ControllerBase
{
    private readonly ProjectContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectEstimate>>> GetProjectEstimates()
    {
        return Ok(await _context.ProjectEstimates.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectEstimate>> GetProjectEstimate(int id)
    {
        var estimate = await _context.ProjectEstimates.FindAsync(id);
        
        if (estimate == null)
        {
            return NotFound();
        }

        return Ok(estimate);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectEstimate>> CreateProjectEstimate(ProjectEstimate estimate)
    {
        // Calculate pricing based on estimate kind
        estimate.EstimatedCost = CalculatePricing(estimate);
        
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjectEstimate), new { id = estimate.Id }, estimate);
    }

    /// <summary>
    /// Calculate pricing for both design-build and service estimates
    /// </summary>
    private decimal CalculatePricing(ProjectEstimate estimate)
    {
        // Set base price based on estimate kind
        decimal basePrice;
        if (estimate.EstimateKind == "DesignBuild")
        {
            // Base pricing for Standard projects
            basePrice = estimate.EstimatedDurationDays switch
            {
                <= 30 => 3750m,  // Short-term
                <= 90 => 2500m,  // Medium-term
                _ => 1800m       // Long-term
            };
        }
        else if (estimate.EstimateKind == "RecurringService")
        {
            // Base hourly rate by service type
            var baseHourlyRate = estimate.ServiceType switch
            {
                "Maintenance" => 75m,
                "Repair" => 95m,
                "Installation" => 120m,
                "Consultation" => 150m,
                _ => 75m // Default to Maintenance
            };

            // Calculate base labor cost
            basePrice = baseHourlyRate * estimate.EstimatedHours;
        }
        else
        {
            basePrice = 0m; // Default fallback
        }

        // Apply multipliers based on project type
        var multiplier = estimate.ProjectType switch
        {
            "Custom" => estimate.EstimateKind == "DesignBuild" ? 2.0m : 1.5m,
            "Peak" => estimate.EstimateKind == "DesignBuild" ? 4.0m : 2.0m,
            "OffSeason" => estimate.EstimateKind == "DesignBuild" ? 0.8m : 0.7m,
            "Rush" => estimate.EstimateKind == "DesignBuild" ? 3.2m : 1.8m,
            "Emergency" => estimate.EstimateKind == "DesignBuild" ? 4.8m : 2.5m,
            _ => 1.0m // Standard
        };

        // Calculate total cost
        var totalCost = basePrice * multiplier;

        // Add additional costs for recurring service estimates
        if (estimate.EstimateKind == "RecurringService")
        {
            totalCost += estimate.MaterialCost + estimate.EquipmentCost + estimate.TravelCost;

            // Apply recurring service discount
            if (estimate.IsRecurring)
            {
                totalCost *= 0.9m; // 10% discount for recurring services
            }
        }

        return totalCost;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjectEstimate(int id, ProjectEstimate estimate)
    {
        if (id != estimate.Id)
        {
            return BadRequest();
        }

        _context.Entry(estimate).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectEstimateExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectEstimate(int id)
    {
        var estimate = await _context.ProjectEstimates.FindAsync(id);
        if (estimate == null)
        {
            return NotFound();
        }

        _context.ProjectEstimates.Remove(estimate);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProjectEstimateExists(int id)
    {
        return _context.ProjectEstimates.Any(e => e.Id == id);
    }

}
