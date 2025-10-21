using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estimator.Models;

namespace Estimator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstimatesController(EstimatesContext context) : ControllerBase
{
    private readonly EstimatesContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Estimate>>> GetEstimates()
    {
        return Ok(await _context.Estimates.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Estimate>> GetEstimate(int id)
    {
        var estimate = await _context.Estimates.FindAsync(id);
        
        if (estimate == null)
        {
            return NotFound();
        }

        return Ok(estimate);
    }

    [HttpPost]
    public async Task<ActionResult<Estimate>> CreateEstimate(Estimate estimate)
    {
        _context.Estimates.Add(estimate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEstimate), new { id = estimate.Id }, estimate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEstimate(int id, Estimate estimate)
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
            if (!_context.Estimates.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return Ok(estimate);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstimate(int id)
    {
        var estimate = await _context.Estimates.FindAsync(id);
        if (estimate == null)
        {
            return NotFound();
        }

        _context.Estimates.Remove(estimate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
