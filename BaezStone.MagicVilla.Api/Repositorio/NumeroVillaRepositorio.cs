using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Repositorio.IRepositorio;
using BaezStone.MagicVilla.Api.Store;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BaezStone.MagicVilla.Api.Repositorio;

public class NumeroVillaRepositorio : Repositorio<NumeroVilla>, INumeroVillaRepositorio
{
    private readonly ApplicationDbContext _db;

    public NumeroVillaRepositorio(ApplicationDbContext db)
        : base(db)
    {
        _db = db;
    }

    public async Task<NumeroVilla?> Actualizar(NumeroVilla entidad)
    {
        if (entidad == null)
            throw new ArgumentNullException(nameof(entidad));

        var entidadDb = await _db.NumeroVillas.FirstOrDefaultAsync(x => x.VillaNro == entidad.VillaNro);

        if (entidadDb == null)
            return null;

        // Mapear propiedades
        entidad.Adapt(entidadDb);
        entidadDb.FechaActualizacion = DateTime.UtcNow;
        
        await _db.SaveChangesAsync();
        return entidadDb;
    }

    public async Task<List<NumeroVilla>> ObtenerTodosConVilla()
    {
        return await _db.NumeroVillas
            .Include(x => x.Villa)
            .ToListAsync();
    }
}
