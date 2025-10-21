using Microsoft.EntityFrameworkCore;
using Estimator.Models;

namespace Estimator;

public class EstimatesContext(DbContextOptions<EstimatesContext> options) : DbContext(options)
{
    public DbSet<Estimate> Estimates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
