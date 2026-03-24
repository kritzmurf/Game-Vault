using System.Net;
using System.Net.Http.Json;
using GameVault.Api.Models;
using GameVault.Api.Tests.Fixtures;
using GameVault.Api.Tests.Helpers;

namespace GameVault.Api.Tests;

[Collection("Database")]
public class GameByIdEndpointTests : IDisposable
{
    private readonly GameVaultWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GameByIdEndpointTests(SharedDatabaseFixture fixture)
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
    public async Task GetById_ValidId_ReturnsGame()
    {
        // First, get a known game ID from the list
        var list = await _client.GetFromJsonAsync<PaginatedResponse<Game>>("/api/games?pageSize=1");
        Assert.NotNull(list);
        Assert.NotEmpty(list.Items);

        var knownId = list.Items.First().Id;
        var expectedTitle = list.Items.First().Title;

        var response = await _client.GetAsync($"/api/games/{knownId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var game = await response.Content.ReadFromJsonAsync<Game>();
        Assert.NotNull(game);
        Assert.Equal(knownId, game.Id);
        Assert.Equal(expectedTitle, game.Title);
    }

    [Fact]
    public async Task GetById_NonexistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/games/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
