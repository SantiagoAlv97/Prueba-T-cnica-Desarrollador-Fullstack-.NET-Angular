using System.Reflection;
using eventos_vivos_api.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace eventos_vivos.Tests;

public class AuthorizationAttributesTests
{
    [Theory]
    [InlineData(typeof(ReservasController), nameof(ReservasController.ConfirmarPago))]
    [InlineData(typeof(EventosController), nameof(EventosController.Post))]
    [InlineData(typeof(EventosController), nameof(EventosController.Put))]
    [InlineData(typeof(EventosController), nameof(EventosController.Cancelar))]
    [InlineData(typeof(VenuesController), nameof(VenuesController.Post))]
    [InlineData(typeof(VenuesController), nameof(VenuesController.Put))]
    [InlineData(typeof(VenuesController), nameof(VenuesController.Delete))]
    [InlineData(typeof(ReportesController), nameof(ReportesController.ObtenerOcupacion))]
    public void EndpointsAdministrativos_DebenExigirRolAdministrador(Type controllerType, string methodName)
    {
        var method = controllerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("administrador", attribute!.Roles);
    }
}
