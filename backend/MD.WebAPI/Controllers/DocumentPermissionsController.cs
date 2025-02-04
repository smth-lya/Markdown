using MD.Domain;
using MD.Application;
using MD.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("api/documents")]
public class DocumentPermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    public DocumentPermissionsController(
        IPermissionService permissionService,
        ICurrentUser currentUser)
    {
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    [HttpPost("share")]
    public async Task<IActionResult> ShareDocument([FromBody] ShareRequest request)
    {
        if (!await _permissionService.CheckAccessAsync(request.DocumentId, _currentUser.Id, AccessLevel.Edit))
            return Forbid();

        var result = await _permissionService.GrantAccessAsync(
            request.DocumentId,
            request.UserId,
            request.AccessLevel
        );

        return result.IsSuccess ? NoContent() : BadRequest("Failed to share document");
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions(Guid documentId)
    {
        if (!await _permissionService.CheckAccessAsync(documentId, _currentUser.Id, AccessLevel.Edit))
            return Forbid();

        var permissions = await _permissionService.GetDocumentPermissionsAsync(documentId);
        return Ok(permissions);
    }

    [HttpDelete]
    public async Task<IActionResult> RevokePermission(Guid documentId, Guid userId)
    {
        if (!await _permissionService.CheckAccessAsync(documentId, _currentUser.Id, AccessLevel.Edit))
            return Forbid();

        await _permissionService.RevokeAccessAsync(documentId, userId);
        return NoContent();
    }
}
