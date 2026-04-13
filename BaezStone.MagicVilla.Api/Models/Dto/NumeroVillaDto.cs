using System.ComponentModel.DataAnnotations;

namespace BaezStone.MagicVilla.Api.Models.Dto;

public class NumeroVillaDto
{
    [Required]
    public int VillaNro { get; set; }
    [Required]
    public int VillaId { get; set; }
    public string? DetalleEspecial { get; set; }
    public VillaDto? Villa { get; set; }
}
