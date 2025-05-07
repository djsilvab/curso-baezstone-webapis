using BaezStone.MagicVilla.Api.Models.Dto;
using BaezStone.MagicVilla.Api.Store;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{id:int}")]
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
}
