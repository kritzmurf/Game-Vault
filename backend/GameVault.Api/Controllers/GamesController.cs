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
    public async Task<IActionResult> GetAll(int page = DefaultPage, int pageSize = DefaultPageSize, string? platform = null)
    {
        if (page < DefaultPage) page = DefaultPage;
        if (pageSize <  MinPageSize|| pageSize > MaxPageSize) pageSize = DefaultPageSize;

        var offset = (page -1) * pageSize;

        using var connection = _dbFactory.CreateConnection();
    
        var where = platform != null
            ? "WHERE category = @Category AND platform = @Platform"
            : "WHERE category = @Category";
        var games = await connection.QueryAsync<Game>(
                $"SELECT * from games {where} ORDER BY title LIMIT @PageSize OFFSET @Offset",
                new { PageSize = pageSize, Offset = offset, Platform = platform, Category = GameCategory.MainGame }
                );

        var totalCount = await connection.ExecuteScalarAsync<int>(
            $"SELECT COUNT(*) FROM games {where}",
            new { Platform = platform, Category = GameCategory.MainGame }
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

    [HttpGet("platforms")]
    public async Task<IActionResult> GetPlatforms()
    {
        using var connection = _dbFactory.CreateConnection();
        var platforms = await connection.QueryAsync<dynamic>(
            "SELECT platform, COUNT(*) as game_count FROM games WHERE category = @Category GROUP BY platform ORDER BY platform",
            new { Category = GameCategory.MainGame }
        );
        return Ok(platforms);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string q = "", int page = DefaultPage, int pageSize = DefaultPageSize)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(new PaginatedResponse<Game> { Items = [], TotalCount = 0, Page = page, PageSize = pageSize });
        if (page < DefaultPage) page = DefaultPage;
        if (pageSize < MinPageSize || pageSize > MaxPageSize) pageSize = DefaultPageSize;
        
        var offset = (page - 1) * pageSize;

        using var connection = _dbFactory.CreateConnection();
        var games = await connection.QueryAsync<Game>(
            "SELECT * FROM games WHERE similarity(title, @Search) > 0.3 AND category = @Category ORDER BY similarity(title, @Search) DESC LIMIT @PageSize OFFSET @Offset",
            new { Search = q, PageSize = pageSize, Offset = offset, Category = GameCategory.MainGame }
        );

        var totalCount = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM games WHERE similarity(title, @Search) > 0.3 AND category = @Category",
            new { Search = q, Category = GameCategory.MainGame }
        );

        return Ok(new PaginatedResponse<Game>
        {
            Items = games,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
