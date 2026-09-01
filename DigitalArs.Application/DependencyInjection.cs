<<<<<<< HEAD
using DigitalArs.Application.Mapping;
using DigitalArs.Application.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
=======
using DigitalArs.Application.Services; // Contratos e implementaciones de Application
using Microsoft.Extensions.DependencyInjection; // IServiceCollection.AddScoped
using DigitalArs.Application.Security;
>>>>>>> 9a6eacc93c41109c51c00bf4167e649005a98b31

namespace DigitalArs.Application;

/// Registra servicios de aplicación, validadores y el mapeo centralizado (Mapster).
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(typeof(MappingConfig).Assembly);
        services.AddSingleton(mappingConfig);
        services.AddScoped<IMapper, Mapper>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAuthService, AuthService>();
        return services; // Permite encadenar AddInfrastructure en Program.cs
    }
}
