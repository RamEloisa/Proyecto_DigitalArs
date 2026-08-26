using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.ID_Transaction); //ID_Transaction es la primary key

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);

            //Indice
            builder.HasIndex(t => t.Date_Transaction);
                
            //Account 1:N Transaction
            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.ID_Account)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
