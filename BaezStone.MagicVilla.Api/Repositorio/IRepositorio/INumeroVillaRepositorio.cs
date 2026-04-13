using BaezStone.MagicVilla.Api.Models;

namespace BaezStone.MagicVilla.Api.Repositorio.IRepositorio;

public interface INumeroVillaRepositorio : IRepositorio<NumeroVilla>
{
    Task<NumeroVilla?> Actualizar(NumeroVilla entidad);
    Task<List<NumeroVilla>> ObtenerTodosConVilla();
}
