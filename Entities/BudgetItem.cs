using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.Entities;

[Table("budget_items")]
public class BudgetItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
        
    [Required]
    [Column("budget_id")]
    public Guid BudgetId { get; set; }
        
    [Required]
    [Column("category")]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [Column("currency")]
    [MaxLength(3)]
    public CurrencyType Currency { get; set; } = CurrencyType.TRY;

    [ForeignKey("BudgetId")]
    public Budget? Budget { get; set; }
}