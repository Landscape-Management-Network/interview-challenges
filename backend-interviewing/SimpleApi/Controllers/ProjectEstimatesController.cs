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
        var cost = CalculatePricing(estimate);
        if(estimate.EstimateKind == "DesignBuild")
        {
            estimate.TotalEstimatedCost = cost;
        }
        else if(estimate.EstimateKind == "RecurringService")
        {
            estimate.MonthlyEstimatedCost = cost;
        }
        else
        {
            return BadRequest();
        }
        
        context.ProjectEstimates.Add(estimate);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateProjectEstimate), new { id = estimate.Id }, estimate);
    }
    /// <summary>
    /// Calculate pricing for both design-build and service estimates
    /// </summary>
    private static decimal CalculatePricing(ProjectEstimate estimate)
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
            basePrice = estimate.PerVisitCost * estimate.VisitsPerMonth;
        }
        else
        {
            basePrice = 0m; // Default fallback
        }

        // Apply multipliers based on project type
        var multiplier = estimate.ProjectType switch
        {
            "Custom" => estimate.EstimateKind == "DesignBuild" ? 2.0m : 1m,
            "Peak" => estimate.EstimateKind == "DesignBuild" ? 4.0m : 1m,
            "OffSeason" => estimate.EstimateKind == "DesignBuild" ? 0.8m : 1m,
            "Rush" => estimate.EstimateKind == "DesignBuild" ? 3.2m : 1m,
            "Emergency" => estimate.EstimateKind == "DesignBuild" ? 4.8m : 1m,
            _ => 1.0m // Standard
        };

        // Calculate total cost
        var totalCost = basePrice * multiplier;

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
