using Microsoft.AspNetCore.Mvc;
using eventos_vivos.BLL.Interfaces;
using eventos_vivos.BDO.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;

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
                return StatusCode(500, new { message = "Ocurrió un error al iniciar sesión." });
            }
        }
    }
}