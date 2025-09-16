using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RobRef.DDD.Domain.Users;

namespace RobRef.DDD.WebApi.Tests;

public sealed class UsersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterUser_ReturnsCreatedLocationAndId()
    {
        using var client = _factory.CreateClient();

        var payload = new
        {
            email = $"user-{Guid.NewGuid():N}@example.com",
            title = "Dr",
            firstName = "Ada",
            lastName = "Lovelace"
        };

        var response = await client.PostAsJsonAsync("/api/users/register", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(body);

        var id = body!.RootElement.GetProperty("id").GetString();
        Assert.False(string.IsNullOrWhiteSpace(id));
        Assert.Equal(UserId.Length, id!.Length);
        Assert.Equal($"/api/users/{id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task RegisterUser_WithInvalidPayload_ReturnsValidationProblemDetails()
    {
        using var client = _factory.CreateClient();

        var payload = new
        {
            email = "invalid-email-format",
            title = "",
            firstName = string.Empty,
            lastName = ""
        };

        var response = await client.PostAsJsonAsync("/api/users/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(body);

        var root = body!.RootElement;
        Assert.Equal("https://robref.ddd/problems/validation-error", root.GetProperty("type").GetString());
        Assert.Equal("Validation Failed", root.GetProperty("title").GetString());
        Assert.Equal((int)HttpStatusCode.BadRequest, root.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrWhiteSpace(root.GetProperty("detail").GetString()));

        var errors = root.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Email", out var emailErrors));
        Assert.True(emailErrors.GetArrayLength() > 0);
        Assert.True(errors.TryGetProperty("FirstName", out var firstNameErrors));
        Assert.True(firstNameErrors.GetArrayLength() > 0);
        Assert.True(errors.TryGetProperty("LastName", out var lastNameErrors));
        Assert.True(lastNameErrors.GetArrayLength() > 0);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ReturnsConflictProblemDetails()
    {
        using var client = _factory.CreateClient();

        var duplicateEmail = $"duplicate-{Guid.NewGuid():N}@example.com";

        var firstPayload = new
        {
            email = duplicateEmail,
            title = "Ms",
            firstName = "Grace",
            lastName = "Hopper"
        };

        var firstResponse = await client.PostAsJsonAsync("/api/users/register", firstPayload);
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondPayload = new
        {
            email = duplicateEmail,
            title = "Ms",
            firstName = "Grace",
            lastName = "Hopper"
        };

        var secondResponse = await client.PostAsJsonAsync("/api/users/register", secondPayload);

        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);

        var body = await secondResponse.Content.ReadFromJsonAsync<JsonDocument>();
        Assert.NotNull(body);

        var root = body!.RootElement;
        Assert.Equal("https://robref.ddd/problems/user-already-exists", root.GetProperty("type").GetString());
        Assert.Equal("User Already Exists", root.GetProperty("title").GetString());
        Assert.Equal((int)HttpStatusCode.Conflict, root.GetProperty("status").GetInt32());

        var detail = root.GetProperty("detail").GetString();
        Assert.False(string.IsNullOrWhiteSpace(detail));
        Assert.Contains(duplicateEmail, detail!);
    }
}
