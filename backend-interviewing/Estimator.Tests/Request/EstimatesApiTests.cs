using Estimator.Models;

namespace Estimator.Tests;

public class EstimatesApiTests(CustomWebApplicationFactory factory) : BaseApiTest(factory)
{
    [Fact]
    public async Task GetEstimates()
    {
        var estimate = new Estimate
        {
            ClientName = "Test Client",
            ClientEmail = "client@example.com",
            Status = EstimateStatus.Draft,
            TotalPrice = 100M
        };

        WithinDbContext(estimatesContext =>
        {
            estimatesContext.Estimates.Add(estimate);
        });

        var result = await AuthenticatedGet("/api/estimates");

        AssertJsonContains(result, "[].id", estimate.Id);
        AssertJsonContains(result, "[].clientName", "Test Client");
        AssertJsonContains(result, "[].clientEmail", "client@example.com");
        AssertJsonContains(result, "[].status", (int)EstimateStatus.Draft);
        AssertJsonContains(result, "[].totalPrice", 100M);
    }

    [Fact]
    public async Task CreateEstimate()
    {
        var result = await AuthenticatedPost("/api/estimates", new
        {
            clientName = "Test Client",
            clientEmail = "client@example.com",
            status = (int)EstimateStatus.Draft,
            totalPrice = 100M
        });

        AssertJsonContains(result, "clientName", "Test Client");
        AssertJsonContains(result, "clientEmail", "client@example.com");
        AssertJsonContains(result, "status", (int)EstimateStatus.Draft);
        AssertJsonContains(result, "totalPrice", 100M);
    }

    [Fact]
    public async Task UpdateEstimate()
    {
        var estimate = new Estimate
        {
            ClientName = "Test Client",
            ClientEmail = "client@example.com",
            Status = EstimateStatus.Draft,
            TotalPrice = 100M
        };

        await WithinDbContextAsync(async estimatesContext =>
        {
            await estimatesContext.Estimates.AddAsync(estimate);
        });

        var result = await AuthenticatedPut($"/api/estimates/{estimate.Id}", new
        {
            id = estimate.Id,
            clientName = "Changed",
            clientEmail = "client@example.com",
            status = (int)EstimateStatus.Draft,
            totalPrice = 100M
        });

        AssertJsonContains(result, "id", estimate.Id);
        AssertJsonContains(result, "clientName", "Changed");
        AssertJsonContains(result, "clientEmail", "client@example.com");
        AssertJsonContains(result, "status", (int)EstimateStatus.Draft);
        AssertJsonContains(result, "totalPrice", 100M);

        await WithinDbContextAsync(async estimatesContext =>
        {
            Assert.Equal("Changed", (await estimatesContext.Estimates.FindAsync(estimate.Id))!.ClientName);
        });
    }

    [Fact]
    public async Task DeleteEstimate()
    {
        var estimate = new Estimate
        {
            ClientName = "Test Client",
            ClientEmail = "client@example.com",
            Status = EstimateStatus.Draft,
            TotalPrice = 100M
        };

        await WithinDbContextAsync(async estimatesContext =>
        {
            await estimatesContext.Estimates.AddAsync(estimate);
        });

        await AuthenticatedDelete($"/api/estimates/{estimate.Id}");

        await WithinDbContextAsync(async estimatesContext =>
        {
            Assert.Null(await estimatesContext.Estimates.FindAsync(estimate.Id));
        });
    }
}