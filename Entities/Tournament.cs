using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.Entities;

[Table("tournaments")]
public class Tournament
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateTime EndDate { get; set; }

    [Column("is_planning_to_play")]
    public bool IsPlanningToPlay { get; set; }

    [Column("rounds")]
    public int Rounds { get; set; }

    [Required]
    [Column("type")]
    public TournamentType Type { get; set; }

    [Column("country")]
    [MaxLength(100)]
    public string? Country { get; set; }

    [Column("is_confirmed")]
    public bool IsConfirmed { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Budget? Budget { get; set; }
}