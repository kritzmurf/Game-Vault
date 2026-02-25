namespace GameVault.Api.Models;

/* Database data model for a game entry
 *
 * fields:
 * Id:              Primary key - used for Postgres to identify
 * Title:           Title of the game
 * Platform:        Hardware Platform the Game was built for (i.e.: PS1)
 * Release Date:    Date Game released
 * Publisher:       Game's Publisher
 * Developer:       Game's Developer
 * Description:     Brief Description of the game.
 * CoverArtUrl:     URL path to Art asset for game.
 * Region:          Region of Release
 *
 */
public class Game
{
    public int Id { get; set;}
    public string Title { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateOnly? ReleaseDate { get; set; }
    public string? Publisher { get; set; }
    public string? Developer { get; set; }
    public string? Description { get; set; }
    public string? CoverArtUrl { get; set; }
    public string Region { get; set; } = string.Empty;
}
