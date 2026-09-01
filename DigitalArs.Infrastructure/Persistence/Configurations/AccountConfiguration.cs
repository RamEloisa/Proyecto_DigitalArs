using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.ID_Account); //ID_Account es la primary key

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            //Indice 
            builder.HasIndex(a => a.ID_User);

            //User 1:1 Account
            builder.HasOne(a => a.User)
                .WithOne(u => u.Account)
                .HasForeignKey<Account>(a => a.ID_User)
                .OnDelete(DeleteBehavior.Restrict);

            //Account !:N Transaction
            builder.HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.ID_Account)
                .OnDelete(DeleteBehavior.Restrict);

            //SEED CUENTAS
            builder.HasData(
                //SUFIJO m o M para indicar q es decimal en precio
                new Account { ID_Account = 1, ID_User = 1, Name = "Cuenta Admin", Price = 10000m, Date = new DateTime(2026, 1, 1) },
                new Account { ID_Account = 2, ID_User = 2, Name = "Cuenta Juan", Price = 5000m, Date = new DateTime(2026, 1, 1) },
                new Account { ID_Account = 3, ID_User = 3, Name = "Cuenta Maria", Price = 7500m, Date = new DateTime(2026, 1, 1) }
            );
        }
    }
}
