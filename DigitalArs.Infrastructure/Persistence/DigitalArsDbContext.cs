using DigitalArs.Domain.Entities; // User, Role, Account, Transaction (modelo de dominio)
using Microsoft.EntityFrameworkCore; // DbContext, DbSet y ModelBuilder de EF Core

namespace DigitalArs.Infrastructure.Persistence; // Infraestructura: SQL Server / EF, no Domain

/// <summary>
/// Único DbContext de la app. Los servicios no lo inyectan: usan IUnitOfWork.
/// Sin migraciones todavía: el modelo queda en código hasta que se generen.
/// </summary>
public class DigitalArsDbContext : DbContext // Hereda el contexto de EF Core
{
    // Recibe las opciones (cadena, proveedor SQL Server) que registra el DI como Scoped
    public DigitalArsDbContext(DbContextOptions<DigitalArsDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } // Tabla Users (el repositorio genérico también usa Set<T>())
    public DbSet<Role> Roles { get; set; } // Tabla Roles
    public DbSet<Account> Accounts { get; set; } // Tabla Accounts
    public DbSet<Transaction> Transactions { get; set; } // Tabla Transactions

    // Fluent API: claves, índices y relaciones viven en Persistence/Configurations
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Aplica la configuración por defecto de EF
        // Carga User/Role/Account/TransactionConfiguration de este ensamblado (sin migraciones aún)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DigitalArsDbContext).Assembly);
    }
}
