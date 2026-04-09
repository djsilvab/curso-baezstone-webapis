using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace BaezStone.MagicVilla.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
    private readonly ILogger<VillaController> _logger;
    private readonly ApplicationDbContext _dbContext;
    //private readonly IMapper _mapper;

    public VillaController(ILogger<VillaController> logger, 
                            ApplicationDbContext dbContext
                            )
    {
        _logger = logger;
        _dbContext = dbContext;
        //_mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
    {
        _logger.LogInformation("Obtener todas las villas");

        var villasDto = await _dbContext.Villas.AsNoTracking().Select(x => new VillaDto { 
            Id = x.Id,
            Nombre = x.Nombre,
            Detalle = x.Detalle,
            Ocupantes = x.Ocupantes,
            MetrosCuadrados = x.MetrosCuadrados,
            Tarifa = x.Tarifa,
            ImagenUrl = x.ImagenURL,
            Amenidad = x.Amenidad
        }).ToListAsync();

        return Ok(villasDto);
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

        var villaDto = await _dbContext.Villas.AsNoTracking().Where(x => x.Id == id).Select(x => new VillaDto
        {
            Id = x.Id,
            Nombre = x.Nombre,
            Detalle = x.Detalle,
            Ocupantes = x.Ocupantes,
            MetrosCuadrados = x.MetrosCuadrados,
            Tarifa = x.Tarifa,
            ImagenUrl = x.ImagenURL,
            Amenidad = x.Amenidad
        }).FirstOrDefaultAsync();

        if (villaDto is null) 
            return NotFound();

        return Ok(villaDto);
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
        var existeNombre = await _dbContext
                                    .Villas
                                    .AnyAsync(x => x.Nombre == nombre);

        if (existeNombre)
            return Conflict(new { message = "Ya existe una villa con ese nombre" });
        
        var modelo = createDto.Adapt<Villa>();

        modelo.Nombre = nombre;
        modelo.FechaCreacion = DateTime.UtcNow;
        modelo.FechaActualizacion = DateTime.UtcNow;

        await _dbContext.Villas.AddAsync(modelo);
        await _dbContext.SaveChangesAsync();
        
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

        var rowAffected = await _dbContext.Villas
                                            .Where(v => v.Id == id)
                                            .ExecuteDeleteAsync();

        if (rowAffected == 0) 
            return NotFound();

        return NoContent(); //204 No Content
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        if (updateDto is null )
            return BadRequest(new { message = "El objeto es nulo." });

        if (id != updateDto.Id)
            return BadRequest(new { message = "El Id no coincide." });

        var villa = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);

        if (villa is null) 
            return NotFound();

        //_mapper.Map(updateDto, villa); // Mapear los cambios del DTO al entity trackeado
        updateDto.Adapt(villa); // Mapear los cambios del DTO al entity trackeado

        villa.FechaActualizacion = DateTime.UtcNow;
                
        await _dbContext.SaveChangesAsync();

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

        var villaEntity = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);

        if (villaEntity is null) 
            return NotFound();
        
        var villaDto = villaEntity.Adapt<VillaUpdateDto>();

        patchDto.ApplyTo(villaDto, ModelState);

        villaDto.Id = id; // Asegurar que el Id no se modifique

        if (!TryValidateModel(villaDto))
            return BadRequest(ModelState);

        //_mapper.Map(villaDto, villaEntity); // Mapear los cambios del DTO al entity trackeado
        villaDto.Adapt(villaEntity); // Mapear los cambios del DTO al entity trackeado

        villaEntity.FechaActualizacion = DateTime.UtcNow;
                
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
