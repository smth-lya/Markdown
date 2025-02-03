using MD.Domain;
using MD.Application;
using MD.WebAPI;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/documents/{documentId}/permissions")]
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

    [HttpPost("share/{id}")]
    public async Task<IActionResult> ShareDocument(
        Guid id,
        [FromBody] ShareRequest request)
    {
        if (!await _permissionService.CheckAccessAsync(id, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        var result = await _permissionService.GrantAccessAsync(
            id,
            request.UserId,
            request.AccessLevel
        );

        return result.IsSuccess ? NoContent() : BadRequest("Failed to share document");
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions(Guid documentId)
    {
        if (!await _permissionService.CheckAccessAsync(documentId, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        var permissions = await _permissionService.GetDocumentPermissionsAsync(documentId);
        return Ok(permissions);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> RevokePermission(Guid documentId, Guid userId)
    {
        if (!await _permissionService.CheckAccessAsync(documentId, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        await _permissionService.RevokeAccessAsync(documentId, userId);
        return NoContent();
    }
}
