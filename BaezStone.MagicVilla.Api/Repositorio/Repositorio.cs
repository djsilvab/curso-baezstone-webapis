using BaezStone.MagicVilla.Api.Repositorio.IRepositorio;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BaezStone.MagicVilla.Api.Repositorio;

public class Repositorio<T> : IRepositorio<T>
    where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repositorio(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }

    public async Task Crear(T entidad)
    {
        await dbSet.AddAsync(entidad);
        await Grabar();
    }

    public async Task Grabar()
    {
        await _db.SaveChangesAsync();
    }

    public async Task<T?> Obtener(Expression<Func<T, bool>>? filtro = null, bool tracked = true)
    {
        IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

        return filtro is null
               ? await query.FirstOrDefaultAsync()
               : await query.FirstOrDefaultAsync(filtro);
    }

    //public Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null)
    //{
    //    IQueryable<T> query = dbSet;

    //    return filtro is null
    //           ? query.ToListAsync()
    //           : query.Where(filtro).ToListAsync();
    //}

    public async Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null,
        string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        if (filtro != null)
            query = query.Where(filtro);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp 
                in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return await query.ToListAsync();
    }

    public async Task Remover(T entidad)
    {
        dbSet.Remove(entidad);
        await Grabar();
    }
}
