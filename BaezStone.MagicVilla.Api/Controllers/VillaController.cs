using BaezStone.MagicVilla.Api.Models;
using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Repositorio.IRepositorio;
using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
    public async Task<ActionResult<ApiResponse<List<VillaDto>>>> GetVillas()
    {
        try
        {
            _logger.LogInformation("Solicitud GET api/villa iniciada.");

            var villas = await _villaRepo.ObtenerTodos();

            var villasDto = villas.Adapt<List<VillaDto>>();

            return Ok(new ApiResponse<List<VillaDto>>()
            {
                Resultado = villasDto,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las villas");

            var response = new ApiResponse<List<VillaDto>>()
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = new List<string> { ex.Message }
            };

            return StatusCode((int)response.StatusCode, response);
        }
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<VillaDto>>> GetVilla(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Id inválido al obtener villa: {Id}", id);

            return BadRequest(new ApiResponse<VillaDto>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMensajes = new List<string> { "Id inválido." }
            });
        }

        try
        {
            var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: false);

            if (villa is null)
            {
                _logger.LogWarning("Villa no encontrada con id: {Id}", id);
                return NotFound(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = new List<string> { "Villa no encontrada." }
                });
            }

            var villaDto = villa.Adapt<VillaDto>();

            return Ok(new ApiResponse<VillaDto>
            {
                Resultado = villaDto,
                StatusCode = HttpStatusCode.OK,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener villa con id: {Id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError,
             new ApiResponse<VillaDto>
             {
                 EsExitoso = false,
                 StatusCode = HttpStatusCode.InternalServerError,
                 ErrorMensajes = new List<string> { ex.Message }
             });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<VillaDto>>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        try
        {
            // Normalizar el nombre recibido y proteger contra nulls para la consulta EF
            var nombre = (createDto.Nombre ?? string.Empty).Trim();

            // Usar Any y comparación en minúsculas para que EF pueda traducir la expresión a SQL
            var existeNombre = await _villaRepo.Obtener(x => x.Nombre == nombre, tracked: false) != null;

            if (existeNombre)
            {
                return Conflict(new ApiResponse<VillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.Conflict,
                    ErrorMensajes = new List<string> { "Ya existe una villa con ese nombre." }
                });
            }

            var modelo = createDto.Adapt<Villa>();

            modelo.Nombre = nombre;
            modelo.FechaCreacion = DateTime.UtcNow;
            modelo.FechaActualizacion = DateTime.UtcNow;

            await _villaRepo.Crear(modelo);

            var resultDto = modelo.Adapt<VillaDto>();

            return CreatedAtAction(nameof(GetVilla), new { id = modelo.Id }, new ApiResponse<VillaDto>
            {
                Resultado = resultDto,
                StatusCode = HttpStatusCode.Created
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la villa");

            return StatusCode(StatusCodes.Status500InternalServerError,
              new ApiResponse<VillaDto>
              {
                  EsExitoso = false,
                  StatusCode = HttpStatusCode.InternalServerError,
                  ErrorMensajes = new List<string> { ex.Message }
              });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteVilla(int id)
    {
        try
        {
            //Se utiliza IActionResult porque no se retorna un objeto
            if (id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMensajes = new List<string> { "Id inválido." }
                });
            }

            var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: false);

            if (villa == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = new List<string> { "Villa no encontrada." }
                });
            }

            await _villaRepo.Remover(villa);

            return NoContent();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al borrar la villa");
            return StatusCode(StatusCodes.Status500InternalServerError,
            new ApiResponse<object>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = new List<string> { ex.Message }
            });
        }

    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
    {
        try
        {
            if (id != updateDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    ErrorMensajes = new List<string> { "El Id no coincide." },
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            // Usar Any y comparación en minúsculas para que EF pueda traducir la expresión a SQL
            var existeNombre = await _villaRepo.Obtener(x => x.Nombre == updateDto.Nombre, tracked: false) != null;

            if (existeNombre)
            {
                return Conflict(new ApiResponse<VillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.Conflict,
                    ErrorMensajes = new List<string> { "Ya existe una villa con ese nombre." }
                });
            }

            var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: true);

            if (villa is null)
            {
                return NotFound(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = new List<string> { "Villa no encontrada." }
                });
            }

            updateDto.Adapt(villa);

            await _villaRepo.Actualizar(villa);

            return NoContent();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la villa");
            return StatusCode(StatusCodes.Status500InternalServerError,
            new ApiResponse<object>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = new List<string> { ex.Message }
            });
        }
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
    {
        try
        {

            if (patchDto == null || id <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    ErrorMensajes = new List<string> { "Datos inválidos." },
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            // Evitar modificación de Id
            if (patchDto.Operations.Any(op => op.path.Equals("/id", StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new ApiResponse<object>
                {
                    ErrorMensajes = new List<string> { "No se permite modificar el Id." },
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            var villaEntity = await _villaRepo.Obtener(x => x.Id == id, tracked: true);

            if (villaEntity is null)
                return NotFound(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = new List<string> { "Villa no encontrada." }
                });

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la villa");
            return StatusCode(StatusCodes.Status500InternalServerError,
            new ApiResponse<object>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = new List<string> { ex.Message }
            });
        }
    }
}
