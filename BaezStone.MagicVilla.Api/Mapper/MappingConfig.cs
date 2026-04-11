using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Models.Dto;
using Mapster;

namespace BaezStone.MagicVilla.Api.Mapper;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Villa, VillaDto>
            .NewConfig()
            .Map(dest => dest.ImagenUrl, src => src.ImagenURL);

        TypeAdapterConfig<VillaCreateDto, Villa>
            .NewConfig()
            .Map(dest => dest.ImagenURL, src => src.ImagenUrl);

        TypeAdapterConfig<VillaUpdateDto, Villa>
            .NewConfig()
            .Map(dest => dest.ImagenURL, src => src.ImagenUrl)
            .IgnoreNullValues(true);

        TypeAdapterConfig<Villa, VillaUpdateDto>
            .NewConfig()
            .Map(dest => dest.ImagenUrl, src => src.ImagenURL);

    }
}
