using System.Globalization;
using DigitalArs.Application.DTOs;
using DigitalArs.Domain.Entities;
using DigitalArs.Domain.Enum;

namespace DigitalArs.Application.Realtime;

public static class AccountNotificationFactory
{
    private static readonly CultureInfo Ar = CultureInfo.GetCultureInfo("es-AR");

    public static Notification Create(int userId, TransactionType type, decimal amount)
    {
        var (title, message) = Describe(type, amount);

        return new Notification
        {
            ID_User = userId,
            Type = type.ToString(),
            Title = title,
            Message = message,
            Amount = amount,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static AccountRealtimeEvent ToEvent(Notification notification, decimal balance) =>
        new(
            notification.ID_User,
            balance,
            notification.Type,
            notification.Amount,
            notification.Title,
            notification.Message,
            notification.CreatedAt,
            notification.ID_Notification);

    public static (string Title, string Message) Describe(TransactionType type, decimal amount)
    {
        var formatted = amount.ToString("N2", Ar);

        return type switch
        {
            TransactionType.Deposit => (
                "Depósito acreditado",
                $"Ingresaron ${formatted} a tu cuenta."),
            TransactionType.Transfer_In => (
                "Transferencia recibida",
                $"Recibiste ${formatted}."),
            TransactionType.Transfer_Out => (
                "Transferencia enviada",
                $"Enviaste ${formatted}."),
            TransactionType.FixedTerm_Out => (
                "Plazo fijo constituido",
                $"Se debitaron ${formatted} para un plazo fijo."),
            TransactionType.FixedTerm_In => (
                "Plazo fijo acreditado",
                $"Se acreditaron ${formatted} por vencimiento de un plazo fijo."),
            _ => (
                "Movimiento en tu cuenta",
                $"Hubo un movimiento de ${formatted}.")
        };
    }
}
