using eventos_vivos.BDO.DTOs.Eventos;
using eventos_vivos.BDO.Enums;
using eventosvivos.DAL.Entities;
using eventosvivos.DAL.Persistence;

namespace eventos_vivos.Tests.TestInfrastructure;

internal static class TestData
{
    public static CrearEventoRequest BuildValidEventoRequest(
        long venueId = 1,
        int capacidadMaxima = 50,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        decimal precioEntrada = 80m)
    {
        var inicio = fechaInicio ?? DateTime.UtcNow.AddDays(10);
        var fin = fechaFin ?? inicio.AddHours(3);

        return new CrearEventoRequest
        {
            Titulo = "Evento valido",
            Descripcion = "Descripcion valida para pruebas",
            VenueId = venueId,
            TipoEventoId = 1,
            CapacidadMaxima = capacidadMaxima,
            FechaInicio = inicio,
            FechaFin = fin,
            PrecioEntrada = precioEntrada
        };
    }

    public static async Task<Evento> AddEventoAsync(
        EventosVivosDbContext context,
        long venueId = 1,
        int capacidadMaxima = 50,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        decimal precioEntrada = 80m,
        EstadoEventoEnum estado = EstadoEventoEnum.Activo,
        string titulo = "Evento sembrado")
    {
        var inicio = fechaInicio ?? DateTime.UtcNow.AddDays(10);
        var fin = fechaFin ?? inicio.AddHours(3);

        var evento = new Evento
        {
            Titulo = titulo,
            Descripcion = "Descripcion valida para pruebas",
            VenueId = venueId,
            TipoEventoId = 1,
            CapacidadMaxima = capacidadMaxima,
            FechaInicio = inicio,
            FechaFin = fin,
            PrecioEntrada = precioEntrada,
            EstadoEventoId = (long)estado
        };

        context.Eventos.Add(evento);
        await context.SaveChangesAsync();
        return evento;
    }

    public static async Task<Reserva> AddReservaAsync(
        EventosVivosDbContext context,
        long eventoId,
        int cantidad,
        EstadoReservaEnum estado,
        long usuarioId = 1,
        string? codigoReserva = null)
    {
        var usuario = await context.Usuarios.FindAsync(usuarioId) ?? throw new InvalidOperationException("Usuario base no sembrado");

        var reserva = new Reserva
        {
            EventoId = eventoId,
            UsuarioId = usuarioId,
            Cantidad = cantidad,
            NombreComprador = usuario.Nombre,
            EmailComprador = usuario.Email,
            EstadoReservaId = (long)estado,
            CodigoReserva = codigoReserva,
            FechaReserva = DateTime.UtcNow
        };

        if (estado == EstadoReservaEnum.Confirmada)
        {
            reserva.FechaPago = DateTime.UtcNow;
        }

        if (estado == EstadoReservaEnum.Cancelada || estado == EstadoReservaEnum.Perdida)
        {
            reserva.FechaCancelacion = DateTime.UtcNow;
        }

        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();
        return reserva;
    }

    public static DateTime NextSaturdayAt(int hour, int minute = 0)
    {
        var date = DateTime.UtcNow.Date.AddDays(1);
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }

        return date.AddHours(hour).AddMinutes(minute);
    }
}
