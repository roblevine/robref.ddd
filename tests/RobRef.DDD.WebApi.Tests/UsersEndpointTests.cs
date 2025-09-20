using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RobRef.DDD.Domain.Users;
using RobRef.DDD.WebApi.Users;

namespace RobRef.DDD.WebApi.Tests;

public sealed class UsersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UsersEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ClearRepository(); // Clear repository state before each test
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

    [Fact]
    public async Task GetAllUsers_WithEmptyDatabase_ReturnsEmptyArray()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getAllResponse = await response.Content.ReadFromJsonAsync<GetAllUsersResponse>();
        Assert.NotNull(getAllResponse);
        Assert.NotNull(getAllResponse.Users);
        Assert.Empty(getAllResponse.Users);
    }

    [Fact]
    public async Task GetAllUsers_WithExistingUsers_ReturnsAllUsers()
    {
        using var client = _factory.CreateClient();

        // Register first user
        var user1Payload = new
        {
            email = $"user1-{Guid.NewGuid():N}@example.com",
            title = "Dr",
            firstName = "Ada",
            lastName = "Lovelace"
        };
        var user1Response = await client.PostAsJsonAsync("/api/users/register", user1Payload);
        Assert.Equal(HttpStatusCode.Created, user1Response.StatusCode);

        // Register second user
        var user2Payload = new
        {
            email = $"user2-{Guid.NewGuid():N}@example.com",
            title = (string?)null,
            firstName = "Grace",
            lastName = "Hopper"
        };
        var user2Response = await client.PostAsJsonAsync("/api/users/register", user2Payload);
        Assert.Equal(HttpStatusCode.Created, user2Response.StatusCode);

        // Get all users
        var getAllResponse = await client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var getAllUsersResponse = await getAllResponse.Content.ReadFromJsonAsync<GetAllUsersResponse>();
        Assert.NotNull(getAllUsersResponse);
        Assert.NotNull(getAllUsersResponse.Users);
        Assert.True(getAllUsersResponse.Users.Count >= 2); // May contain users from other tests

        // Verify our users are present
        var user1Email = user1Payload.email;
        var user2Email = user2Payload.email;

        Assert.Contains(getAllUsersResponse.Users, u => u.Email == user1Email && u.FirstName == "Ada" && u.LastName == "Lovelace" && u.Title == "Dr");
        Assert.Contains(getAllUsersResponse.Users, u => u.Email == user2Email && u.FirstName == "Grace" && u.LastName == "Hopper" && u.Title == null);
    }

    [Fact]
    public async Task GetUserByEmail_WithExistingUser_ReturnsUser()
    {
        using var client = _factory.CreateClient();

        // Register a user
        var email = $"test-{Guid.NewGuid():N}@example.com";
        var userPayload = new
        {
            email = email,
            title = "Prof",
            firstName = "Katherine",
            lastName = "Johnson"
        };
        var registerResponse = await client.PostAsJsonAsync("/api/users/register", userPayload);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        // Get user by email
        var getUserResponse = await client.GetAsync($"/api/users/by-email?email={Uri.EscapeDataString(email)}");
        Assert.Equal(HttpStatusCode.OK, getUserResponse.StatusCode);

        var getUserByEmailResponse = await getUserResponse.Content.ReadFromJsonAsync<GetUserByEmailResponse>();
        Assert.NotNull(getUserByEmailResponse);
        Assert.NotNull(getUserByEmailResponse.User);

        var user = getUserByEmailResponse.User!;
        Assert.Equal(email, user.Email);
        Assert.Equal("Prof", user.Title);
        Assert.Equal("Katherine", user.FirstName);
        Assert.Equal("Johnson", user.LastName);
        Assert.False(string.IsNullOrWhiteSpace(user.Id));
        Assert.Equal(UserId.Length, user.Id.Length);
    }

    [Fact]
    public async Task GetUserByEmail_WithNonExistentUser_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var nonExistentEmail = $"nonexistent-{Guid.NewGuid():N}@example.com";
        var response = await client.GetAsync($"/api/users/by-email?email={Uri.EscapeDataString(nonExistentEmail)}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var getUserByEmailResponse = await response.Content.ReadFromJsonAsync<GetUserByEmailResponse>();
        Assert.NotNull(getUserByEmailResponse);
        Assert.Null(getUserByEmailResponse.User);
    }

    [Fact]
    public async Task GetUserByEmail_WithoutEmailParameter_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users/by-email");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserByEmail_WithEmptyEmailParameter_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users/by-email?email=");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserByEmail_WithInvalidEmailFormat_ReturnsBadRequest()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/users/by-email?email=invalid-email-format");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
