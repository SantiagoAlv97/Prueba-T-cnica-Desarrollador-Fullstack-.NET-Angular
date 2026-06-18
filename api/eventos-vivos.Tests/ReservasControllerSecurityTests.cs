using System.Security.Claims;
using eventos_vivos.BDO.DTOs.Reservas;
using eventos_vivos.BLL.Interfaces;
using eventos_vivos_api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eventos_vivos.Tests;

public class ReservasControllerSecurityTests
{
    [Fact]
    public async Task GetPorUsuario_CuandoClienteConsultaOtroUsuario_Devuelve403()
    {
        var service = new ReservaServiceStub();
        var controller = CreateController(service, BuildUser(usuarioId: 8, rol: "cliente"));

        var result = await controller.GetPorUsuario(99);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetMisReservas_CuandoUsuarioAutenticado_UsaElUsuarioDelToken()
    {
        var service = new ReservaServiceStub();
        var controller = CreateController(service, BuildUser(usuarioId: 11, rol: "cliente"));

        var result = await controller.GetMisReservas();

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(11, service.LastUsuarioIdListado);
    }

    [Fact]
    public async Task Post_CuandoClienteManipulaUsuarioId_SeSobrescribeConElToken()
    {
        var service = new ReservaServiceStub();
        var controller = CreateController(service, BuildUser(usuarioId: 42, rol: "cliente"));

        var result = await controller.Post(new CrearReservaRequest
        {
            EventoId = 5,
            UsuarioId = 999,
            Cantidad = 2
        });

        Assert.IsType<CreatedAtActionResult>(result);
        Assert.NotNull(service.LastCreateRequest);
        Assert.Equal(42, service.LastCreateRequest!.UsuarioId);
    }

    private static ReservasController CreateController(IReservaService service, ClaimsPrincipal user)
    {
        return new ReservasController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            }
        };
    }

    private static ClaimsPrincipal BuildUser(long usuarioId, string rol)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim(ClaimTypes.Role, rol)
        ], "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private sealed class ReservaServiceStub : IReservaService
    {
        public long? LastUsuarioIdListado { get; private set; }

        public CrearReservaRequest? LastCreateRequest { get; private set; }

        public Task<ReservaResponse> CancelarAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<ReservaResponse> ConfirmarPagoAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<ReservaResponse> CrearAsync(CrearReservaRequest request)
        {
            LastCreateRequest = request;

            return Task.FromResult(new ReservaResponse
            {
                ReservaId = 1,
                EventoId = request.EventoId,
                UsuarioId = request.UsuarioId,
                Cantidad = request.Cantidad
            });
        }

        public Task<IEnumerable<ReservaResponse>> ListarAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ReservaResponse>> ListarPorUsuarioAsync(long usuarioId)
        {
            LastUsuarioIdListado = usuarioId;
            return Task.FromResult<IEnumerable<ReservaResponse>>([]);
        }

        public Task<ReservaResponse?> ObtenerPorIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
