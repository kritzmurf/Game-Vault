using Microsoft.AspNetCore.Mvc;
using Dapper;

using GameVault.Api.Data;
using GameVault.Api.Models;

namespace GameVault.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly DbConnectionFactory _dbFactory;
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;

    public GamesController(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = DefaultPage, int pageSize = DefaultPageSize)
    {
        if (page < DefaultPage) page = DefaultPage;
        if (pageSize <  MinPageSize|| pageSize > MaxPageSize) pageSize = DefaultPageSize;

        var offset = (page -1) * pageSize;

        using var connection = _dbFactory.CreateConnection();
        var games = await connection.QueryAsync<Game>(
                "SELECT * from games ORDER BY id LIMIT @PageSize OFFSET @Offset",
                new { PageSize = pageSize, Offset = offset }
                );

        var totalCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM games"
            );

        return Ok(new PaginatedResponse<Game>
            {
                Items = games,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var connection = _dbFactory.CreateConnection();
        var game = await connection.QuerySingleOrDefaultAsync<Game>(
            "SELECT * FROM games WHERE id = @Id",
            new { Id = id }
        );
        if (game is null) { return NotFound();}

        return Ok(game);
    }
}
