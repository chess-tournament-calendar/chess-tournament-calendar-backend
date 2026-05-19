using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChessTournamentCalendarBackend.API.Enums;

namespace ChessTournamentCalendarBackend.API.Entities;

[Table("budgets")]
public class Budget
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
        
    [Required]
    [Column("tournament_id")]
    public Guid TournamentId { get; set; }
        
    [Required]
    [Column("plan_type")]
    public BudgetPlanType PlanType { get; set; }

    [Required]
    [Column("total_amount", TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column("currency")]
    [MaxLength(3)]
    public CurrencyType Currency { get; set; } = CurrencyType.TRY;

    [ForeignKey("TournamentId")]
    public Tournament? Tournament { get; set; }
        
    public ICollection<BudgetItem> Items { get; set; } = new List<BudgetItem>();
}