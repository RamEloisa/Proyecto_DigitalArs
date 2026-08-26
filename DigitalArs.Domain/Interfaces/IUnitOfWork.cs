namespace DigitalArs.Domain.Interfaces; 

public interface IUnitOfWork : IAsyncDisposable 
{
    // Obtiene (o reutiliza) el repositorio genérico de la entidad T
    IRepository<T> Repository<T>() where T : class;

    // Persiste en la base todos los cambios pendientes de los repositorios
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    // Abre una transacción SQL explícita (BEGIN TRAN)
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    // Confirma la transacción (COMMIT) después de guardar
    Task CommitAsync(CancellationToken cancellationToken = default);

    // Deshace la transacción (ROLLBACK) si algo falló
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
