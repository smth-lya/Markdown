using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using MD.Infrastructure;
using MD.Application;

namespace MD.WebAPI;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        var uploadingResult = await _documentService.UploadDocumentAsync(file);

        if (!uploadingResult.IsSuccess)
            return BadRequest(string.Join(", ", uploadingResult.Errors));

        return Ok(new { DocumentId = uploadingResult.Value});
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadDocument(Guid id)
    {
        var downloadingResult = await _documentService.DownloadDocumentAsync(id);

        if (!downloadingResult.IsSuccess)
            return BadRequest(string.Join(", ", downloadingResult.Errors));

        var (stream, fileName) = downloadingResult.Value;

        return File(stream, "application/octet-stream", fileName);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        var deletingResult = await _documentService.DeleteDocumentAsync(id);

        if (!deletingResult.IsSuccess)
            return BadRequest(string.Join(", ", deletingResult.Errors));

        return NoContent();
    }
}