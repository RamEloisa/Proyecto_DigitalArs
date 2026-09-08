using DigitalArs.Domain.Enum;

namespace DigitalArs.Domain.Entities;

public class FixedTermDeposit
{
    public int ID_FixedTermDeposit { get; set; }

    public int ID_Account { get; set; }
    public Account Account { get; set; }

    public decimal Amount { get; set; }
    public decimal AnnualRate { get; set; }
    public int TermDays { get; set; }
    public decimal InterestAmount { get; set; }
    public decimal FinalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime MaturityDate { get; set; }

    public FixedTermDepositStatus Status { get; set; } = FixedTermDepositStatus.Active;
    public DateTime? SettledAt { get; set; }
}
