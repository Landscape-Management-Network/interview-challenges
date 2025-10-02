using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

// create a base api controller class which has the base route
[Route("api/ProjectEstimates")]
[ApiController]
public class BaseProjectEstimatesController : ControllerBase
{   protected bool ProjectEstimateExists(ProjectContext context, int id)
    {
        return context.ProjectEstimates.Any(e => e.Id == id);
    }
}

public class GetProjectEstimatesController(ProjectContext context) : BaseProjectEstimatesController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectEstimate>>> GetProjectEstimates()
    {
        return Ok(await context.ProjectEstimates.ToListAsync());
    }
}

public class GetProjectEstimateController(ProjectContext context) : BaseProjectEstimatesController
{

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectEstimate>> GetProjectEstimate(int id)
    {
        var estimate = await context.ProjectEstimates.FindAsync(id);
        
        if (estimate == null)
        {
            return NotFound();
        }

        return Ok(estimate);
    }
}

public class CreateProjectEstimatesController(ProjectContext context) : BaseProjectEstimatesController
{
    [HttpPost]
    public async Task<ActionResult<ProjectEstimate>> CreateProjectEstimate(ProjectEstimate estimate)
    {
        // Calculate pricing based on estimate kind
        switch (estimate.EstimateKind)
        {
            case "DesignBuild":
                estimate.TotalEstimatedCost = CalculateDesignBuildPricing(estimate);
                break;
            case "RecurringService":
                estimate.MonthlyEstimatedCost = CalculateRecurringServicePricing(estimate);
                break;
            case "OnDemand":
                estimate.TotalEstimatedCost = CalculateOnDemandPricing(estimate);
                break;
            default:
                return BadRequest($"Invalid EstimateKind: {estimate.EstimateKind}");
        }
        
        context.ProjectEstimates.Add(estimate);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateProjectEstimate), new { id = estimate.Id }, estimate);
    }

    /// <summary>
    /// Calculate pricing for Design-Build projects based on duration and project type
    /// </summary>
    private static decimal CalculateDesignBuildPricing(ProjectEstimate estimate)
    {
        // Base pricing based on project duration
        var basePrice = estimate.EstimatedDurationDays switch
        {
            <= 30 => 3750m,  // Short-term
            <= 90 => 2500m,  // Medium-term
            _ => 1800m       // Long-term
        };

        // Apply multipliers based on project type
        var multiplier = estimate.ProjectType switch
        {
            "Custom" => 2.0m,
            "Peak" => 4.0m,
            "OffSeason" => 0.8m,
            "Rush" => 3.2m,
            "Emergency" => 4.8m,
            _ => 1.0m // Standard
        };

        return basePrice * multiplier;
    }

    /// <summary>
    /// Calculate monthly pricing for Recurring Service projects
    /// </summary>
    private static decimal CalculateRecurringServicePricing(ProjectEstimate estimate)
    {
        return estimate.PerVisitCost * estimate.VisitsPerMonth;
    }

    /// <summary>
    /// Calculate pricing for On-Demand work
    /// Base Rate: $150 per service, Travel Fee: $25 for locations >15 miles from base
    /// </summary>
    private static decimal CalculateOnDemandPricing(ProjectEstimate estimate)
    {
        const decimal baseRate = 150m;
        const decimal travelFee = 25m;
        const decimal travelDistanceThreshold = 15m;

        var totalCost = baseRate;

        // Add travel fee if distance exceeds threshold
        if (estimate.DistanceFromBase > travelDistanceThreshold)
        {
            totalCost += travelFee;
        }

        return totalCost;
    }
}

public class UpdateProjectEstimateController(ProjectContext context) : BaseProjectEstimatesController
{
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjectEstimate(int id, ProjectEstimate estimate)
    {
        if (id != estimate.Id)
        {
            return BadRequest();
        }

        context.Entry(estimate).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProjectEstimateExists(context, id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }
}

public class DeleteProjectEstimateController(ProjectContext context) : BaseProjectEstimatesController
{
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectEstimate(int id)
    {
        var estimate = await context.ProjectEstimates.FindAsync(id);
        if (estimate == null)
        {
            return NotFound();
        }

        context.ProjectEstimates.Remove(estimate);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
