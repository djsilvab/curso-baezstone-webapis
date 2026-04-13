using System.ComponentModel.DataAnnotations;

namespace BaezStone.MagicVilla.Api.Models.Dto;

public class NumeroVillaCreateDto
{
    [Required]
    public int VillaNro { get; set; }
    [Required]
    public int VillaId { get; set; }
    public string DetalleEspecial { get; set; }
}
