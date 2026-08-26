using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.ID_User); //ID_user es la primary key

            builder.Property(u => u.Full_Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email) 
                .IsRequired()
                .HasMaxLength(150);
            //indice User.Email
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Password_Hasheada)
                .IsRequired();

            builder.Property(u => u.DNI)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.Alias)
                .IsRequired()
                .HasMaxLength(50);

            //Role 1:N User
            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.ID_Role)
                //esto evita el ondelete cascade
                .OnDelete(DeleteBehavior.Restrict);

            //User 1:1 Account
            builder.HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.ID_User)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
