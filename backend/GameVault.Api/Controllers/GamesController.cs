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

    public GamesController(DbConnectionFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        using var connection = _dbFactory.CreateConnection();
        var games = await connection.QueryAsync<Game>(
                "SELECT * from games"
                );
        return Ok(games);
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
