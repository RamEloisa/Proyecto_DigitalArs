using DigitalArs.Application.DTOs;
using DigitalArs.Application.Exceptions;
using DigitalArs.Application.Services;
using DigitalArs.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DigitalArs.API.Controllers;

[ApiController]
[Route("api/users")]
[Tags("Users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Lista usuarios paginados con filtros (nombre, email, rol, activo)")]
    [ProducesResponseType(typeof(PagedResultDto<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetAll(
        [FromQuery] UserQueryDto query,
        CancellationToken cancellationToken)
    {
        return Ok(await _users.GetPagedAsync(query, cancellationToken));
    }

    [HttpGet("alias/{alias}")]
    [EndpointSummary("Lista usuarios activos cuyo alias empieza con el texto indicado")]
    [ProducesResponseType(typeof(IReadOnlyList<UserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<UserLookupDto>>> SearchByAlias(
        string alias,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(alias) || alias.Trim().Length > 50)
        {
            return ValidationErrorResponseFactory.From(
                [new ValidationErrorDto("alias", "El alias es obligatorio y no puede superar los 50 caracteres.")]);
        }

        var users = await _users.SearchByAliasAsync(alias, cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Obtiene un usuario por id")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Crea un usuario y su cuenta en la misma transacción")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var created = await _users.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Actualiza datos de un usuario (no cambia la contraseña)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var updated = await _users.UpdateAsync(id, dto, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Baja lógica de un usuario (IsActive = false)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _users.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /*private BadRequestObjectResult InvalidRole() =>
        ValidationErrorResponseFactory.From(
            [new ValidationErrorDto("RoleId", "El rol no existe.")],
            HttpContext.TraceIdentifier);*/
}
