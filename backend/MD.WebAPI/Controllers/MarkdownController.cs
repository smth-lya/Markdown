using Markdown.Core.Processors;
using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace MD.WebAPI;

[ApiController]
[Route("api/markdown")]
[Authorize]
public class MarkdownController : ControllerBase
{
    private readonly IMarkdownProcessor _processor;
    private readonly IDocumentService _documentService;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    public MarkdownController(
        IMarkdownProcessor processor,
        IDocumentService documentService,
        IPermissionService permissionService,
        ICurrentUser currentUser)
    {
        _processor = processor;
        _documentService = documentService;
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    [HttpPost("convert")]
    public ActionResult<ConvertResponse> Convert([FromBody] ConvertRequest request)
    {
        var html = _processor.ConvertToHtml(request.Markdown);
        return Ok(new ConvertResponse(html));
    }

    [HttpGet("document/{id}/preview")]
    public async Task<ActionResult<ConvertResponse>> PreviewDocument(Guid id)
    {
        if (!await _permissionService.CheckAccessAsync(id, _currentUser.Id, AccessLevel.Read))
            return Forbid();

        var docResult = await _documentService.GetDocumentContentAsync(id);
        if (!docResult.IsSuccess)
            return NotFound();

        var html = _processor.ConvertToHtml(docResult.Value);
        return Ok(new ConvertResponse(html));
    }

    public record ConvertRequest(string Markdown);
    public record ConvertResponse(string Html);
}