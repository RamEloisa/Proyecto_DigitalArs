using System;
using System.Collections.Generic;
using System.Text;
using DigitalArs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DigitalArs.Infrastructure.Security;

namespace DigitalArs.Infrastructure.Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            var hasher = new BCryptPasswordHasher();

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
            builder.HasIndex(u => u.Alias)
                .IsUnique();

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

            //SEED USER
            builder.HasData(
                new User
                {
                    ID_User = 1,
                    Full_Name = "Admin Principal",
                    Email = "admin@digitalars.com",
                    //Admin123! en bcrypt
                    Password_Hasheada = "$2a$11$Lcc/.JSUqr0pWtLG9/SIwe0R3J7uk.QwAiJ0i4Vws3odRsPRa28uy",
                    DNI = "30111222",
                    Alias = "admin.digitalars",
                    ID_Role = 1 //Admin
                },
                new User
                {
                    ID_User = 2,
                    Full_Name = "Juan Perez",
                    Email = "juan.perez@digitalars.com",
                    //User123! en bcrypt
                    Password_Hasheada = "$2a$11$4/qtspjvQA4wq8df1/6oq..O.REndayYq/MGD5W8I0uQOtpO21icW",
                    DNI = "35222333",
                    Alias = "juan.perez",
                    ID_Role = 2 //User
                },
                new User
                {
                    ID_User = 3,
                    Full_Name = "Maria Gomez",
                    Email = "maria.gomez@digitalars.com",
                    //User123! en bcrypt
                    Password_Hasheada = "$2a$11$4/qtspjvQA4wq8df1/6oq..O.REndayYq/MGD5W8I0uQOtpO21icW",
                    DNI = "36333444",
                    Alias = "maria.gomez",
                    ID_Role = 2 //User
                }
            );
        }
    }
}
