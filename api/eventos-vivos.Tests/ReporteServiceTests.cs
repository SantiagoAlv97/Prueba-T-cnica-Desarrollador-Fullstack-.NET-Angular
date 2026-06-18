using eventos_vivos.BDO.Enums;
using eventos_vivos.BLL.Services;
using eventos_vivos.Tests.TestInfrastructure;

namespace eventos_vivos.Tests;

public class ReporteServiceTests
{
    [Fact]
    public async Task ObtenerReporteOcupacionAsync_CalculaEntradasVendidasPorcentajeEIngresosSoloConReservasConfirmadas()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 20, precioEntrada: 120m);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 3, EstadoReservaEnum.Confirmada, codigoReserva: "EV-000001");
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.PendientePago);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 1, EstadoReservaEnum.Perdida, codigoReserva: "EV-000002");
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 4, EstadoReservaEnum.Cancelada);
        var service = new ReporteService(database.Context);

        var reporte = await service.ObtenerReporteOcupacionAsync(evento.EventoId);

        Assert.NotNull(reporte);
        Assert.Equal(3, reporte!.EntradasVendidas);
        Assert.Equal(15m, reporte.PorcentajeOcupacion);
        Assert.Equal(360m, reporte.TotalIngresos);
    }

    [Fact]
    public async Task ObtenerReporteOcupacionAsync_CalculaEntradasDisponiblesSinContarCanceladas()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 20, precioEntrada: 90m);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 3, EstadoReservaEnum.Confirmada, codigoReserva: "EV-000001");
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.PendientePago);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 1, EstadoReservaEnum.Perdida, codigoReserva: "EV-000002");
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 4, EstadoReservaEnum.Cancelada);
        var service = new ReporteService(database.Context);

        var reporte = await service.ObtenerReporteOcupacionAsync(evento.EventoId);

        Assert.NotNull(reporte);
        Assert.Equal(14, reporte!.EntradasDisponibles);
        Assert.Equal("activo", reporte.EstadoEvento);
    }

    [Fact]
    public async Task ObtenerReporteOcupacionAsync_CuandoEventoActivoYaFinalizo_MuestraEstadoActualDelEvento()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(
            database.Context,
            fechaInicio: DateTime.UtcNow.AddDays(-2),
            fechaFin: DateTime.UtcNow.AddDays(-1),
            estado: EstadoEventoEnum.Activo);
        var service = new ReporteService(database.Context);

        var reporte = await service.ObtenerReporteOcupacionAsync(evento.EventoId);

        Assert.NotNull(reporte);
        Assert.Equal("completado", reporte!.EstadoEvento);
    }
}
