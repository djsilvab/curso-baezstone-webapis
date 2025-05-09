using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BaezStone.MagicVilla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        return Ok(VillaStore.villaList);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0) return BadRequest("Id debe ser mayor a cero.");
        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa is null) return NotFound();
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villaDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (villaDto is null) return BadRequest("El objeto es nulo.");
        if(villaDto.Id > 0) return StatusCode(StatusCodes.Status400BadRequest, "El Id debe ser cero");
        if(VillaStore.villaList.FirstOrDefault(x => x.Nombre.ToLower() == villaDto.Nombre.ToLower()) is not null) 
        { 
            ModelState.AddModelError("NombreExistente", "Ya existe una villa con ese nombre.");
            return BadRequest(ModelState);
        }

        villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault()?.Id + 1 ?? 1;

        //if (VillaStore.villaList.Any(v => v.Id == villaDto.Id)) return BadRequest("Ya existe una villa con ese Id.");
        VillaStore.villaList.Add(villaDto);
        return CreatedAtAction(nameof(GetVilla), new { id = villaDto.Id }, villaDto);
    }
}
