using BaezStone.MagicVilla.Api.Models.Dto;

namespace BaezStone.MagicVilla.Api.Store;
public static class VillaStore
{
    public static List<VillaDto> villaList = new List<VillaDto>
    {
        new VillaDto
        {
            Id = 1,
            Nombre = "Vista a la piscina",
            Ocupantes = 3,
            MetrosCuadrados = 50
        },
        new VillaDto
        {
            Id = 2,
            Nombre = "Villa 2",
            Ocupantes  = 4,
            MetrosCuadrados = 80
        }
    };
}
