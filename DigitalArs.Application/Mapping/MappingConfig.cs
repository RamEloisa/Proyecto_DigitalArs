using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using Mapster;

namespace DigitalArs.Application.Mapping;

/// Mapeos entidad ↔ DTO. UserDto no incluye Password_Hasheada a propósito.
public sealed class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        EnsureResponseDtosDoNotExposePassword();

        config.NewConfig<User, UserDto>()
            .Map(dest => dest.Id, src => src.ID_User)
            .Map(dest => dest.FullName, src => src.Full_Name)
            .Map(dest => dest.Dni, src => src.DNI)
            .Map(dest => dest.RoleId, src => src.ID_Role)
            .Map(dest => dest.IsActive, src => src.IsActive)
            .Map(dest => dest.AccountId, src => src.Account != null ? src.Account.ID_Account : (int?)null);

        config.NewConfig<User, UserLookupDto>()
            .Map(dest => dest.Id, src => src.ID_User)
            .Map(dest => dest.FullName, src => src.Full_Name)
            .Map(dest => dest.Alias, src => src.Alias)
            .Map(dest => dest.AccountId, src => src.Account != null ? src.Account.ID_Account : (int?)null);

        config.NewConfig<CreateUserDto, User>()
            .Map(dest => dest.Full_Name, src => src.FullName)
            .Map(dest => dest.DNI, src => src.Dni)
            .Map(dest => dest.ID_Role, src => src.RoleId)
            .Ignore(dest => dest.ID_User)
            .Ignore(dest => dest.Password_Hasheada)
            .Ignore(dest => dest.IsActive)
            .Ignore(dest => dest.Role)
            .Ignore(dest => dest.Account);

        config.NewConfig<UpdateUserDto, User>()
            .Map(dest => dest.Full_Name, src => src.FullName)
            .Map(dest => dest.DNI, src => src.Dni)
            .Map(dest => dest.ID_Role, src => src.RoleId)
            .Ignore(dest => dest.ID_User)
            .Ignore(dest => dest.Password_Hasheada)
            .Ignore(dest => dest.Role)
            .Ignore(dest => dest.Account);

        config.NewConfig<Role, RoleDto>()
            .Map(dest => dest.Id, src => src.ID_Role);

        config.NewConfig<CreateRoleDto, Role>()
            .Ignore(dest => dest.ID_Role)
            .Ignore(dest => dest.Users);

        config.NewConfig<UpdateRoleDto, Role>()
            .Ignore(dest => dest.ID_Role)
            .Ignore(dest => dest.Users);

        config.NewConfig<Account, AccountDto>()
            .Map(dest => dest.Id, src => src.ID_Account)
            .Map(dest => dest.UserId, src => src.ID_User);

        config.NewConfig<CreateAccountDto, Account>()
            .Map(dest => dest.ID_User, src => src.UserId)
            .Map(dest => dest.Date, _ => DateTime.UtcNow)
            .Ignore(dest => dest.ID_Account)
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.Transactions);

        config.NewConfig<UpdateAccountDto, Account>()
            .Ignore(dest => dest.ID_Account)
            .Ignore(dest => dest.ID_User)
            .Ignore(dest => dest.Date)
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.Transactions);

        config.NewConfig<Transaction, TransactionDto>()
            .Map(dest => dest.Id, src => src.ID_Transaction)
            .Map(dest => dest.AccountId, src => src.ID_Account)
            .Map(dest => dest.Date, src => src.Date_Transaction);

        config.NewConfig<CreateTransactionDto, Transaction>()
            .Map(dest => dest.ID_Account, src => src.AccountId)
            .Map(dest => dest.Date_Transaction, _ => DateTime.UtcNow)
            .Ignore(dest => dest.ID_Transaction)
            .Ignore(dest => dest.Account);

        config.NewConfig<UpdateMeDto, User>()
            .Map(dest => dest.Full_Name, src => src.FullName)
            .Map(dest => dest.DNI, src => src.Dni)
            .Ignore(dest => dest.ID_User)
            .Ignore(dest => dest.Password_Hasheada)
            .Ignore(dest => dest.ID_Role)
            .Ignore(dest => dest.Role)
            .Ignore(dest => dest.Account);
    }

    private static void EnsureResponseDtosDoNotExposePassword()
    {
        var leaked = new[] { typeof(UserDto), typeof(UserLookupDto) }
            .SelectMany(t => t.GetProperties())
            .Where(p => p.Name.Contains("Password", StringComparison.OrdinalIgnoreCase)
                        || p.Name.Contains("Hash", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Name)
            .ToList();

        if (leaked.Count > 0)
        {
            throw new InvalidOperationException(
                "Los DTOs de usuario no deben exponer el hash de la contraseña: " + string.Join(", ", leaked));
        }
    }
}
