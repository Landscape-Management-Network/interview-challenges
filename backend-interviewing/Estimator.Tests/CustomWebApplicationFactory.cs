using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Estimator.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = null!;

    public Task InitializeAsync()
    {
        HttpClient = CreateClient();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        HttpClient.Dispose();
        return Task.CompletedTask;
    }
}
