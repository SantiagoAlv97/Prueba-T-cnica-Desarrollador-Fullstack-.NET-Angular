using System.Security.Claims;
using eventos_vivos.BDO.DTOs.Auth;
using eventos_vivos.BLL.Interfaces;
using eventos_vivos_api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eventos_vivos.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Me_CuandoUsuarioAutenticado_ConsultaElUsuarioDelToken()
    {
        var service = new AuthServiceStub();
        var controller = CreateController(service, BuildUser(12));

        var result = await controller.Me();

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(12, service.LastUsuarioIdConsultado);
    }

    [Fact]
    public async Task Me_CuandoNoExisteUsuario_Devuelve404()
    {
        var service = new AuthServiceStub { UsuarioActual = null };
        var controller = CreateController(service, BuildUser(77));

        var result = await controller.Me();

        var objectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
    }

    private static AuthController CreateController(IAuthService service, ClaimsPrincipal user)
    {
        return new AuthController(service)
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

    private static ClaimsPrincipal BuildUser(long usuarioId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
        ], "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private sealed class AuthServiceStub : IAuthService
    {
        public long? LastUsuarioIdConsultado { get; private set; }

        public UsuarioPerfilResponse? UsuarioActual { get; set; } = new()
        {
            UsuarioId = 12,
            Nombre = "Ada",
            Email = "ada@example.com",
            Rol = "cliente",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        public Task<LoginResponse> LoginGoogleAsync(GoogleLoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioPerfilResponse?> ObtenerUsuarioActualAsync(long usuarioId)
        {
            LastUsuarioIdConsultado = usuarioId;
            return Task.FromResult(UsuarioActual);
        }
    }
}
