using DigitalArs.Domain.Interfaces; 
using DigitalArs.Infrastructure.Persistence; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 

namespace DigitalArs.Infrastructure; 

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection.");

        // Scoped por defecto: misma instancia de DbContext en todo el request
        services.AddDbContext<DigitalArsDbContext>(options =>
        {
            options.UseSqlServer(connectionString); // Proveedor SQL Server
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>(); 

        return services; 
    }
}
