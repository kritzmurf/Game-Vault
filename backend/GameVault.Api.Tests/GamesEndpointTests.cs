using System.Net;
using System.Net.Http.Json;
using GameVault.Api.Models;
using GameVault.Api.Tests.Fixtures;
using GameVault.Api.Tests.Helpers;

namespace GameVault.Api.Tests;

[Collection("Database")]
public class GamesEndpointTests : IDisposable
{
    private readonly GameVaultWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GamesEndpointTests(SharedDatabaseFixture fixture)
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
    public async Task GetAll_DefaultPagination_ReturnsFirst20Games()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games");

        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(20, result.Items.Count());
        Assert.Equal(25, result.TotalCount); // 25 main games, excludes 2 non-main
    }

    [Fact]
    public async Task GetAll_CustomPageAndPageSize_ReturnsCorrectOffset()
    {
        var page1 = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?page=1&pageSize=5");
        var page2 = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?page=2&pageSize=5");

        Assert.NotNull(page1);
        Assert.NotNull(page2);
        Assert.Equal(5, page1.Items.Count());
        Assert.Equal(5, page2.Items.Count());
        Assert.Equal(2, page2.Page);

        // Pages should contain different games
        var page1Ids = page1.Items.Select(g => g.Id).ToHashSet();
        var page2Ids = page2.Items.Select(g => g.Id).ToHashSet();
        Assert.Empty(page1Ids.Intersect(page2Ids));
    }

    [Fact]
    public async Task GetAll_PlatformFilter_ReturnsOnlyMatchingPlatform()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?platform=PS1");

        Assert.NotNull(result);
        Assert.All(result.Items, g => Assert.Equal("PS1", g.Platform));
        Assert.Equal(10, result.TotalCount); // 10 PS1 main games
    }

    [Fact]
    public async Task GetAll_UnknownPlatform_ReturnsEmpty()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?platform=Atari2600");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetAll_ExcludesNonMainGameCategory()
    {
        // Fetch all pages to check every game
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?pageSize=100");

        Assert.NotNull(result);
        Assert.Equal(25, result.TotalCount);
        Assert.All(result.Items, g => Assert.Equal(GameCategory.MainGame, g.Category));
    }

    [Fact]
    public async Task GetAll_PageBelowMinimum_DefaultsToPage1()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?page=0");

        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
    }

    [Fact]
    public async Task GetAll_PageSizeOutOfBounds_DefaultsTo20()
    {
        var tooSmall = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?pageSize=0");
        var tooLarge = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?pageSize=101");

        Assert.NotNull(tooSmall);
        Assert.NotNull(tooLarge);
        Assert.Equal(20, tooSmall.PageSize);
        Assert.Equal(20, tooLarge.PageSize);
    }

    [Fact]
    public async Task GetAll_PageBeyondResults_ReturnsEmptyItems()
    {
        var result = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?page=9999");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(25, result.TotalCount); // TotalCount still reflects real count
    }
}
