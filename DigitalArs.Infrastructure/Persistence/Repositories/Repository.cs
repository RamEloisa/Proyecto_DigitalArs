using System.Linq.Expressions; 
using DigitalArs.Domain.Interfaces; 
using Microsoft.EntityFrameworkCore; 

namespace DigitalArs.Infrastructure.Persistence.Repositories; 

internal class Repository<T> : IRepository<T> where T : class 
{
    private readonly DbSet<T> _dbSet; 

    // El contexto lo inyecta el UnitOfWork (mismo Scoped por request HTTP)
    public Repository(DigitalArsDbContext context)
    {
        _dbSet = context.Set<T>(); 
    }

    // FindAsync de EF busca por clave primaria y usa el cache del change tracker
    public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken); 
    }

    // AsNoTracking: lectura, no hace falta rastrear cambios para un UPDATE posterior
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken); 
    }

    // Where(predicate) se traduce a SQL (WHERE ...); queda tracked por si después se hace Update
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken); 
    }

    // AddAsync solo agrega al change tracker; no hace INSERT hasta SaveChangesAsync
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken); 
    }

    // Update marca la entidad como Modified (UPDATE al guardar)
    public void Update(T entity)
    {
        _dbSet.Update(entity); 
    }

    // Remove marca la entidad como Deleted (DELETE al guardar)
    public void Delete(T entity)
    {
        _dbSet.Remove(entity); 
    }
}
