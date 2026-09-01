using DigitalArs.Application.DTOs; // Lo que Swagger serializa (no las entidades)
using DigitalArs.Application.Services; // IRoleService usa IUnitOfWork por debajo
using Microsoft.AspNetCore.Mvc; // ApiController, ActionResult, HTTP codes

namespace DigitalArs.API.Controllers;

[ApiController] // Valida el body y arma respuestas de error automáticas
[Route("api/roles")] // Prefijo de todas las acciones de este controller
[Tags("Roles")] // Agrupa estos endpoints en Swagger
public class RolesController : ControllerBase
{
    private readonly IRoleService _roles; // Application, no DbContext

    public RolesController(IRoleService roles)
    {
        _roles = roles;
    }

    [HttpGet]
    [EndpointSummary("Lista todos los roles")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _roles.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Obtiene un rol por id")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roles.GetByIdAsync(id, cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    [EndpointSummary("Crea un rol (Admin, User, etc.)")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var created = await _roles.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // Location: /api/roles/{id}
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualiza el nombre de un rol")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        var updated = await _roles.UpdateAsync(id, dto, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Elimina un rol")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _roles.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
