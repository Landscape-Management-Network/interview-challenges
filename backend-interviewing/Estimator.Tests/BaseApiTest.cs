using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Estimator.Tests;

[Collection("ApiTests")]
public class BaseApiTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    protected string AccessToken = "Test";

    public async Task WithinDbContextAsync(Func<EstimatesContext, Task> action)
    {
        var scope = factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var context = scope.ServiceProvider.GetService<EstimatesContext>()!;
        await action(context);
        await context.SaveChangesAsync();
    }

    public void WithinDbContext(Action<EstimatesContext> action)
    {
        var scope = factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var context = scope.ServiceProvider.GetService<EstimatesContext>()!;
        action(context);
        context.SaveChanges();
    }

    public Task InitializeAsync()
    {
        WithinDbContext(estimatesContext =>
        {
            estimatesContext.Estimates.RemoveRange(estimatesContext.Estimates);
        });

        return Task.CompletedTask;
    }

    public async Task<JsonNode?> AuthenticatedGet(string path)
    {
        return await MakeAuthenticatedCall<JsonNode>(HttpMethod.Get, path);
    }

    public async Task<JsonNode?> AuthenticatedPost(string path, object body)
    {
        return await MakeAuthenticatedCall<JsonNode>(HttpMethod.Post, path, body);
    }

    public async Task<JsonNode?> AuthenticatedPut(string path, object body)
    {
        return await MakeAuthenticatedCall<JsonNode>(HttpMethod.Put, path, body);
    }

    public async Task<JsonNode?> AuthenticatedDelete(string path)
    {
        return await MakeAuthenticatedCall<JsonNode>(HttpMethod.Delete, path);
    }

    public async Task<T?> MakeAuthenticatedCall<T>(HttpMethod method, string path, object? body = null, bool readBodyAsJson = true, string? accept = null, bool ensureSuccess = true) where T : class
    {
        using var message = new HttpRequestMessage(method, path);

        message.Headers.Add("Authorization", $"Bearer {AccessToken}");

        if (accept != null)
        {
            message.Headers.Add("Accept", accept);
        }

        if (body != null)
        {
            message.Content = JsonContent.Create(body);
        }

        var response = await factory.HttpClient.SendAsync(message);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Assert.Fail($"request url not found: {method} {path}");
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return null;
        }

        if (ensureSuccess)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                var errorBody = "";
                if (response.Content.Headers.ContentType!.MediaType == "text/plain")
                {
                    errorBody = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonObject>();
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    errorBody = JsonSerializer.Serialize(result, options);
                }
                Assert.Fail($"request failed. \n Body: {errorBody}");
            }
        }

        if (readBodyAsJson)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
        else
        {
            return response as T;
        }
    }

    public static void AssertJsonContains<T>(JsonNode? node, string path, T value)
    {
        if (node == null || !JsonPathContains(node, path, value))
        {
            Assert.Fail($"'{path} = {value}' not found in response: {node}");
        }
    }

    public static bool JsonPathContains<T>(JsonNode node, string path, T? value)
    {
        if (path.Contains('.'))
        {
            var key = path.Split(".").First();
            var pathRemainder = string.Join(".", path.Split(".").Skip(1));
            if (key == "[]")
            {
                return node.AsArray().Any(item => JsonPathContains(item!, pathRemainder, value));
            }
            if (node[key] == null)
            {
                return false;
            }
            return JsonPathContains(node[key]!, pathRemainder, value);
        }
        else
        {
            if (path == "[]")
            {
                return node.AsArray().Any(item => item!.GetValue<T>()!.Equals(value));
            }
            else
            {
                if (typeof(T).IsEnum)
                {
                    return value!.ToString()!.Equals(node[path]!.GetValue<string>());
                }
                else
                {
                    return node[path] == null ? false : value!.Equals(node[path]!.GetValue<T>());
                }
            }
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
