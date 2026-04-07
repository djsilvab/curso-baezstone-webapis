using System.ComponentModel.DataAnnotations;

namespace BaezStone.MagicVilla.Api.Models.Dto;

public class VillaDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Nombre es requerido")]
    [MaxLength(30)]
    public string Nombre { get; set; }
    public string Detalle { get; set; }
    public int Ocupantes{ get; set; }
    public int MetrosCuadrados { get; set; }
    [Required]
    public double Tarifa { get; set; }
    public string ImagenUrl { get; set; }
    public string Amenidad { get; set; }
}
