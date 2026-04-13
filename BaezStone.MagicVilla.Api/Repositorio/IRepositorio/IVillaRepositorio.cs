using BaezStone.MagicVilla.Api.Models;

namespace BaezStone.MagicVilla.Api.Repositorio.IRepositorio;

public interface IVillaRepositorio : IRepositorio<Villa>
{
    Task<Villa> Actualizar(Villa entidad);
}
