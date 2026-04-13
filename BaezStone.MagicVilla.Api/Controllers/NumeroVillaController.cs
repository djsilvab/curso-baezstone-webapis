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
public class NumeroVillaController : ControllerBase
{
    private readonly ILogger<NumeroVillaController> _logger;
    private readonly IVillaRepositorio _villaRepo;
    private readonly INumeroVillaRepositorio _numeroVillaRepo;

    public NumeroVillaController(ILogger<NumeroVillaController> logger,
                           IVillaRepositorio villaRepo,
                           INumeroVillaRepositorio numeroVillaRepo)
    {
        _logger = logger;
        _villaRepo = villaRepo;
        _numeroVillaRepo = numeroVillaRepo;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<NumeroVillaDto>>>> GetNumeroVillas()
    {
        try
        {
            _logger.LogInformation("Solicitud GET api/NumeroVilla iniciada.");

            //var numeroVillas = await _numeroVillaRepo.ObtenerTodosConVilla();

            var numeroVillas = await _numeroVillaRepo.ObtenerTodos(includeProperties: "Villa");

            var numeroVillasDto = numeroVillas.Adapt<List<NumeroVillaDto>>();

            return Ok(new ApiResponse<List<NumeroVillaDto>>()
            {
                Resultado = numeroVillasDto,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los números de villas");

            var response = new ApiResponse<List<NumeroVillaDto>>()
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = [ex.Message]
            };

            return StatusCode((int)response.StatusCode, response);
        }
    }

    [HttpGet("{id:int}", Name = "GetNumeroVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<NumeroVillaDto>>> GetNumeroVilla(int id)
    {       

        if (id <= 0)
        {
            _logger.LogWarning("Id inválido al obtener número de villa: {Id}", id);

            return BadRequest(new ApiResponse<NumeroVillaDto>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMensajes = ["Id inválido."]
            });
        }

        try
        {
            var numeroVilla = await _numeroVillaRepo.Obtener(x => x.VillaNro == id, tracked: false);

            if (numeroVilla is null)
            {
                _logger.LogWarning("Número de villa no encontrada con id: {Id}", id);
                return NotFound(new ApiResponse<NumeroVillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = ["Número de Villa no encontrado."]
                });
            }

            var numeroVillaDto = numeroVilla.Adapt<NumeroVillaDto>();

            return Ok(new ApiResponse<NumeroVillaDto>
            {
                Resultado = numeroVillaDto,
                StatusCode = HttpStatusCode.OK,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener villa con id: {Id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError,
             new ApiResponse<NumeroVillaDto>
             {
                 EsExitoso = false,
                 StatusCode = HttpStatusCode.InternalServerError,
                 ErrorMensajes = [ex.Message]
             });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<NumeroVillaDto>>> CreateNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<NumeroVillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMensajes = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var existeNroVilla = await _numeroVillaRepo.Obtener(x => x.VillaNro == createDto.VillaNro, tracked: false);
            if (existeNroVilla != null )
            {
                _logger.LogWarning("Intento de crear número de villa duplicado: {VillaNro}", createDto.VillaNro);
                return Conflict(new ApiResponse<NumeroVillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.Conflict,
                    ErrorMensajes = ["Ya existe un número de villa con ese número."]
                });
            }

            var existeVilla = await _villaRepo.Obtener(x => x.Id == createDto.VillaId, tracked: false);
            if (existeVilla == null)
            {
                return NotFound(new ApiResponse<NumeroVillaDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = ["No existe una villa con ese Id."]
                });
            }

            var modelo = createDto.Adapt<NumeroVilla>();

            await _numeroVillaRepo.Crear(modelo);

            var resultDto = modelo.Adapt<NumeroVillaDto>();

            return CreatedAtAction(nameof(GetNumeroVilla), new { id = modelo.VillaNro }, new ApiResponse<NumeroVillaDto>
            {
                Resultado = resultDto,
                StatusCode = HttpStatusCode.Created
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el número villa");

            return StatusCode(StatusCodes.Status500InternalServerError,
              new ApiResponse<NumeroVillaDto>
              {
                  EsExitoso = false,
                  StatusCode = HttpStatusCode.InternalServerError,
                  ErrorMensajes = [ex.Message]
              });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteNumeroVilla(int id)
    {
        try
        {            
            if (id <= 0)
            {                
                return BadRequest(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMensajes = new List<string> { "Id inválido." }
                });
            }

            var nroVilla = await _numeroVillaRepo.Obtener(x => x.VillaNro == id, tracked: false);

            if (nroVilla == null)
            {
                _logger.LogWarning("Intento de eliminar número de villa inexistente: {Id}", id);
                return NotFound(new ApiResponse<object>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = ["Número de villa no encontrado."]
                });
            }

            await _numeroVillaRepo.Remover(nroVilla);

            return NoContent();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al borrar el número de villa");
            return StatusCode(StatusCodes.Status500InternalServerError,
            new ApiResponse<object>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes =[ex.Message ]
            });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<NumeroVillaUpdateDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMensajes = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            if (id != updateDto.VillaNro)
            {
                return BadRequest(new ApiResponse<NumeroVillaUpdateDto>
                {
                    ErrorMensajes = ["El Id no coincide."],
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }

            var villa = await _villaRepo.Obtener(x => x.Id == updateDto.VillaId, tracked: false);

            if (villa == null)
            {
                return NotFound(new ApiResponse<NumeroVillaUpdateDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes =["El Id de la villa no existe." ]
                });
            }

            var numeroVilla = await _numeroVillaRepo.Obtener(x => x.VillaNro == id, tracked: true);

            if (numeroVilla == null)
            {
                return NotFound(new ApiResponse<NumeroVillaUpdateDto>
                {
                    EsExitoso = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMensajes = ["Número de villa no existe."]
                });
            }

            updateDto.Adapt(numeroVilla);

            await _numeroVillaRepo.Actualizar(numeroVilla);

            return NoContent();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el número de villa");
            return StatusCode(StatusCodes.Status500InternalServerError,
            new ApiResponse<NumeroVillaUpdateDto>
            {
                EsExitoso = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMensajes = [ex.Message]
            });
        }
    }    
}
