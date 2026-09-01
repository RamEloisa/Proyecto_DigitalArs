using DigitalArs.Application.Services; // Contratos e implementaciones de Application
using Microsoft.Extensions.DependencyInjection; // IServiceCollection.AddScoped
using DigitalArs.Application.Security;

namespace DigitalArs.Application;

/// <summary>
/// Registra servicios de aplicación. Todos dependen de IUnitOfWork, nunca del DbContext.
/// </summary>
public static class DependencyInjection
{
    // Lo llama Program.cs: builder.Services.AddApplication();
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>(); // Un servicio por request (Scoped)
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAuthService, AuthService>();
        return services; // Permite encadenar AddInfrastructure en Program.cs
    }
}
