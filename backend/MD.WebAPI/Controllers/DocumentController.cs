using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using MD.Infrastructure;
using MD.Application;
using Ardalis.Result;

namespace MD.WebAPI;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    public DocumentsController(
        IDocumentService documentService,
        IPermissionService permissionService,
        ICurrentUser currentUser)
    {
        _documentService = documentService;
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        var uploadingResult = await _documentService.UploadDocumentAsync(file);

        if (!uploadingResult.IsSuccess)
            return BadRequest(string.Join(", ", uploadingResult.Errors));

        await _permissionService.GrantAccessAsync(
            uploadingResult.Value,
            _currentUser.Id,
            AccessLevel.Edit
        );

        return Ok(new { DocumentId = uploadingResult.Value});
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        if (!await _permissionService.CheckAccessAsync(id, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        var downloadingResult = await _documentService.DownloadDocumentAsync(id);

        if (!downloadingResult.IsSuccess)
            return BadRequest(string.Join(", ", downloadingResult.Errors));

        var (stream, fileName) = downloadingResult.Value;

        return File(stream, "application/octet-stream", fileName);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        if (!await _permissionService.CheckAccessAsync(id, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        var deletingResult = await _documentService.DeleteDocumentAsync(id);

        if (!deletingResult.IsSuccess)
            return BadRequest(string.Join(", ", deletingResult.Errors));

        return NoContent();
    }
}

public record ShareRequest(Guid UserId, AccessLevel AccessLevel);
