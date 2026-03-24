using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using GameVault.Api.Tests.Fixtures;
using GameVault.Api.Tests.Helpers;

namespace GameVault.Api.Tests;

// Platform response shape from the dynamic query
// The controller returns snake_case keys from the SQL query
public class PlatformResult
{
    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("game_count")]
    public int GameCount { get; set; }
}

[Collection("Database")]
public class PlatformsEndpointTests : IDisposable
{
    private readonly GameVaultWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlatformsEndpointTests(SharedDatabaseFixture fixture)
    {
        TestDataHelper.TruncateGames(fixture.ConnectionString);
        TestDataHelper.SeedStandardGames(fixture.ConnectionString);
        _factory = new GameVaultWebApplicationFactory(fixture.ConnectionString);
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetPlatforms_ReturnsAllPlatformsWithCounts()
    {
        var response = await _client.GetAsync("/api/games/platforms");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var platforms = await response.Content.ReadFromJsonAsync<List<PlatformResult>>();
        Assert.NotNull(platforms);
        Assert.Equal(3, platforms.Count); // PS1, PS2, SNES

        var ps1 = platforms.First(p => p.Platform == "PS1");
        var ps2 = platforms.First(p => p.Platform == "PS2");
        var snes = platforms.First(p => p.Platform == "SNES");

        Assert.Equal(10, ps1.GameCount);
        Assert.Equal(5, ps2.GameCount);
        Assert.Equal(10, snes.GameCount);
    }

    [Fact]
    public async Task GetPlatforms_CountsExcludeNonMainGameCategory()
    {
        // PS1 has 10 main games + 1 DLC in seed data
        // PS2 has 5 main games + 1 expansion in seed data
        // Counts should only reflect main games
        var platforms = await _client.GetFromJsonAsync<List<PlatformResult>>("/api/games/platforms");

        Assert.NotNull(platforms);
        var ps1 = platforms.First(p => p.Platform == "PS1");
        var ps2 = platforms.First(p => p.Platform == "PS2");

        Assert.Equal(10, ps1.GameCount); // Not 11
        Assert.Equal(5, ps2.GameCount);  // Not 6
    }

    [Fact]
    public async Task GetPlatforms_ResultsAreOrderedAlphabetically()
    {
        var platforms = await _client.GetFromJsonAsync<List<PlatformResult>>("/api/games/platforms");

        Assert.NotNull(platforms);
        var names = platforms.Select(p => p.Platform).ToList();
        var sorted = names.OrderBy(n => n).ToList();

        Assert.Equal(sorted, names);
    }
}
