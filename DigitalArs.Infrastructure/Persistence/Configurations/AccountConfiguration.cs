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
        }
    }
}
