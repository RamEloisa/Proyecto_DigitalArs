using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations;

public class FixedTermDepositConfiguration : IEntityTypeConfiguration<FixedTermDeposit>
{
    public void Configure(EntityTypeBuilder<FixedTermDeposit> builder)
    {
        builder.HasKey(d => d.ID_FixedTermDeposit);

        builder.Property(d => d.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.AnnualRate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(d => d.TermDays)
            .IsRequired();

        builder.Property(d => d.InterestAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.FinalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.MaturityDate)
            .IsRequired();

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(d => d.Account)
            .WithMany(a => a.FixedTermDeposits)
            .HasForeignKey(d => d.ID_Account)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.ID_Account, d.Status, d.MaturityDate });
    }
}
