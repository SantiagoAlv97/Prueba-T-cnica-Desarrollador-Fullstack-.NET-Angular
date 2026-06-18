using eventos_vivos.BDO.DTOs.Auth;
using eventos_vivos.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eventos_vivos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { message = "El token de Google es obligatorio." });

            try
            {
                var response = await _service.LoginGoogleAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Ocurrio un error al iniciar sesion." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized(new { message = "Usuario no autenticado correctamente." });

            var usuario = await _service.ObtenerUsuarioActualAsync(usuarioId);

            if (usuario is null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(usuario);
        }

        private bool TryObtenerUsuarioId(out long usuarioId)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(usuarioIdClaim, out usuarioId);
        }
    }
}
