# DigitalArs — Datos de prueba (Seed)

Al aplicar las migraciones, la base de datos se precarga automáticamente con los
siguientes datos, mediante `HasData()` en las clases de `Persistence/Configurations`
(`RoleConfiguration`, `UserConfiguration`, `AccountConfiguration`).

## Roles

| Id | Nombre |
|----|--------|
| 1  | Admin  |
| 2  | User   |

## Usuarios de prueba

Estas credenciales son **para entorno de desarrollo/testing**.

| Email                        | Contraseña | Rol   | Alias             |
|-------------------------------|------------|-------|-------------------|
| admin@digitalars.com          | Admin123!  | Admin | admin.digitalars  |
| juan.perez@digitalars.com     | User123!   | User  | juan.perez        |
| maria.gomez@digitalars.com    | User123!   | User  | maria.gomez       |

Las contraseñas se almacenan **hasheadas con bcrypt** en la columna
`Password_Hasheada`. El texto plano de esta tabla es únicamente para que el
equipo pueda loguearse.

## Cuentas de prueba

Relación 1:1 con cada usuario, todas con saldo inicial distinto de cero.

| Cuenta         | Usuario | Saldo inicial |
|----------------|---------|----------------|
| Cuenta Admin   | Admin   | $10.000        |
| Cuenta Juan    | Juan    | $5.000         |
| Cuenta Maria   | Maria   | $7.500         |

## Cómo aplicar el seed / Consola de paquetes NuGet

Si es la primera vez que se crea la base (si ya existía podemos usar el mismo comando o solo el Update):

```bash
Add-Migration InitialCreate -Project DigitalArs.Infrastructure -StartupProject DigitalArs.API
Update-Database -Project DigitalArs.Infrastructure -StartupProject DigitalArs.API
```

## Cómo aplicar el seed / Terminal

Si es la primera vez que se crea la base (si ya existía podemos usar el mismo comando o solo el Update):

```bash
dotnet ef migrations add SeedInitialData --project DigitalArs.Infrastructure --startup-project DigitalArs.API
dotnet ef database update --project DigitalArs.Infrastructure --startup-project DigitalArs.API
```

## Configuración de conexión local

En el archivo `appsettings.json` se encuentra la configuración del servidor


## Tests unitarios

El proyecto incluye un proyecto independiente `DigitalArs.Tests`, desarrollado con **xUnit** y **Moq**, para validar la lógica de negocio de los servicios de aplicación.

Se cubren **10 casos de prueba**:

* **Login:** credenciales válidas, contraseña incorrecta y usuario inactivo.
* **Depósitos:** monto válido y monto superior al límite permitido.
* **Transferencias:** transferencia válida, saldo insuficiente, destino inexistente, transferencia a la propia cuenta y rollback ante errores.

Los tests utilizan mocks para aislar repositorios, `UnitOfWork` y servicios externos. También verifican el envío de notificaciones en tiempo real en las operaciones exitosas, sin depender de una base de datos real.

### Ejecución

```bash
dotnet test DigitalArs.Tests
```

**Resultado actual: 10/10 tests aprobados.**
