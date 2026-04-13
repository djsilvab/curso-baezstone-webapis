using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaezStone.MagicVilla.Api.Models;

public class NumeroVilla : BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VillaNro { get; set; }
    [Required]
    public int VillaId { get; set; }    
    public Villa? Villa { get; set; }
    public string? DetalleEspecial { get; set; }    
}
