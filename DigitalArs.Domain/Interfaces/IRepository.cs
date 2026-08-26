using System.Linq.Expressions; 

namespace DigitalArs.Domain.Interfaces; 

public interface IRepository<T> where T : class 
{
    // Busca una entidad por su clave primaria; null si no existe
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    // Devuelve todas las filas de la tabla de T
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // Devuelve las entidades que cumplen el predicado (filtro LINQ → SQL WHERE)
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    // Marca la entidad para insertarse (todavía no hay INSERT hasta SaveChangesAsync)
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Marca la entidad como modificada (todavía no hay UPDATE hasta SaveChangesAsync)
    void Update(T entity);

    // Marca la entidad para eliminarse (todavía no hay DELETE hasta SaveChangesAsync)
    void Delete(T entity);
}
