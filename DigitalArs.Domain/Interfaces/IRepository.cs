using System.Linq.Expressions; 

namespace DigitalArs.Domain.Interfaces; 

public interface IRepository<T> where T : class 
{
    // Busca una entidad por su clave primaria; null si no existe
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    // Devuelve todas las filas de la tabla de T
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // Devuelve las entidades que cumplen el predicado (filtro LINQ → SQL WHERE)
    //permite incluir relaciones de navegación mediante Include.
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes
    );

    //Consulta entidad aplicando filtro y proyectando el resultado
    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        CancellationToken cancellationToken = default
    );
    
    // Página de resultados (Skip/Take) con total para armar el envelope.
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    // Página proyectada en SQL (Select) para evitar Include/N+1. Orden opcional.
    Task<(IReadOnlyList<TResult> Items, int TotalCount)> GetPagedProjectedAsync<TResult, TOrderKey>(
        int page,
        int pageSize,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, TOrderKey>>? orderBy = null,
        bool descending = false,
        CancellationToken cancellationToken = default);

    // Marca la entidad para insertarse (todavía no hay INSERT hasta SaveChangesAsync)
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Marca la entidad como modificada (todavía no hay UPDATE hasta SaveChangesAsync)
    void Update(T entity);

    // Marca la entidad para eliminarse (todavía no hay DELETE hasta SaveChangesAsync)
    void Delete(T entity);
}
