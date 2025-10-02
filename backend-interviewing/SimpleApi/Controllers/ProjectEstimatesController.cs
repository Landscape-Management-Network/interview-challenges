using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Controllers;

// create a base api controller class which has the base route
[Route("api/{controller}")]
[ApiController]
public class ProjectEstimatesController : ControllerBase
{
    private readonly ProjectContext _context;

    public ProjectEstimatesController(ProjectContext context)
    {
        _context = context;
    }
    protected bool ProjectEstimateExists(int id)
    {
        return _context.ProjectEstimates.Any(e => e.Id == id);
    }

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
        
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

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
}
