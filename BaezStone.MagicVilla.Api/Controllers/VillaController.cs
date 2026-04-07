using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaezStone.MagicVilla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
    private readonly ILogger<VillaController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public VillaController(ILogger<VillaController> logger, 
                            ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        _logger.LogInformation("Obtener todas las villas");
        return Ok(_dbContext.Villas.ToList());
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
        //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
        if (villa is null) return NotFound();
        return Ok(villa);
    }

    // Reemplazo del método CreateVilla
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<VillaDto> CreateVilla([FromBody] VillaCreateDto villaDto)
    {
        if (villaDto is null) 
            return BadRequest("El objeto es nulo.");

        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        // Normalizar el nombre recibido y proteger contra nulls para la consulta EF
        var nombreNormalized = (villaDto.Nombre ?? string.Empty).Trim().ToLower();

        // Usar Any y comparación en minúsculas para que EF pueda traducir la expresión a SQL
        var existeNombre = _dbContext.Villas
            .AsNoTracking()
            .Any(x => ((x.Nombre ?? string.Empty).ToLower()) == nombreNormalized);

        if (existeNombre)
        {
            ModelState.AddModelError("NombreExistente", "Ya existe una villa con ese nombre.");
            return BadRequest(ModelState);
        }

        var modelo = new Villa
        {
            Nombre = (villaDto.Nombre ?? string.Empty).Trim(),
            Detalle = villaDto.Detalle,
            Ocupantes = villaDto.Ocupantes,
            MetrosCuadrados = villaDto.MetrosCuadrados,
            Tarifa = villaDto.Tarifa,
            ImagenURL = villaDto.ImagenUrl,
            Amenidad = villaDto.Amenidad,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        _dbContext.Villas.Add(modelo);
        _dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetVilla), new { id = modelo.Id }, villaDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVilla(int id)
    {
        //Se utiliza IActionResult porque no se retorna un objeto
        if (id == 0) return BadRequest("Id debe ser mayor a cero.");
        var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
        if (villa is null) return NotFound();
        //VillaStore.villaList.Remove(villa);
        _dbContext.Villas.Remove(villa);
        _dbContext.SaveChanges();
        return NoContent(); //204 No Content
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
    {
        if (villaDto == null || id != villaDto.Id) return BadRequest();        

        var modelo = new Villa
        {
            Id = id,
            Nombre = villaDto.Nombre,
            Detalle = villaDto.Detalle,
            Ocupantes = villaDto.Ocupantes,
            MetrosCuadrados = villaDto.MetrosCuadrados,
            Tarifa = villaDto.Tarifa,
            ImagenURL = villaDto.ImagenUrl,
            Amenidad = villaDto.Amenidad
        };

        _dbContext.Villas.Update(modelo);
        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        if (patchDto == null || id == 0) 
            return BadRequest();

        var villaEntity = _dbContext.Villas.FirstOrDefault(x => x.Id == id);

        if (villaEntity is null) 
            return NotFound();

        var villaDto = new VillaUpdateDto
        {
            Id = villaEntity.Id,
            Nombre = villaEntity.Nombre,
            Detalle = villaEntity.Detalle,
            Ocupantes = villaEntity.Ocupantes,
            MetrosCuadrados = villaEntity.MetrosCuadrados,
            Tarifa = villaEntity.Tarifa,
            ImagenUrl = villaEntity.ImagenURL,
            Amenidad = villaEntity.Amenidad
        };

        patchDto.ApplyTo(villaDto, ModelState);

        if(!TryValidateModel(villaDto))
            return BadRequest(ModelState);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Mapear cambios al entity trackeado y persistir
        villaEntity.Nombre = villaDto.Nombre;
        villaEntity.Detalle = villaDto.Detalle;
        villaEntity.Ocupantes = villaDto.Ocupantes;
        villaEntity.MetrosCuadrados = villaDto.MetrosCuadrados;
        villaEntity.Tarifa = villaDto.Tarifa;
        villaEntity.ImagenURL = villaDto.ImagenUrl;
        villaEntity.Amenidad = villaDto.Amenidad;
        villaEntity.FechaActualizacion = DateTime.Now;

        _dbContext.Villas.Update(villaEntity);
        _dbContext.SaveChanges();

        return NoContent();
    }
}
