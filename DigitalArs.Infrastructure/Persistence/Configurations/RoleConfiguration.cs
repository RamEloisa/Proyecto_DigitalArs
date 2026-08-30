using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalArs.Infrastructure.Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.ID_Role); //ID_Role es la primary key

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);
            //esto no lo pide, no se si es necesario
            builder.HasIndex(r => r.Name)
                .IsUnique();

            //Role 1:N User
            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.ID_Role)
                .OnDelete(DeleteBehavior.Restrict);

            //SEED
            //ID ADMIN Y USER
            builder.HasData(
                new Role { ID_Role = 1, Name = "Admin" },
                new Role { ID_Role = 2, Name = "User"}

                );
        }
    }
}
