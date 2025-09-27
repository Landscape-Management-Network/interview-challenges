using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Controllers;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.Tests;

public sealed class ProjectEstimatesControllerTests : IDisposable
{
    private readonly ProjectContext _context;
    private readonly ProjectEstimatesController _controller;

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

        _controller = new ProjectEstimatesController(_context);
    }

    [Fact]
    public async Task GetProjectEstimates_ReturnsAllEstimates()
    {
        // Arrange
        var estimate1 = new ProjectEstimate { ProjectName = "Test Project 1", ClientName = "John Doe", ClientEmail = "john@test.com", EstimatedCost = 10000 };
        var estimate2 = new ProjectEstimate { ProjectName = "Test Project 2", ClientName = "Jane Smith", ClientEmail = "jane@test.com", EstimatedCost = 20000 };

        _context.ProjectEstimates.AddRange(estimate1, estimate2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetProjectEstimates();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var estimates = Assert.IsAssignableFrom<IEnumerable<ProjectEstimate>>(okResult.Value);
        Assert.Equal(2, estimates.Count());
    }

    [Fact]
    public async Task GetProjectEstimate_WithValidId_ReturnsEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "Test Project", ClientName = "John Doe", ClientEmail = "john@test.com", EstimatedCost = 15000 };
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetProjectEstimate(estimate.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedEstimate = Assert.IsType<ProjectEstimate>(okResult.Value);
        Assert.Equal(estimate.ProjectName, returnedEstimate.ProjectName);
    }

    [Fact]
    public async Task GetProjectEstimate_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetProjectEstimate(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProjectEstimate_WithValidData_CreatesEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "New Project", ClientName = "John Doe", ClientEmail = "john@test.com", EstimatedCost = 25000 };

        // Act
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(estimate.ProjectName, createdEstimate.ProjectName);

        // Verify it was saved to database
        var savedEstimate = await _context.ProjectEstimates.FindAsync(createdEstimate.Id);
        Assert.NotNull(savedEstimate);
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Standard_ShortTerm_CalculatesCorrectPrice()
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
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(3750m, createdEstimate.EstimatedCost);
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Custom_MediumTerm_CalculatesCorrectPrice()
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
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(10000m, createdEstimate.EstimatedCost); // 2500 * 2.0 = 5000
    }

    [Fact]
    public async Task CreateProjectEstimate_DesignBuild_Peak_LongTerm_CalculatesCorrectPrice()
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
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(7200m, createdEstimate.EstimatedCost); // 1800 * 4.0 = 7200
    }

    [Fact]
    public async Task CreateProjectEstimate_RecurringService_Maintenance_CalculatesCorrectPrice()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Lawn Maintenance", 
            ClientName = "Alice Brown", 
            ClientEmail = "alice@test.com",
            EstimateKind = "RecurringService",
            ServiceType = "Maintenance",
            ProjectType = "Standard",
            EstimatedHours = 8,
            MaterialCost = 50m,
            EquipmentCost = 25m,
            TravelCost = 15m
        };

        // Act
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(690m, createdEstimate.EstimatedCost); // (75 * 8 * 1.0) + 50 + 25 + 15 = 690
    }

    [Fact]
    public async Task CreateProjectEstimate_RecurringService_Repair_Peak_CalculatesCorrectPrice()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Emergency Repair", 
            ClientName = "Charlie Davis", 
            ClientEmail = "charlie@test.com",
            EstimateKind = "RecurringService",
            ServiceType = "Repair",
            ProjectType = "Peak",
            EstimatedHours = 4,
            MaterialCost = 100m,
            EquipmentCost = 50m,
            TravelCost = 20m
        };

        // Act
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(830m, createdEstimate.EstimatedCost); // (95 * 4 * 2.0) + 100 + 50 + 20 = 830
    }

    [Fact]
    public async Task CreateProjectEstimate_RecurringService_Recurring_AppliesDiscount()
    {
        // Arrange
        var estimate = new ProjectEstimate 
        { 
            ProjectName = "Monthly Maintenance", 
            ClientName = "David Lee", 
            ClientEmail = "david@test.com",
            EstimateKind = "RecurringService",
            ServiceType = "Maintenance",
            ProjectType = "Standard",
            EstimatedHours = 6,
            MaterialCost = 30m,
            EquipmentCost = 20m,
            TravelCost = 10m,
            IsRecurring = true
        };

        // Act
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(540m, createdEstimate.EstimatedCost); // ((75 * 6 * 1.0) + 30 + 20 + 10) * 0.9 = 540
    }

    [Fact]
    public async Task CreateProjectEstimate_InvalidEstimateKind_ReturnsZero()
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
        var result = await _controller.CreateProjectEstimate(estimate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdEstimate = Assert.IsType<ProjectEstimate>(createdResult.Value);
        Assert.Equal(0m, createdEstimate.EstimatedCost);
    }

    [Fact]
    public async Task DeleteProjectEstimate_WithValidId_DeletesEstimate()
    {
        // Arrange
        var estimate = new ProjectEstimate { ProjectName = "To Delete", ClientName = "John Doe", ClientEmail = "john@test.com", EstimatedCost = 10000 };
        _context.ProjectEstimates.Add(estimate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteProjectEstimate(estimate.Id);

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