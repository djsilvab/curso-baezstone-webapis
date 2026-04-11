using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Repositorio.IRepositorio;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaezStone.MagicVilla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
    private readonly ILogger<VillaController> _logger;
    private readonly IVillaRepositorio _villaRepo;   

    public VillaController(ILogger<VillaController> logger,
                           IVillaRepositorio villaRepo)
    {
        _logger = logger;
        _villaRepo = villaRepo;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VillaDto>>> GetVillas()
    {
        try
        {
            _logger.LogInformation("Solicitud GET api/villa iniciada.");

            var villas = await _villaRepo.ObtenerTodos();

            var villasDto = villas.Adapt<List<VillaDto>>();

            return Ok(villasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las villas");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud." });
        }       
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDto>> GetVilla(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Id inválido al obtener villa: {Id}", id);
            return BadRequest(new { message = "Id debe ser mayor a cero." });
        }        

        try
        {
            var villa = await _villaRepo.Obtener(x => x.Id == id, false);           

            if (villa is null) 
            {
                _logger.LogWarning("Villa no encontrada con id: {Id}", id);
                return NotFound(new { message = $"No existe una villa con id {id}" });
            }

            var villaDto = villa.Adapt<VillaDto>();

            return Ok(villaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener villa con id: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error al procesar la solicitud." });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        if (createDto is null)
            return BadRequest(new { message = "El objeto es nulo." });

        // Normalizar el nombre recibido y proteger contra nulls para la consulta EF
        var nombre = (createDto.Nombre ?? string.Empty).Trim();

        // Usar Any y comparación en minúsculas para que EF pueda traducir la expresión a SQL
        var existeNombre = await _villaRepo.Obtener(x => x.Nombre == nombre, false) != null;

        if (existeNombre)
            return Conflict(new { message = "Ya existe una villa con ese nombre" });

        var modelo = createDto.Adapt<Villa>();

        modelo.Nombre = nombre;
        modelo.FechaCreacion = DateTime.UtcNow;
        modelo.FechaActualizacion = DateTime.UtcNow;

        await _villaRepo.Crear(modelo);       

        var resultDto = modelo.Adapt<VillaDto>();

        return CreatedAtAction(nameof(GetVilla), new { id = modelo.Id }, resultDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        //Se utiliza IActionResult porque no se retorna un objeto
        if (id <= 0)
            return BadRequest("Id debe ser mayor a cero.");

        var villa = await _villaRepo.Obtener(x => x.Id == id, false);                                            

        if (villa == null)
            return NotFound();

        await _villaRepo.Remover(villa);

        return NoContent(); //204 No Content
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto is null)
            return BadRequest(new { message = "El objeto es nulo." });

        if (id != updateDto.Id)
            return BadRequest(new { message = "El Id no coincide." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: true);

        if (villa is null)
            return NotFound();
        
        updateDto.Adapt(villa); // Mapear los cambios del DTO al entity trackeado        

        await _villaRepo.Actualizar(villa);

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id <= 0)
            return BadRequest(new { message = "Datos inválidos." });

        // 🔒 Evitar modificación de Id
        if (patchDto.Operations.Any(op => op.path.ToLower().Contains("id")))
            return BadRequest("No se permite modificar el Id.");

        var villaEntity = await _villaRepo.Obtener(x => x.Id == id, tracked: true);

        if (villaEntity is null)
            return NotFound();

        var villaDto = villaEntity.Adapt<VillaUpdateDto>();

        patchDto.ApplyTo(villaDto, ModelState);

        // Validar errores del patch
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        villaDto.Id = id; // Asegurar que el Id no se modifique

        if (!TryValidateModel(villaDto))
            return BadRequest(ModelState);

        villaDto.Adapt(villaEntity); // Mapear los cambios del DTO al entity trackeado

        await _villaRepo.Actualizar(villaEntity);

        return NoContent();
    }
}
