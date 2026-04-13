using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Repositorio.IRepositorio;
using BaezStone.MagicVilla.Api.Store;

namespace BaezStone.MagicVilla.Api.Repositorio;

public class VillaRepositorio : Repositorio<Villa>, IVillaRepositorio
{
    private readonly ApplicationDbContext _db;

    public VillaRepositorio(ApplicationDbContext db)
        : base(db)
    {
        _db = db;
    }

    public async Task<Villa> Actualizar(Villa entidad)
    {
        entidad.FechaActualizacion = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return entidad;
    }
}
