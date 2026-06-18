using Microsoft.AspNetCore.Mvc;
using eventos_vivos.BLL.Interfaces;
using eventos_vivos.BDO.DTOs.Reservas;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eventos_vivos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _service;

        public ReservasController(IReservaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _service.ListarAsync();
            return Ok(list);
        }

        [Authorize]
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            var reserva = await _service.ObtenerPorIdAsync(id);

            if (reserva == null)
                return NotFound(new { message = "Reserva no encontrada." });

            if (!User.IsInRole("administrador") && reserva.UsuarioId != usuarioId)
                return StatusCode(403, new { message = "No tienes permiso para consultar esta reserva." });

            return Ok(reserva);
        }

        [Authorize]
        [HttpGet("mis-reservas")]
        public async Task<IActionResult> GetMisReservas()
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            var list = await _service.ListarPorUsuarioAsync(usuarioId);
            return Ok(list);
        }

        [Authorize]
        [HttpGet("usuario/{usuarioId:long}")]
        public async Task<IActionResult> GetPorUsuario(long usuarioId)
        {
            if (!TryObtenerUsuarioId(out var usuarioIdAutenticado))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            if (!User.IsInRole("administrador") && usuarioId != usuarioIdAutenticado)
                return StatusCode(403, new { message = "No tienes permiso para consultar reservas de otro usuario." });

            var list = await _service.ListarPorUsuarioAsync(usuarioId);
            return Ok(list);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CrearReservaRequest request)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            request.UsuarioId = usuarioId;

            try
            {
                var created = await _service.CrearAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.ReservaId },
                    created
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "administrador")]
        [HttpPatch("{id:long}/confirmar-pago")]
        public async Task<IActionResult> ConfirmarPago(long id)
        {
            var reserva = await _service.ObtenerPorIdAsync(id);

            if (reserva == null)
                return NotFound(new { message = "Reserva no encontrada." });

            try
            {
                var confirmada = await _service.ConfirmarPagoAsync(id);
                return Ok(confirmada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("{id:long}/cancelar")]
        public async Task<IActionResult> Cancelar(long id)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            var reserva = await _service.ObtenerPorIdAsync(id);

            if (reserva == null)
                return NotFound(new { message = "Reserva no encontrada." });

            if (!User.IsInRole("administrador") && reserva.UsuarioId != usuarioId)
                return StatusCode(403, new { message = "No tienes permiso para cancelar esta reserva." });

            try
            {
                var cancelada = await _service.CancelarAsync(id);
                return Ok(cancelada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool TryObtenerUsuarioId(out long usuarioId)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(usuarioIdClaim, out usuarioId);
        }
    }
}
