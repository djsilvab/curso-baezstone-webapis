using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BaezStone.MagicVilla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
    private readonly ILogger<VillaController> _logger;

    public VillaController(ILogger<VillaController> logger)
    {
        _logger = logger;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        _logger.LogInformation("Obtener todas las villas");
        return Ok(VillaStore.villaList);
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0)
        {
            _logger.LogError($"Error al obtener la Villa con Id : {id}");
            return BadRequest("Id debe ser mayor a cero.");
        }
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
        if (villaDto is null) return BadRequest("El objeto es nulo.");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (villaDto.Id > 0) return StatusCode(StatusCodes.Status400BadRequest, "El Id debe ser cero");
        if (VillaStore.villaList.FirstOrDefault(x => x.Nombre.Equals(villaDto.Nombre, StringComparison.OrdinalIgnoreCase)) is not null)
        {
            ModelState.AddModelError("NombreExistente", "Ya existe una villa con ese nombre.");
            return BadRequest(ModelState);
        }

        villaDto.Id = (VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault()?.Id ?? 0) + 1;

        //if (VillaStore.villaList.Any(v => v.Id == villaDto.Id)) return BadRequest("Ya existe una villa con ese Id.");
        VillaStore.villaList.Add(villaDto);
        return CreatedAtAction(nameof(GetVilla), new { id = villaDto.Id }, villaDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVilla(int id)
    {
        //Se utiliza IActionResult porque no se retorna un objeto
        if (id == 0) return BadRequest("Id debe ser mayor a cero.");
        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa is null) return NotFound();
        VillaStore.villaList.Remove(villa);
        return NoContent(); //204 No Content
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
    {
        if (villaDto == null || id != villaDto.Id) return BadRequest();
        var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
        if (villa is null) return NotFound();
        villa.Nombre = villaDto.Nombre;
        villa.Ocupantes = villaDto.Ocupantes;
        villa.MetrosCuadrados = villaDto.MetrosCuadrados;

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
    {
        if (patchDto == null || id == 0) return BadRequest();

        var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
        if (villa is null) return NotFound();

        patchDto.ApplyTo(villa, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return NoContent();
    }
}
