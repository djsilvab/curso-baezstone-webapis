using System.ComponentModel.DataAnnotations;

namespace BaezStone.MagicVilla.Api.Models.Dto;

public class VillaDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Nombre es requerido")]
    [MaxLength(30)]
    public string Nombre { get; set; }
}
