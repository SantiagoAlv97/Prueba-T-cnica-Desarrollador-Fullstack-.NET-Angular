/*
Prueba técnica SQL
Script necesario para inicializar base de datos dbEventosVivos
Estrcutura y datos iniciales
*/

CREATE DATABASE dbEventosVivos;
GO

USE dbEventosVivos;
GO

/* VENUES */

CREATE TABLE Venues (
    VenueID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    Capacidad INT NOT NULL,
    Ciudad VARCHAR(50) NOT NULL,

    CONSTRAINT CK_Venues_Capacidad 
        CHECK (Capacidad > 0)
);

INSERT INTO Venues VALUES
('Auditorio Central', 200, 'Bogotá'),
('Sala Norte', 50, 'Bogotá'),
('Arena Sur', 500, 'Medellín');


/* TIPOS DE EVENTO */

CREATE TABLE TipoEvento (
    TipoEventoID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Descripcion VARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO TipoEvento VALUES
('conferencia'),
('taller'),
('concierto');


/* ESTADOS DE EVENTO */

CREATE TABLE EstadoEvento (
    EstadoEventoID BIGINT NOT NULL PRIMARY KEY,
    Descripcion VARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO EstadoEvento (EstadoEventoID, Descripcion) VALUES
(1, 'activo'),
(2, 'cancelado'),
(3, 'completado');


/* ESTADOS DE RESERVA */

CREATE TABLE EstadoReserva (
    EstadoReservaID BIGINT NOT NULL PRIMARY KEY,
    Descripcion VARCHAR(100) NOT NULL UNIQUE
);

INSERT INTO EstadoReserva VALUES
(1, 'pendiente_pago'),
(2, 'confirmada'),
(3, 'cancelada'),
(4, 'perdida');


/* ROLES */

CREATE TABLE Roles (
    RolID BIGINT NOT NULL PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO Roles VALUES
(1, 'cliente'),
(2, 'administrador');


/* GESTIÓN USUARIOS */

CREATE TABLE Usuarios (
    UsuarioID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    GoogleID VARCHAR(100) NOT NULL,
    Email VARCHAR(254) NOT NULL,
    Nombre VARCHAR(150) NOT NULL,
    FotoUrl VARCHAR(500) NULL,
    RolID BIGINT NOT NULL DEFAULT 1,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaUltimoAcceso DATETIME2 NULL,

    CONSTRAINT UQ_Usuarios_GoogleID 
        UNIQUE (GoogleID),

    CONSTRAINT UQ_Usuarios_Email 
        UNIQUE (Email),

    CONSTRAINT FK_Usuarios_Roles 
        FOREIGN KEY (RolID) REFERENCES Roles(RolID)
);


/* GESTIÓN EVENTOS */

CREATE TABLE Eventos (
    EventoID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Titulo VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    VenueID BIGINT NOT NULL,
    TipoEventoID BIGINT NOT NULL,
    CapacidadMaxima INT NOT NULL,
    FechaInicio DATETIME2 NOT NULL,
    FechaFin DATETIME2 NOT NULL,
    PrecioEntrada DECIMAL(18,2) NOT NULL,
    EstadoEventoID BIGINT NOT NULL DEFAULT 1, -- 1 = activo

    CONSTRAINT FK_Eventos_Venues 
        FOREIGN KEY (VenueID) REFERENCES Venues(VenueID),

    CONSTRAINT FK_Eventos_TipoEvento 
        FOREIGN KEY (TipoEventoID) REFERENCES TipoEvento(TipoEventoID),

    CONSTRAINT FK_Eventos_EstadoEvento 
        FOREIGN KEY (EstadoEventoID) REFERENCES EstadoEvento(EstadoEventoID),

    CONSTRAINT CK_Eventos_Titulo 
        CHECK (LEN(Titulo) BETWEEN 5 AND 100),

    CONSTRAINT CK_Eventos_Descripcion 
        CHECK (LEN(Descripcion) BETWEEN 10 AND 500),

    CONSTRAINT CK_Eventos_CapacidadMaxima 
        CHECK (CapacidadMaxima > 0),

    CONSTRAINT CK_Eventos_Fechas 
        CHECK (FechaFin > FechaInicio),

    CONSTRAINT CK_Eventos_Precio 
        CHECK (PrecioEntrada > 0)
);


/* GESTION RESERVAS */

CREATE TABLE Reservas (
    ReservaID BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EventoID BIGINT NOT NULL,
    UsuarioID BIGINT NOT NULL,
    Cantidad INT NOT NULL,
    NombreComprador VARCHAR(150) NOT NULL,
    EmailComprador VARCHAR(254) NOT NULL,
    EstadoReservaID BIGINT NOT NULL DEFAULT 1, -- 1 = pendiente_pago
    CodigoReserva VARCHAR(20) NULL,
    FechaReserva DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FechaPago DATETIME2 NULL,
    FechaCancelacion DATETIME2 NULL,

    CONSTRAINT FK_Reservas_Eventos 
        FOREIGN KEY (EventoID) REFERENCES Eventos(EventoID),

    CONSTRAINT FK_Reservas_Usuarios 
        FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID),

    CONSTRAINT FK_Reservas_EstadoReserva 
        FOREIGN KEY (EstadoReservaID) REFERENCES EstadoReserva(EstadoReservaID),

    CONSTRAINT CK_Reservas_Cantidad 
        CHECK (Cantidad > 0)
);


/* ÍNDICE ÚNICO PARA CÓDIGO DE RESERVA CONFIRMADA */
CREATE UNIQUE INDEX IX_Reservas_CodigoReserva
ON Reservas (CodigoReserva)
WHERE CodigoReserva IS NOT NULL;


/* ÍNDICES PARA BUSQUEDAS */

CREATE INDEX IX_Eventos_VenueID 
ON Eventos (VenueID);

CREATE INDEX IX_Eventos_FechaInicio 
ON Eventos (FechaInicio);

CREATE INDEX IX_Reservas_EventoID 
ON Reservas (EventoID);

CREATE INDEX IX_Reservas_UsuarioID 
ON Reservas (UsuarioID);