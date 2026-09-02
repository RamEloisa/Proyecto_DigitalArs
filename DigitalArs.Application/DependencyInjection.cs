using DigitalArs.Application.Mapping;
using DigitalArs.Application.Security;
using DigitalArs.Application.Services;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<IUserMeService, UserMeService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAuthService, AuthService>();
        return services; // Permite encadenar AddInfrastructure en Program.cs
    }
}
