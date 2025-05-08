using BaezStone.MagicVilla.Api.Models.Dto;

namespace BaezStone.MagicVilla.Api.Store;
public static class VillaStore
{
    public static List<VillaDto> villaList = new List<VillaDto>
    {
        new VillaDto
        {
            Id = 1,
            Nombre = "Villa 1"
        },
        new VillaDto
        {
            Id = 2,
            Nombre = "Villa 2"
        }
    };
}
