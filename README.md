# CEIBA - Eventos Vivos

Aplicacion web para gestion y reserva de eventos. El repositorio esta dividido en tres partes:

- `eventos-vivos-ngweb`: frontend en Angular.
- `api`: backend en ASP.NET Core.
- `SQL`: script inicial de base de datos y datos semilla.

## Tecnologias utilizadas

### Frontend

- Angular 22
- TypeScript 6
- Angular Router
- RxJS
- Tailwind CSS 4
- Vitest

### Backend

- ASP.NET Core Web API sobre .NET 10
- Entity Framework Core 10
- SQL Server / LocalDB
- JWT Bearer Authentication
- Google Identity para login
- Swagger / OpenAPI
- xUnit

## Arquitectura elegida y justificación

Se implementó una arquitectura por capas en el backend y una organización por dominios en el frontend. Esta decisión permite separar responsabilidades, desacoplar la lógica de negocio del acceso a datos y de la exposición HTTP, y facilitar la creación de pruebas unitarias sobre los flujos principales del sistema.

### Backend

El backend está organizado en proyectos con responsabilidades claras:

- `eventos_vivos_api`: capa de exposición HTTP. Contiene los controladores REST, la configuración general de la aplicación, autenticación, autorización y documentación Swagger/OpenAPI.
- `eventos_vivos.BLL`: capa de lógica de negocio. Implementa las reglas de eventos, reservas, confirmación de pagos, cancelaciones, reportes, venues y autenticación.
- `eventos_vivos.DAL`: capa de acceso a datos. Contiene la configuración de Entity Framework Core, entidades, `DbContext` y persistencia contra SQL Server.
- `eventos_vivos.BDO`: capa de contratos compartidos. Contiene DTOs, enums y modelos utilizados entre las diferentes capas.
- `eventos-vivos.Tests`: proyecto de pruebas unitarias para validar los flujos de negocio principales.

Se eligió Entity Framework Core como ORM porque permite trabajar de forma productiva con SQL Server sin depender de procedimientos almacenados para la lógica principal de la aplicación. Para este proyecto, las operaciones requeridas no justificaban el uso de procedimientos almacenados complejos, ya que las reglas de negocio podían mantenerse de forma más clara en C# dentro de la capa BLL.

La arquitectura por capas se eligió para desacoplar las diferentes responsabilidades del sistema. Los controladores no contienen reglas de negocio; únicamente reciben las solicitudes HTTP, aplican autorización y delegan la operación a los servicios correspondientes. La BLL concentra las validaciones y reglas del dominio, mientras que la DAL se encarga de la persistencia. Esta separación facilita las pruebas unitarias, ya que permite validar la lógica sin depender directamente de la API ni de una base de datos real.

Además, esta estructura permite que el proyecto sea más mantenible y escalable en el futuro, ya que cada capa puede evolucionar con menor impacto sobre las demás.

Para la autenticación se decidió usar Google Identity. Esta opción simplifica la experiencia del usuario porque evita implementar un formulario tradicional de registro e inicio de sesión con contraseña. El usuario puede ingresar con su cuenta de Google y el sistema registra o actualiza sus datos básicos automáticamente. Técnicamente, también facilita la administración de usuarios, ya que el backend valida el token de Google, crea el usuario si no existe y luego genera un JWT propio para controlar el acceso a los endpoints protegidos y a las funciones administrativas.

### Frontend

El frontend sigue una estructura por responsabilidades:

- `src/app/core`: servicios, modelos, guards, interceptors y utilidades transversales.
- `src/app/shared`: layouts y componentes reutilizables.
- `src/app/features`: pantallas organizadas por dominio, como `public`, `auth`, `admin` y `account`.

Ademas, las rutas cargan componentes de forma diferida (`loadComponent`), lo que reduce el peso inicial de la aplicacion y mantiene aisladas las secciones publica y administrativa.

Esta organización permite separar la lógica común de las pantallas específicas y facilita el mantenimiento del proyecto. Las rutas usan carga diferida con `loadComponent`, lo que ayuda a reducir el peso inicial de la aplicación y mantiene aisladas las secciones pública, autenticada y administrativa.

En conjunto, la arquitectura seleccionada permite mantener el sistema ordenado, probar los flujos de negocio con mayor facilidad y separar claramente las responsabilidades entre presentación, lógica de negocio, persistencia y contratos compartidos.

## Instrucciones para ejecutar el proyecto localmente

### 1. Prerrequisitos

- .NET SDK 10
- Node.js con npm
- SQL Server LocalDB o una instancia de SQL Server compatible

Nota: la configuracion actual del backend usa `Server=(localdb)\MSSQLLocalDB;Database=dbEventosVivos;...`, por lo que en Windows funciona directo con LocalDB. Si se usa otra instancia, ajuste `api/eventos-vivos.api/appsettings.json`.

### 2. Crear la base de datos

Ejecute el script:

- `SQL/Estructura_inicial_base_de_datos_dbEventosVivos.sql`

Puede correrlo desde SQL Server Management Studio, Azure Data Studio o `sqlcmd`. El script:

- crea la base `dbEventosVivos`,
- crea tablas y restricciones,
- inserta datos iniciales de venues, tipos de evento, estados y roles.

### 3. Levantar la API

Desde la raiz del repositorio:

```powershell
cd api/eventos-vivos.api
dotnet restore
dotnet run --launch-profile https
```

La API queda disponible en:

- `https://localhost:7222`
- Swagger: `https://localhost:7222/swagger`

Si es la primera vez en su maquina y tienes problemas con HTTPS, ejecute:

```powershell
dotnet dev-certs https --trust
```

### 4. Levantar el frontend

En otra terminal:

```powershell
cd eventos-vivos-ngweb
npm install
npm start -- --proxy-config proxy.conf.json
```

La aplicacion queda disponible en:

- `http://localhost:4200`

Importante: el frontend usa `apiUrl: '/api'`, por lo que para desarrollo necesita el proxy hacia `https://localhost:7222`. Por eso el arranque local debe incluir `--proxy-config proxy.conf.json`.

### 5. Acceso y autenticacion

- Las consultas publicas de eventos y venues no requieren autenticacion.
- El login se hace con Google.
- El backend asigna rol administrador a los correos listados en `AdminEmails` dentro de `api/eventos-vivos.api/appsettings.json`.

Si cambia el puerto de la API, actualice tambien `eventos-vivos-ngweb/proxy.conf.json`.

## Comandos utiles

### Pruebas backend

```powershell
dotnet test api/eventos-vivos.Tests/eventosvivos.Tests.csproj
```

### Build frontend

```powershell
cd eventos-vivos-ngweb
npm run build
```

## Estado verificado

En este repositorio se validaron correctamente estos comandos:

- `dotnet test api/eventos-vivos.Tests/eventosvivos.Tests.csproj`
- `npm run build` en `eventos-vivos-ngweb`
