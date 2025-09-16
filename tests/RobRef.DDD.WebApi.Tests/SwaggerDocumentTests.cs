using System.Linq;
using System.Net;
using System.Text.Json;

namespace RobRef.DDD.WebApi.Tests;

public sealed class SwaggerDocumentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SwaggerDocumentTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SwaggerDocument_MatchesSnapshot()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actual = await response.Content.ReadAsStringAsync();

        var snapshotPath = Path.Combine(AppContext.BaseDirectory, "__snapshots__", "Swagger.v1.json");
        Assert.True(File.Exists(snapshotPath), $"Snapshot file not found at '{snapshotPath}'.");

        var expected = await File.ReadAllTextAsync(snapshotPath);

        using var actualDocument = JsonDocument.Parse(actual);
        using var expectedDocument = JsonDocument.Parse(expected);

        AssertJsonSubset(expectedDocument.RootElement, actualDocument.RootElement);
    }

    private static void AssertJsonSubset(JsonElement expected, JsonElement actual)
    {
        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in expected.EnumerateObject())
                {
                    Assert.True(actual.TryGetProperty(property.Name, out var actualProperty),
                        $"Expected property '{property.Name}' was not found in the actual Swagger document.");
                    AssertJsonSubset(property.Value, actualProperty);
                }
                break;
            case JsonValueKind.Array:
                var expectedArray = expected.EnumerateArray().ToArray();
                var actualArray = actual.EnumerateArray().ToArray();
                Assert.True(actualArray.Length >= expectedArray.Length,
                    "Actual array contained fewer elements than expected.");
                for (var i = 0; i < expectedArray.Length; i++)
                {
                    AssertJsonSubset(expectedArray[i], actualArray[i]);
                }
                break;
            case JsonValueKind.String:
                Assert.Equal(expected.GetString(), actual.GetString());
                break;
            case JsonValueKind.Number:
                Assert.Equal(expected.GetDecimal(), actual.GetDecimal());
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                Assert.Equal(expected.GetBoolean(), actual.GetBoolean());
                break;
            case JsonValueKind.Null:
                Assert.Equal(JsonValueKind.Null, actual.ValueKind);
                break;
            default:
                Assert.Equal(expected.ToString(), actual.ToString());
                break;
        }
    }
}
