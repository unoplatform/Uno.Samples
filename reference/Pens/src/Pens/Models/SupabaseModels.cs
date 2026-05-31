using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Pens.Models;

/// <summary>
/// Database model for players table
/// </summary>
[Table("players")]
public class DbPlayer : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("number")]
    public int Number { get; set; }

    [Column("position")]
    public string Position { get; set; } = string.Empty;

    [Column("is_injured")]
    public bool IsInjured { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Database model for attendance table
/// </summary>
[Table("attendance")]
public class DbAttendance : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int PlayerId { get; set; }

    [Column("week_of")]
    public DateTime WeekOf { get; set; }

    [Column("status")]
    public string Status { get; set; } = "pending";

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Database model for chat_messages table
/// </summary>
[Table("chat_messages")]
public class DbChatMessage : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("player_id")]
    public int? PlayerId { get; set; }

    [Column("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Database model for beer_tracker table
/// </summary>
[Table("beer_tracker")]
public class DbBeerTracker : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("season")]
    public string Season { get; set; } = string.Empty;

    [Column("consumed_cases")]
    public int ConsumedCases { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Database model for games table
/// </summary>
[Table("games")]
public class DbGame : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("opponent")]
    public string Opponent { get; set; } = string.Empty;

    [Column("game_date")]
    public DateTime GameDate { get; set; }

    [Column("game_time")]
    public string GameTime { get; set; } = string.Empty;

    [Column("rink")]
    public string Rink { get; set; } = string.Empty;

    [Column("home_score")]
    public int? HomeScore { get; set; }

    [Column("away_score")]
    public int? AwayScore { get; set; }

    [Column("is_home")]
    public bool IsHome { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Database model for duties table
/// </summary>
[Table("duties")]
public class DbDuty : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("game_id")]
    public int GameId { get; set; }

    [Column("player_id")]
    public int? PlayerId { get; set; }

    [Column("duty_type")]
    public string DutyType { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
