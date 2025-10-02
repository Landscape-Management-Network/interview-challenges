using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Controllers;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Tests;

public sealed class ProjectEstimatesControllerTests : IDisposable
{
    private readonly ProjectContext _context;

    public ProjectEstimatesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ProjectContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProjectContext(options);
        _context.Database.EnsureCreated();

        // Clear seeded data
        _context.ProjectEstimates.RemoveRange(_context.ProjectEstimates);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProjectEstimates_ReturnsAllEstimates()
    {
        // Arrange
        var estimate1 = new ProjectEstimate { ProjectName = "Test Project 1", ClientName = "John Doe", ClientEmail = "john@test.com", TotalEstimatedCost = 10000 };
        var estimate2 = new ProjectEstimate { ProjectName = "Test Project 2", ClientName = "Jane Smith", ClientEmail = "jane@test.com", TotalEstimatedCost = 20000 };

        _context.ProjectEstimates.AddRange(estimate1, estimate2);
        await _context.SaveChangesAsync();

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.GetProjectEstimates();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var estimates = Assert.IsAssignableFrom<IEnumerable<ProjectEstimate>>(okResult.Value);
        Assert.Equal(2, estimates.Count());
    }

    [Fact]
    public async Task GetProjectEstimate_WithValidId_ReturnsEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "Test Project", ClientName = "John Doe", ClientEmail = "john@test.com", TotalEstimatedCost = 15000 };
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.GetProjectEstimate(estimate.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEstimate = Assert.IsType<ProjectEstimate>(okResult.Value);
        Assert.Equal(estimate.ProjectName, returnedEstimate.ProjectName);
    }

    [Fact]
    public async Task GetProjectEstimate_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.GetProjectEstimate(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProjectEstimate_WithValidData_CreatesEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "New Project", ClientName = "John Doe", ClientEmail = "john@test.com", TotalEstimatedCost = 25000 };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(estimate.ProjectName, createdEstimate.ProjectName);

        // Verify it was saved to database
        var savedEstimate = await _context.ProjectEstimates.FindAsync(createdEstimate.Id);
        Assert.NotNull(savedEstimate);
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Standard_ShortTerm_CalculatesCorrectTotalCost()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Design Build Test", 
            ClientName = "John Doe", 
            ClientEmail = "john@test.com",
            EstimateKind = "DesignBuild",
            ProjectType = "Standard",
            EstimatedDurationDays = 20
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(3750m, createdEstimate.TotalEstimatedCost);
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Custom_MediumTerm_CalculatesCorrectTotalCost()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Custom Design Build", 
            ClientName = "Jane Smith", 
            ClientEmail = "jane@test.com",
            EstimateKind = "DesignBuild",
            ProjectType = "Custom",
            EstimatedDurationDays = 60
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(5000m, createdEstimate.TotalEstimatedCost); // 2500 * 2.0 = 5000
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Peak_LongTerm_CalculatesCorrectTotalCost()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Peak Season Project", 
            ClientName = "Bob Wilson", 
            ClientEmail = "bob@test.com",
            EstimateKind = "DesignBuild",
            ProjectType = "Peak",
            EstimatedDurationDays = 120
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(7200m, createdEstimate.TotalEstimatedCost); // 1800 * 4.0 = 7200
    }

    [Fact]
    public async Task CreateProjectEstimate_RecurringService_Maintenance_CalculatesCorrectMonthlyCost()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Lawn Maintenance", 
            ClientName = "Alice Brown", 
            ClientEmail = "alice@test.com",
            EstimateKind = "RecurringService",
            ServiceType = "Maintenance",
            PerVisitCost = 690m,
            VisitsPerMonth = 1
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(690m, createdEstimate.MonthlyEstimatedCost); // 690 * 1 = 690
    }


    [Fact]
    public async Task CreateProjectEstimate_RecurringService_Recurring_CalculatesCorrectMonthlyCost()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Monthly Maintenance", 
            ClientName = "David Lee", 
            ClientEmail = "david@test.com",
            EstimateKind = "RecurringService",
            ServiceType = "Maintenance",
            PerVisitCost = 459m,
            VisitsPerMonth = 1,
            IsRecurring = true
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(459m, createdEstimate.MonthlyEstimatedCost); // 459 * 1 = 459
    }

    [Fact]
    public async Task CreateProjectEstimate_InvalidEstimateKind_ReturnsBadRequest()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Invalid Kind", 
            ClientName = "Eve Johnson", 
            ClientEmail = "eve@test.com",
            EstimateKind = "Invalid",
            ProjectType = "Standard",
            EstimatedDurationDays = 30
        };

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.CreateProjectEstimate(estimate);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task DeleteProjectEstimate_WithValidId_DeletesEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "To Delete", ClientName = "John Doe", ClientEmail = "john@test.com", TotalEstimatedCost = 10000 };
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

        // Act
        var controller = new ProjectEstimatesController(_context);
        var result = await controller.DeleteProjectEstimate(estimate.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify it was deleted from database
        var deletedEstimate = await _context.ProjectEstimates.FindAsync(estimate.Id);
        Assert.Null(deletedEstimate);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}