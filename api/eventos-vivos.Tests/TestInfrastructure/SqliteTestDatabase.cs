using eventos_vivos.BDO.Enums;
using eventosvivos.DAL.Entities;
using eventosvivos.DAL.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace eventos_vivos.Tests.TestInfrastructure;

internal sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private SqliteTestDatabase(SqliteConnection connection, EventosVivosDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public EventosVivosDbContext Context { get; }

    public static async Task<SqliteTestDatabase> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        connection.CreateFunction("sysutcdatetime", () => DateTime.UtcNow.ToString("O"));

        var options = new DbContextOptionsBuilder<EventosVivosDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new EventosVivosDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await SeedAsync(context);

        return new SqliteTestDatabase(connection, context);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private static async Task SeedAsync(EventosVivosDbContext context)
    {
        var now = DateTime.UtcNow;

        context.EstadoEventos.AddRange(
            new EstadoEvento { EstadoEventoId = (long)EstadoEventoEnum.Activo, Descripcion = "activo" },
            new EstadoEvento { EstadoEventoId = (long)EstadoEventoEnum.Cancelado, Descripcion = "cancelado" },
            new EstadoEvento { EstadoEventoId = (long)EstadoEventoEnum.Completado, Descripcion = "completado" });

        context.EstadoReservas.AddRange(
            new EstadoReserva { EstadoReservaId = (long)EstadoReservaEnum.PendientePago, Descripcion = "pendiente_pago" },
            new EstadoReserva { EstadoReservaId = (long)EstadoReservaEnum.Confirmada, Descripcion = "confirmada" },
            new EstadoReserva { EstadoReservaId = (long)EstadoReservaEnum.Cancelada, Descripcion = "cancelada" },
            new EstadoReserva { EstadoReservaId = (long)EstadoReservaEnum.Perdida, Descripcion = "perdida" });

        context.Roles.AddRange(
            new Role { RolId = 1, Nombre = "cliente" },
            new Role { RolId = 2, Nombre = "administrador" });

        context.TipoEventos.AddRange(
            new TipoEvento { TipoEventoId = 1, Descripcion = "concierto" },
            new TipoEvento { TipoEventoId = 2, Descripcion = "obra" });

        context.Venues.AddRange(
            new Venue { VenueId = 1, Nombre = "Venue Centro", Capacidad = 100, Ciudad = "Bogota" },
            new Venue { VenueId = 2, Nombre = "Venue Norte", Capacidad = 80, Ciudad = "Bogota" });

        context.Usuarios.AddRange(
            new Usuario
            {
                UsuarioId = 1,
                GoogleId = "google-1",
                Email = "cliente1@test.dev",
                Nombre = "Cliente Uno",
                RolId = 1,
                Activo = true,
                FechaCreacion = now
            },
            new Usuario
            {
                UsuarioId = 2,
                GoogleId = "google-2",
                Email = "cliente2@test.dev",
                Nombre = "Cliente Dos",
                RolId = 1,
                Activo = true,
                FechaCreacion = now
            });

        await context.SaveChangesAsync();
    }
}
