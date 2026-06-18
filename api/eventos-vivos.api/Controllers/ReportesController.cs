using Microsoft.AspNetCore.Mvc;
using eventos_vivos.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace eventos_vivos_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _service;

        public ReportesController(IReporteService service)
        {
            _service = service;
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("eventos/{eventoId:long}/ocupacion")]
        public async Task<IActionResult> ObtenerOcupacion(long eventoId)
        {
            try
            {
                var reporte = await _service.ObtenerReporteOcupacionAsync(eventoId);

                if (reporte == null)
                    return NotFound(new { message = "Evento no encontrado." });

                return Ok(reporte);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}