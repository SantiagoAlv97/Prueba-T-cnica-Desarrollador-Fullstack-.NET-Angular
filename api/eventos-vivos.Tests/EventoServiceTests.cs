using eventos_vivos.BDO.DTOs.Eventos;
using eventos_vivos.BDO.Enums;
using eventos_vivos.BLL.Services;
using eventos_vivos.Tests.TestInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace eventos_vivos.Tests;

public class EventoServiceTests
{
    [Fact]
    public async Task CrearAsync_CuandoEventoEsValido_CreaEventoActivoConCuposDisponibles()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest();

        var response = await service.CrearAsync(request);

        Assert.Equal(1L, response.EventoId);
        Assert.Equal(request.Titulo, response.Titulo);
        Assert.Equal((long)EstadoEventoEnum.Activo, response.EstadoEventoId);
        Assert.Equal("activo", response.EstadoEvento);
        Assert.Equal(request.CapacidadMaxima, response.CuposDisponibles);
    }

    [Fact]
    public async Task CrearAsync_CuandoTituloTieneMenosDeCincoCaracteres_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest();
        request.Titulo = "Abcd";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        TestAssert.MessageEquals(exception.Message, "Título inválido", "TÃ­tulo invÃ¡lido");
    }

    [Fact]
    public async Task CrearAsync_CuandoDescripcionTieneMenosDeDiezCaracteres_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest();
        request.Descripcion = "Muy corta";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        TestAssert.MessageEquals(exception.Message, "Descripción inválida", "DescripciÃ³n invÃ¡lida");
    }

    [Fact]
    public async Task CrearAsync_CuandoCapacidadSuperaVenue_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest(capacidadMaxima: 101);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        Assert.Equal("Capacidad máxima no puede ser mayor a la capacidad del venue", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoFechaInicioEsPasada_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest(
            fechaInicio: DateTime.UtcNow.AddMinutes(-5),
            fechaFin: DateTime.UtcNow.AddHours(2));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        Assert.Equal("Fecha inicio debe ser futura", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoFechaFinNoEsPosteriorALaFechaInicio_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var inicio = DateTime.UtcNow.AddDays(5);
        var request = TestData.BuildValidEventoRequest(fechaInicio: inicio, fechaFin: inicio);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        Assert.Equal("Fecha fin debe ser mayor a fecha inicio", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoPrecioNoEsPositivo_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var request = TestData.BuildValidEventoRequest(precioEntrada: 0m);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        Assert.Equal("Precio entrada debe ser mayor a 0", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoHayCruceEnMismoVenue_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var inicio = DateTime.UtcNow.AddDays(7);
        await TestData.AddEventoAsync(database.Context, fechaInicio: inicio, fechaFin: inicio.AddHours(4));

        var request = TestData.BuildValidEventoRequest(
            fechaInicio: inicio.AddHours(1),
            fechaFin: inicio.AddHours(5));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        Assert.Equal("Ya existe un evento activo en el mismo venue con horario superpuesto", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoCruceEsEnVenueDistinto_PermiteCreacion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var inicio = DateTime.UtcNow.AddDays(7);
        await TestData.AddEventoAsync(database.Context, venueId: 1, fechaInicio: inicio, fechaFin: inicio.AddHours(4));

        var request = TestData.BuildValidEventoRequest(
            venueId: 2,
            capacidadMaxima: 40,
            fechaInicio: inicio.AddHours(1),
            fechaFin: inicio.AddHours(5));

        var response = await service.CrearAsync(request);

        Assert.Equal(2L, response.EventoId);
        Assert.Equal(2L, response.VenueId);
    }

    [Fact]
    public async Task CrearAsync_CuandoEventoDeFinDeSemanaIniciaDespuesDeLas2200_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var inicio = TestData.NextSaturdayAt(22, 1);
        var request = TestData.BuildValidEventoRequest(fechaInicio: inicio, fechaFin: inicio.AddHours(2));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(request));

        TestAssert.MessageEquals(
            exception.Message,
            "Los eventos de fin de semana no pueden iniciar después de las 22:00",
            "Los eventos de fin de semana no pueden iniciar despuÃ©s de las 22:00");
    }

    [Fact]
    public async Task CrearAsync_CuandoEventoDeFinDeSemanaIniciaALas2200_PermiteCreacion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var inicio = TestData.NextSaturdayAt(22);
        var request = TestData.BuildValidEventoRequest(fechaInicio: inicio, fechaFin: inicio.AddHours(2));

        var response = await service.CrearAsync(request);

        Assert.Equal((long)EstadoEventoEnum.Activo, response.EstadoEventoId);
        Assert.Equal(inicio, response.FechaInicio);
    }

    [Fact]
    public async Task ListarAsync_CuandoEventoActivoYaFinalizo_LoMarcaComoCompletado()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var service = new EventoService(database.Context);
        var evento = await TestData.AddEventoAsync(
            database.Context,
            fechaInicio: DateTime.UtcNow.AddDays(-2),
            fechaFin: DateTime.UtcNow.AddDays(-1),
            estado: EstadoEventoEnum.Activo);

        var response = await service.ListarAsync(new EventoFiltroRequest());
        var eventoActualizado = await database.Context.Eventos
            .AsNoTracking()
            .FirstAsync(x => x.EventoId == evento.EventoId);

        var listado = Assert.Single(response);
        Assert.Equal((long)EstadoEventoEnum.Completado, eventoActualizado.EstadoEventoId);
        Assert.Equal((long)EstadoEventoEnum.Completado, listado.EstadoEventoId);
        Assert.Equal("completado", listado.EstadoEvento);
    }
}
