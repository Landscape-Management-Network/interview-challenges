using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Data;

public class ProjectContext(DbContextOptions<ProjectContext> options) : DbContext(options)
{
    public DbSet<ProjectEstimate> ProjectEstimates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed some initial data
        modelBuilder.Entity<ProjectEstimate>().HasData(
            new ProjectEstimate { Id = 1, ProjectName = "Garden Design", ClientName = "John Smith", ClientEmail = "john@email.com", EstimatedCost = 15000, Status = "Pending", CreatedDate = DateTime.Now.AddDays(-2) },
            new ProjectEstimate { Id = 2, ProjectName = "Pool Renovation", ClientName = "Jane Doe", ClientEmail = "jane@email.com", EstimatedCost = 25000, Status = "Approved", CreatedDate = DateTime.Now.AddDays(-1) },
            new ProjectEstimate { Id = 3, ProjectName = "Commercial Landscaping", ClientName = "ABC Corp", ClientEmail = "contact@abc.com", EstimatedCost = 45000, Status = "Draft", CreatedDate = DateTime.Now }
        );
    }
}
