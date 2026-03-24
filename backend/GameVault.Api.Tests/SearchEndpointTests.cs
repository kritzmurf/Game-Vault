using System.Net;
using System.Net.Http.Json;
using GameVault.Api.Models;
using GameVault.Api.Tests.Fixtures;
using GameVault.Api.Tests.Helpers;

namespace GameVault.Api.Tests;

[Collection("Database")]
public class SearchEndpointTests : IDisposable
{
    private readonly GameVaultWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public SearchEndpointTests(SharedDatabaseFixture fixture)
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
    public async Task Search_MatchingQuery_ReturnsResults()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=Final Fantasy");

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, g =>
            Assert.Contains("Final Fantasy", g.Title, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Search_FuzzyMatch_FindsMisspelledTitle()
    {
        // Trigram similarity should still match despite typos
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=Fnal Fantsy");

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task Search_NoResults_ReturnsEmptyPaginatedResponse()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=xyznonexistent12345");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task Search_EmptyQuery_ReturnsEmptyImmediately()
    {
        var empty = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=");
        var whitespace = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=%20%20%20");

        Assert.NotNull(empty);
        Assert.NotNull(whitespace);
        Assert.Empty(empty.Items);
        Assert.Empty(whitespace.Items);
    }

    [Fact]
    public async Task Search_Pagination_Works()
    {
        var page1 = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=Final Fantasy&pageSize=2&page=1");
        var page2 = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=Final Fantasy&pageSize=2&page=2");

        Assert.NotNull(page1);
        Assert.NotNull(page2);
        Assert.Equal(2, page1.Items.Count());
        Assert.NotEmpty(page2.Items);

        // Pages should contain different games
        var page1Ids = page1.Items.Select(g => g.Id).ToHashSet();
        var page2Ids = page2.Items.Select(g => g.Id).ToHashSet();
        Assert.Empty(page1Ids.Intersect(page2Ids));
    }

    [Fact]
    public async Task Search_ExcludesNonMainGameCategory()
    {
        // "FF VII: Advent Children DLC" is category 1 and contains "FF"
        // Search for "FF" should not return it
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=FF&pageSize=100");

        Assert.NotNull(result);
        Assert.All(result.Items, g => Assert.Equal(GameCategory.MainGame, g.Category));
    }

    [Fact]
    public async Task Search_ResultsOrderedBySimilarity()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>(
            "/api/games/search?q=Final Fantasy VII");

        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        // The first result should be the closest match
        Assert.Equal("Final Fantasy VII", result.Items.First().Title);
    }
}
