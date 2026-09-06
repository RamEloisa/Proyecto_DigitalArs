namespace DigitalArs.Application.Options;

public sealed class FixedTermDepositSettings
{
    public const string SectionName = "FixedTermDeposit";

    public decimal AnnualRate { get; set; } = 0.30m;
    public decimal MinAmount { get; set; } = 1000m;
    public int MinTermDays { get; set; } = 30;
}
