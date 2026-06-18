using eventos_vivos.BLL.Interfaces;
using eventos_vivos.BDO.DTOs.Venues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eventos_vivos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var venues = await _venueService.ListarAsync();
            return Ok(venues);
        }

        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var venue = await _venueService.ObtenerPorIdAsync(id);

            if (venue == null)
                return NotFound(new { message = "Venue no encontrado." });

            return Ok(venue);
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CrearVenueRequest request)
        {
            try
            {
                var created = await _venueService.CrearAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.VenueId },
                    created
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Put(long id, [FromBody] ActualizarVenueRequest request)
        {
            try
            {
                var ok = await _venueService.ActualizarAsync(id, request);

                if (!ok)
                    return NotFound(new { message = "Venue no encontrado." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var ok = await _venueService.EliminarAsync(id);

                if (!ok)
                    return NotFound(new { message = "Venue no encontrado." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}