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
            builder.HasKey(t => t.ID_Transaction); //ID_Transaction es la primary keynew T

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);

            //Indice
            builder.HasIndex(t => t.Date_Transaction);
                
            //Account 1:N Transaction
            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.ID_Account)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasData(
                //Deposito a la cuenta de Juan
                new Transaction
                {
                    ID_Transaction = 1,
                    ID_Account = 2,
                    //es correcto llamar de esta forma?
                    Type = Domain.Enum.TransactionType.Deposit,
                    Amount = 2000m,
                    Date_Transaction = new DateTime(2026, 1, 2)
                },
                //Transferencia desde Admin (id 1) a Maria (id 3)
                new Transaction
                {
                    ID_Transaction = 2,
                    ID_Account = 1,
                    Type = Domain.Enum.TransactionType.Transfer_Out,
                    Amount = 1500m,
                    Date_Transaction = new DateTime(2026, 1, 3)
                },
                new Transaction
                {
                    ID_Transaction = 3,
                    ID_Account = 3,
                    Type = Domain.Enum.TransactionType.Transfer_In,
                    Amount = 1500m,
                    Date_Transaction = new DateTime(2026, 1, 3)
                }
                );
        }
    }
}
