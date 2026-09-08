using DigitalArs.Domain.Interfaces; 
using DigitalArs.Infrastructure.Persistence.Repositories; 
using Microsoft.EntityFrameworkCore.Storage; 

namespace DigitalArs.Infrastructure.Persistence; 

public class UnitOfWork : IUnitOfWork
{
    private readonly DigitalArsDbContext _context; 
    private IDbContextTransaction? _transaction; 
    private readonly Dictionary<Type, object> _repositories = new(); 

    public UnitOfWork(DigitalArsDbContext context)
    {
        _context = context; 
    }

    // Devuelve el mismo repositorio si ya se pidió T en este request
    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T); // Clave del diccionario (ej. typeof(User))

        if (!_repositories.TryGetValue(type, out var repository)) 
        {
            repository = new Repository<T>(_context); 
            _repositories[type] = repository; 
        }

        return (IRepository<T>)repository; 
    }

    // INSERT/UPDATE/DELETE pendientes → SQL (sin transacción explícita, cada save es su propio commit)
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken); // Devuelve cuántas filas se afectaron
    }

    // BEGIN TRANSACTION en SQL Server
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) 
        {
            throw new InvalidOperationException("Ya hay una transacción activa."); 
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken); 
    }

    // COMMIT: primero persiste, después confirma la transacción
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken); 

            if (_transaction is not null) 
            {
                await _transaction.CommitAsync(cancellationToken); 
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken); 
            throw; 
        }
        finally
        {
            await DisposeTransactionAsync(); 
        }
    }

    // ROLLBACK: deshace todo lo de la transacción actual
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) 
        {
            await _transaction.RollbackAsync(cancellationToken); 
            await DisposeTransactionAsync(); 
        }
    }

    // Cierra y limpia la transacción explícita
    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync(); 
            _transaction = null; 
        }
    }

    // El contenedor Scoped llama esto al terminar el HTTP request
    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync(); 
        GC.SuppressFinalize(this); 
    }
}
