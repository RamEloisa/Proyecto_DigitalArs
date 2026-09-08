# DigitalArs

Billetera virtual desarrollada como proyecto integrador para Aceleración Tech Río Negro by Alkemy. Permite a los usuarios
registrarse, autenticarse y gestionar sus operaciones financieras de forma segura, incluyendo la consulta de cuentas y movimientos, depósitos, transferencias y constitución de plazos fijos.

## Descripción

DigitalArs es una aplicación web compuesta por una API REST desarrollada en .NET y un frontend desarrollado en React.

La aplicación ofrece las siguientes funcionalidades principales:

- Autenticación y autorización mediante JWT (login).
- Gestión de usuarios con roles (Admin / User).
- Gestión de cuenta asociada a cada usuario (relación 1:1), con saldo y movimientos.
- Registro de transacciones (depósitos y transferencias entre cuentas).
- Constitución y consulta de inversión en plazo fijo.
- Notificaciones para informar al usuario sobre eventos relevantes de su cuenta y operaciones.
- Panel de administración con CRUD de usuarios, disponible solo para el rol Admin.
- Autogestión de perfil, permitiendo a cada usuario consultar y actualizar sus propios datos.
- Documentación de la API mediante Swagger/OpenAPI.
- Colección de Postman para facilitar la prueba de los endpoints.
- Validación de datos y manejo global de errores mediante mecanismos de validación y middleware.
- Test unitarios para validar lógica de negocio de los principales servicios de la aplicación.

## Stack tecnológico

**Backend**
- .NET 10 / C#
- ASP.NET Core Web API
- Entity Framework Core (Code First) + SQL Server
- Autenticación JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- BCrypt (`BCrypt.Net-Next`) para hasheo de contraseñas
- Mapster para mapeo entidad ↔ DTO
- FluentValidation para validación de requests
- Swagger / OpenAPI para documentación interactiva

**Frontend**
- React + Vite
- React Router DOM para navegación y rutas protegidas por sesión y por rol
- Axios para comunicación con la API, con interceptores para el manejo del token y respuestas 401
- ReactBits para componentes y recursos visuales
- Context API para gestión del estado de autenticación

**Arquitectura**

El backend utiliza una arquitectura basada en Clean Architecture, separando las responsabilidades de la aplicación en diferentes proyectos:

```
DigitalArs.API
    → Controllers, filtros de validación, middlewares y configuración de la API

DigitalArs.Application
    → DTOs, servicios de aplicación, validaciones, excepciones y mapeos

DigitalArs.Domain
    → Entidades, enums e interfaces que representan las reglas y contratos del dominio

DigitalArs.Infrastructure
    → Entity Framework Core, DbContext, repositorios y servicios de infraestructura,
      incluyendo JWT y hasheo de contraseñas con BCrypt

```

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 18 o superior (para el frontend)
- SQL Server (Express, Developer, o LocalDB)
- Visual Studio 2022 o superior (recomendado, para usar la Package Manager Console)
- Visual Studio Code o cualquier editor de tu preferencia para trabajar con frontend

## Instalación — Backend

1. Cloná este repositorio y abrí `DigitalArs.slnx` en Visual Studio.

2. Restaurá los paquetes NuGet (Visual Studio lo hace automáticamente al abrir la solución; si no hacé clic derecho en la solución → **Restaurar paquetes NuGet**).

3. Configurá tu cadena de conexión. **No modifiques el  `appsettings.json` compartido** — creá un `appsettings.Development.json` en `DigitalArs.API` con tu propia conexión local, por ejemplo:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=DigitalArs;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

   (Ajustá `Server=` según tu instalación de SQL Server. Por ejemplo: `localhost\\SQLEXPRESS` para SQL Server Express, `(localdb)\MSSQLLocalDB` para LocalDB, `localhost,1433` si tu instancia escucha en el puerto por defecto, etc.)

4. Abrí la **Consola del Administrador de paquetes** (menú *Herramientas → Administrador de paquetes NuGet → Consola del Administrador de paquetes*).
Verificá que el **Proyecto predeterminado** sea `DigitalArs.Infrastructure`.

5. Ejecutá la migración inicial: 

   ```powershell
   Add-Migration InitialCreate -Project DigitalArs.Infrastructure -StartupProject DigitalArs.API
   Update-Database -Project DigitalArs.Infrastructure -StartupProject DigitalArs.API
   ```
   La migración crea las tablas de la base de datos y ejecuta el seeding de los datos de prueba.

   Si al clonar el repositorio ya existen migraciones previas en el proyecto, no vuelvas a crear InitialCreate. En ese caso, ejecutá directamente:

   ```powershell
   Update-Database -Project DigitalArs.Infrastructure -StartupProject DigitalArs.API
   ```

6. Ejecutá la API (F5, o el botón ▶ en Visual Studio). Se accede a swagger desde:
   `https://localhost:{puerto}/swagger\index.html`

   Este puerto puede variar según la configuración de ejecución del proyecto.


## Relación Backend y Frontend

DigitalArs está compuesto por dos repositorios independientes:

* **Backend:** API REST desarrollada en .NET 10.
* **Frontend:** aplicación web desarrollada en React + Vite.

El frontend consume los endpoints expuestos por el backend mediante HTTP. Para ejecutar la aplicación completa, ambos proyectos deben estar funcionando y el frontend debe estar configurado para utilizar la URL correspondiente de la API.

La URL del backend se configura mediante la variable `VITE_API_URL` en el archivo `.env` del frontend.

> **Ejemplo:** `VITE_API_URL=https://localhost:7139/api`

El puerto mostrado es únicamente un ejemplo y puede variar según la configuración local de cada desarrollador.


## Credenciales de prueba

Al aplicar las migraciones, se cargan estos usuarios de ejemplo:

| Email                        | Contraseña  | Rol   |
|-------------------------------|-------------|-------|
| admin@digitalars.com          | Admin123!   | Admin |
| juan.perez@digitalars.com     | User123!    | User  |
| maria.gomez@digitalars.com    | User123!    | User  |

El usuario Admin permite probar las funcionalidades exclusivas de administración, mientras que los usuarios User permiten probar las funcionalidades disponibles para usuarios regulares.

Estas credenciales son **solo para desarrollo/testing**, nunca usar en producción. Las contraseñas se almacenan hasheadas con BCrypt; el texto plano de esta tabla existe únicamente para que el equipo pueda loguearse durante las pruebas.

**Relaciones:**
- `Role` 1:N `User` — un rol puede tener varios usuarios.
- `User` 1:1 `Account` — cada usuario tiene una única cuenta.
- `Account` 1:N `Transaction` — una cuenta puede registrar muchos movimientos.

El modelo completo y sus relaciones se encuentran representados en el siguiente diagrama entidad-relación:

![Diagrama Entidad-Relación](docs/ER-Diagram.png)

## Manejo de errores

La API utiliza un manejo centralizado de excepciones para devolver respuestas de error consistentes y evitar que detalles internos de la aplicación sean expuestos al cliente.Devuelve un formato único:

```json
{
  "statusCode": 400,
  "message": "Descripción del error",
  "errors": [{ "field": "Email", "message": "El email no es válido." }],
  "traceId": "0HN1..."
}
```

En producción, los errores 500 no exponen detalles internos ni stack trace; toda excepción queda registrada en el log del servidor junto con su `traceId`, para poder correlacionar un reporte de error del usuario con el log real.
Los errores de validación de los datos de entrada pueden utilizar una estructura específica de validación, según el endpoint y el tipo de error.


## Seguridad, variables de entorno y secretos

El proyecto evita almacenar credenciales, claves privadas y otros datos sensibles directamente en el repositorio.

### Backend

La consifuración general de la aplicación se encuentra en:

`DigitalArs.API/appsettings.json`

La configuración específica del entorno de desarrollo se encuentra en:

`DigitalArs.API/appsettings.Development.json`

La cadena de conexión utilizada durante el desarrollo local se configura en `appsettings.Development.json`.

La clave utilizada para firmar los tokens JWT es un dato sensible y debe configurarse mediante .NET User Secrets, evitando almacenarla directamente en los archivos `appsettings.json`.

Para configurar la clave JWT en un entorno de desarrollo:

```json
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "TU_CLAVE_JWT"
```

Las claves y configuraciones sensibles utilizadas por JWT deben configurarse de forma local y no deben publicarse en Git.

### Frontend

La URL de la API se configura mediante variables de entorno en el archivo `.env` del frontend:

```env
VITE_API_URL=https://localhost:{puerto}/api
```

El puerto depende de la configuración local del proyecto. Las variables `VITE_*` son accesibles desde el navegador, por lo que **no deben utilizarse para almacenar secretos o credenciales privadas**.

### Buenas prácticas

* No subir contraseñas, claves JWT, tokens ni cadenas de conexión con credenciales al repositorio.
* Utilizar **.NET User Secrets** para almacenar secretos durante el desarrollo local.
* Mantener los archivos de configuración locales fuera del control de versiones cuando contengan información sensible.
* Utilizar mecanismos seguros de gestión de secretos para entornos de producción.
* Las credenciales incluidas en la sección **Credenciales de prueba** son exclusivamente para desarrollo y testing.


## Documentación de la API

La API cuenta con documentación OpenAPI/Swagger y una colección de Postman para facilitar la exploración y prueba de los endpoints.

### Swagger

Documentación interactiva con api en ejecución en:

`http://localhost:<puerto>/swagger/index.html`

El documento OpenAPI se encuentra en:

`docs/openapi.json`

### Postman

La colección de Postman se encuentra en:

`docs/postman/DigitalArs.postman_collection.json`

La colección se genera a partir del documento OpenAPI mediante el script:

`docs/generate-postman.js`

Para regenerar la colección luego de realizar cambios en los endpoints, ejecutar desde la raíz del proyecto:

`node docs/generate-postman.js`

La colección generada puede importarse posteriormente en Postman para realizar pruebas de los endpoints.

Para utilizarla:

1. Ejecutar la API.
2. Importar `DigitalArs.postman_collection.json` en Postman.
3. Configurar `baseUrl` con la URL donde se ejecuta la API.
4. Ejecutar `Auth > Login`.
5. El token JWT obtenido se guarda automáticamente en la variable `token`.
6. Los demás endpoints utilizan automáticamente ese token para autenticarse.

## Tests unitarios

El proyecto incluye un proyecto independiente `DigitalArs.Tests`, desarrollado con **xUnit** y **Moq**, para validar la lógica de negocio de los servicios de aplicación.

Se cubren **10 casos de prueba**:

* **Login:** credenciales válidas, contraseña incorrecta y usuario inactivo.
* **Depósitos:** monto válido y monto superior al límite permitido.
* **Transferencias:** transferencia válida, saldo insuficiente, destino inexistente, transferencia a la propia cuenta y rollback ante errores.

Los tests utilizan mocks para aislar repositorios, `UnitOfWork` y servicios externos. También verifican el envío de notificaciones en tiempo real en las operaciones exitosas, sin depender de la base de datos real.

### Ejecución

Desde la raíz del proyect, ejecutar:

```bash
dotnet test DigitalArs.Tests
```

**Resultado actual: 10/10 tests aprobados.**
