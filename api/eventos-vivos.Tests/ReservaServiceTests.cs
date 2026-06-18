using eventos_vivos.BDO.DTOs.Reservas;
using eventos_vivos.BDO.Enums;
using eventos_vivos.BLL.Services;
using eventos_vivos.Tests.TestInfrastructure;

namespace eventos_vivos.Tests;

public class ReservaServiceTests
{
    [Fact]
    public async Task CrearAsync_CuandoReservaEsValida_CreaReservaPendientePago()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 50, precioEntrada: 75m);
        var service = new ReservaService(database.Context);

        var response = await service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 2
        });

        Assert.Equal((long)EstadoReservaEnum.PendientePago, response.EstadoReservaId);
        Assert.Equal("pendiente_pago", response.EstadoReserva);
        Assert.Equal("Cliente Uno", response.NombreComprador);
        Assert.Equal("cliente1@test.dev", response.EmailComprador);
        Assert.Null(response.CodigoReserva);
    }

    [Fact]
    public async Task CrearAsync_CuandoNoHayCuposDisponibles_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 5);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 5, EstadoReservaEnum.Confirmada);
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 1
        }));

        Assert.Equal("No hay suficientes entradas disponibles", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoCantidadEsMenorAUno_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 0
        }));

        Assert.Equal("Cantidad debe ser mayor o igual a 1", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoFaltaMenosDeUnaHora_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var inicio = DateTime.UtcNow.AddMinutes(45);
        var evento = await TestData.AddEventoAsync(database.Context, fechaInicio: inicio, fechaFin: inicio.AddHours(2));
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 1
        }));

        TestAssert.MessageEquals(
            exception.Message,
            "No se permiten reservas cuando faltan menos de 1 hora para el evento",
            "No se permiten reservas cuando faltan menos de 1 hora para el evento");
    }

    [Fact]
    public async Task CrearAsync_CuandoFaltanMenosDe24HorasYSuperaCincoEntradas_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var inicio = DateTime.UtcNow.AddHours(12);
        var evento = await TestData.AddEventoAsync(database.Context, fechaInicio: inicio, fechaFin: inicio.AddHours(2));
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 6
        }));

        TestAssert.MessageEquals(
            exception.Message,
            "Si faltan menos de 24 horas para el evento, solo se permiten máximo 5 entradas por transacción",
            "Si faltan menos de 24 horas para el evento, solo se permiten mÃ¡ximo 5 entradas por transacciÃ³n");
    }

    [Fact]
    public async Task CrearAsync_CuandoPrecioSuperaCienYSuperaDiezEntradas_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, precioEntrada: 150m, capacidadMaxima: 30);
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 11
        }));

        Assert.Equal("Para eventos con precio de entrada superior a 100, se permite un máximo de 10 entradas por transacción.", exception.Message);
    }

    [Fact]
    public async Task ConfirmarPagoAsync_CuandoReservaEstaPendiente_LaConfirmaYGeneraCodigo()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var service = new ReservaService(database.Context);
        var reserva = await service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 2
        });

        var response = await service.ConfirmarPagoAsync(reserva.ReservaId);

        Assert.Equal((long)EstadoReservaEnum.Confirmada, response.EstadoReservaId);
        Assert.Equal("confirmada", response.EstadoReserva);
        Assert.Equal("EV-000001", response.CodigoReserva);
        Assert.NotNull(response.FechaPago);
    }

    [Fact]
    public async Task ConfirmarPagoAsync_CuandoReservaYaEstaConfirmada_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.Confirmada, codigoReserva: "EV-000001");
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.ConfirmarPagoAsync(reserva.ReservaId));

        TestAssert.MessageEquals(exception.Message, "La reserva ya está confirmada", "La reserva ya estÃ¡ confirmada");
    }

    [Fact]
    public async Task ConfirmarPagoAsync_CuandoReservaEstaCancelada_LanzaExcepcion()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.Cancelada);
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.ConfirmarPagoAsync(reserva.ReservaId));

        Assert.Equal("No se puede confirmar una reserva cancelada", exception.Message);
    }

    [Fact]
    public async Task CancelarAsync_CuandoReservaEstaPendiente_LaMarcaComoCancelada()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.PendientePago);
        var service = new ReservaService(database.Context);

        var response = await service.CancelarAsync(reserva.ReservaId);

        Assert.Equal((long)EstadoReservaEnum.Cancelada, response.EstadoReservaId);
        Assert.Equal("cancelada", response.EstadoReserva);
        Assert.NotNull(response.FechaCancelacion);
    }

    [Fact]
    public async Task CancelarAsync_CuandoReservaConfirmadaTiene48HorasOMas_LaMarcaComoCancelada()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var inicio = DateTime.UtcNow.AddDays(3);
        var evento = await TestData.AddEventoAsync(database.Context, fechaInicio: inicio, fechaFin: inicio.AddHours(2));
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.Confirmada, codigoReserva: "EV-000001");
        var service = new ReservaService(database.Context);

        var response = await service.CancelarAsync(reserva.ReservaId);

        Assert.Equal((long)EstadoReservaEnum.Cancelada, response.EstadoReservaId);
        Assert.Equal("cancelada", response.EstadoReserva);
    }

    [Fact]
    public async Task CancelarAsync_CuandoReservaConfirmadaTieneMenosDe48Horas_LaMarcaComoPerdida()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var inicio = DateTime.UtcNow.AddHours(36);
        var evento = await TestData.AddEventoAsync(database.Context, fechaInicio: inicio, fechaFin: inicio.AddHours(2));
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, EstadoReservaEnum.Confirmada, codigoReserva: "EV-000001");
        var service = new ReservaService(database.Context);

        var response = await service.CancelarAsync(reserva.ReservaId);

        Assert.Equal((long)EstadoReservaEnum.Perdida, response.EstadoReservaId);
        Assert.Equal("perdida", response.EstadoReserva);
    }

    [Theory]
    [InlineData(EstadoReservaEnum.Cancelada, "La reserva ya está cancelada", "La reserva ya estÃ¡ cancelada")]
    [InlineData(EstadoReservaEnum.Perdida, "La reserva ya está perdida", "La reserva ya estÃ¡ perdida")]
    public async Task CancelarAsync_CuandoReservaYaNoEsCancelable_LanzaExcepcion(
        EstadoReservaEnum estado,
        string expected,
        string expectedMojibake)
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context);
        var reserva = await TestData.AddReservaAsync(database.Context, evento.EventoId, 2, estado, codigoReserva: "EV-000001");
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CancelarAsync(reserva.ReservaId));

        TestAssert.MessageEquals(exception.Message, expected, expectedMojibake);
    }

    [Theory]
    [InlineData(EstadoReservaEnum.PendientePago)]
    [InlineData(EstadoReservaEnum.Confirmada)]
    [InlineData(EstadoReservaEnum.Perdida)]
    public async Task CrearAsync_CuandoHayReservasOcupandoDisponibilidad_RechazaSiExcedeCupo(EstadoReservaEnum estado)
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 10);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 8, estado, codigoReserva: estado == EstadoReservaEnum.Confirmada ? "EV-000001" : null);
        var service = new ReservaService(database.Context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 3
        }));

        Assert.Equal("No hay suficientes entradas disponibles", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_CuandoLaReservaCanceladaNoCuentaDisponibilidad_PermiteCrearNuevaReserva()
    {
        await using var database = await SqliteTestDatabase.CreateAsync();
        var evento = await TestData.AddEventoAsync(database.Context, capacidadMaxima: 10);
        await TestData.AddReservaAsync(database.Context, evento.EventoId, 8, EstadoReservaEnum.Cancelada);
        var service = new ReservaService(database.Context);

        var response = await service.CrearAsync(new CrearReservaRequest
        {
            EventoId = evento.EventoId,
            UsuarioId = 1,
            Cantidad = 3
        });

        Assert.Equal((long)EstadoReservaEnum.PendientePago, response.EstadoReservaId);
    }
}
