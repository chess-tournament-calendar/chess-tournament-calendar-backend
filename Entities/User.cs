using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.Entities;


[Table("users")]

public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column("name")]
    [MaxLength(32)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public UserRole Role { get; set; } = UserRole.User;
        
    [Required]
    [Column("title")]
    public ChessTitle Title { get; set; } = ChessTitle.None;
        
    [Column("refresh_token")]
    [MaxLength(255)]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry_time")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
        
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}