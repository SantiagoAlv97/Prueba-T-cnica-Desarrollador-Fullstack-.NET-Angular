using Microsoft.AspNetCore.Mvc;
using eventos_vivos.BLL.Interfaces;
using eventos_vivos.BDO.DTOs.Eventos;
using Microsoft.AspNetCore.Authorization;

namespace eventos_vivos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _service;

        public EventosController(IEventoService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] EventoFiltroRequest filtro)
        {
            var list = await _service.ListarAsync(filtro);
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var evento = await _service.ObtenerPorIdAsync(id);

            if (evento == null)
                return NotFound(new { message = "Evento no encontrado." });

            return Ok(evento);
        }

        [AllowAnonymous]
        [HttpGet("tipos")]
        public async Task<IActionResult> GetTipos()
        {
            var tipos = await _service.ListarTiposAsync();
            return Ok(tipos);
        }

        [AllowAnonymous]
        [HttpGet("estados")]
        public async Task<IActionResult> GetEstados()
        {
            var estados = await _service.ListarEstadosAsync();
            return Ok(estados);
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CrearEventoRequest request)
        {
            try
            {
                var created = await _service.CrearAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.EventoId },
                    created
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Put(long id, [FromBody] ActualizarEventoRequest request)
        {
            try
            {
                var ok = await _service.ActualizarAsync(id, request);

                if (!ok)
                    return NotFound(new { message = "Evento no encontrado." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPatch("{id:long}/cancelar")]
        public async Task<IActionResult> Cancelar(long id)
        {
            try
            {
                var ok = await _service.CancelarAsync(id);

                if (!ok)
                    return NotFound(new { message = "Evento no encontrado." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
