
/*
SQL de pruebas internas para validar reglas de negocio desde postman
*/

/* 
1. TOMAR IDS EXISTENTES
*/

DECLARE @Activo BIGINT = (
    SELECT TOP 1 EstadoEventoID
    FROM EstadoEvento
    WHERE Descripcion = 'activo'
);

DECLARE @Conferencia BIGINT = (
    SELECT TOP 1 TipoEventoID
    FROM TipoEvento
    WHERE Descripcion = 'conferencia'
);

DECLARE @Taller BIGINT = (
    SELECT TOP 1 TipoEventoID
    FROM TipoEvento
    WHERE Descripcion = 'taller'
);

DECLARE @Concierto BIGINT = (
    SELECT TOP 1 TipoEventoID
    FROM TipoEvento
    WHERE Descripcion = 'concierto'
);

/* 
Toma venues existentes.
*/
DECLARE @Venue1 BIGINT;
DECLARE @Venue2 BIGINT;
DECLARE @Venue3 BIGINT;
DECLARE @Venue4 BIGINT;

;WITH V AS (
    SELECT 
        VenueID,
        ROW_NUMBER() OVER (ORDER BY VenueID) AS rn
    FROM Venues
)
SELECT @Venue1 = VenueID FROM V WHERE rn = 1;

;WITH V AS (
    SELECT 
        VenueID,
        ROW_NUMBER() OVER (ORDER BY VenueID) AS rn
    FROM Venues
)
SELECT @Venue2 = VenueID FROM V WHERE rn = 2;

;WITH V AS (
    SELECT 
        VenueID,
        ROW_NUMBER() OVER (ORDER BY VenueID) AS rn
    FROM Venues
)
SELECT @Venue3 = VenueID FROM V WHERE rn = 3;

;WITH V AS (
    SELECT 
        VenueID,
        ROW_NUMBER() OVER (ORDER BY VenueID) AS rn
    FROM Venues
)
SELECT @Venue4 = VenueID FROM V WHERE rn = 4;


/* 
2. VALIDACIONES
*/

IF @Activo IS NULL
    THROW 50001, 'No existe EstadoEvento activo.', 1;

IF @Conferencia IS NULL OR @Taller IS NULL OR @Concierto IS NULL
    THROW 50002, 'No existen todos los tipos de evento requeridos: conferencia, taller, concierto.', 1;

IF @Venue1 IS NULL
    THROW 50003, 'No existen venues registrados. Debes tener al menos un venue.', 1;

/* Si no hay suficientes venues, reutiliza el primero */
SET @Venue2 = ISNULL(@Venue2, @Venue1);
SET @Venue3 = ISNULL(@Venue3, @Venue1);
SET @Venue4 = ISNULL(@Venue4, @Venue1);

/* 
4. EVENTOS PARA PROBAR REGLAS
*/

/* Base para probar cruce de horario */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Base Cruce Horario')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Base Cruce Horario',
        'Evento usado para probar validaci�n de cruce de horarios en el mismo venue.',
        @Venue1,
        @Conferencia,
        40,
        '2026-08-20T18:00:00',
        '2026-08-20T21:00:00',
        60.00,
        @Activo
    );
END;

/* Capacidad baja para probar sobreventa */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Cupo Bajo')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Cupo Bajo',
        'Evento con capacidad baja para probar disponibilidad y sobreventa.',
        @Venue4,
        @Taller,
        5,
        '2026-09-05T10:00:00',
        '2026-09-05T12:00:00',
        40.00,
        @Activo
    );
END;

/* Precio mayor a 100: m�ximo 10 entradas por reserva */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Premium Alto Precio')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Premium Alto Precio',
        'Evento usado para probar la regla de m�ximo diez entradas por precio alto.',
        @Venue1,
        @Conferencia,
        50,
        '2026-09-06T16:00:00',
        '2026-09-06T18:00:00',
        150.00,
        @Activo
    );
END;

/* Menos de 24 horas: m�ximo 5 entradas */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Menos de 24 Horas')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Menos de 24 Horas',
        'Evento din�mico para probar l�mite de m�ximo cinco entradas si faltan menos de veinticuatro horas.',
        @Venue2,
        @Taller,
        30,
        DATEADD(HOUR, 10, SYSDATETIME()),
        DATEADD(HOUR, 13, SYSDATETIME()),
        70.00,
        @Activo
    );
END;

/* Menos de 1 hora: no debe permitir reservar */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Menos de 1 Hora')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Menos de 1 Hora',
        'Evento din�mico para probar que no se permitan reservas faltando menos de una hora.',
        @Venue2,
        @Taller,
        30,
        DATEADD(MINUTE, 30, SYSDATETIME()),
        DATEADD(HOUR, 2, SYSDATETIME()),
        70.00,
        @Activo
    );
END;

/* Evento vencido para probar RN-06 completado autom�tico */
IF NOT EXISTS (SELECT 1 FROM Eventos WHERE Titulo = 'Evento Para Completar Automatico')
BEGIN
    INSERT INTO Eventos
    (
        Titulo,
        Descripcion,
        VenueID,
        TipoEventoID,
        CapacidadMaxima,
        FechaInicio,
        FechaFin,
        PrecioEntrada,
        EstadoEventoID
    )
    VALUES
    (
        'Evento Para Completar Automatico',
        'Evento activo con fecha final vencida para probar cambio autom�tico a completado.',
        @Venue1,
        @Conferencia,
        30,
        DATEADD(HOUR, -3, SYSDATETIME()),
        DATEADD(HOUR, -1, SYSDATETIME()),
        30.00,
        @Activo
    );
END;


/* 
5. CONSULTA FINAL
*/

SELECT 
    e.EventoID,
    e.Titulo,
    te.Descripcion AS TipoEvento,
    v.Nombre AS Venue,
    v.Capacidad AS CapacidadVenue,
    e.CapacidadMaxima,
    e.FechaInicio,
    e.FechaFin,
    e.PrecioEntrada,
    ee.Descripcion AS Estado
FROM Eventos e
INNER JOIN TipoEvento te ON e.TipoEventoID = te.TipoEventoID
INNER JOIN Venues v ON e.VenueID = v.VenueID
INNER JOIN EstadoEvento ee ON e.EstadoEventoID = ee.EstadoEventoID
ORDER BY e.FechaInicio;