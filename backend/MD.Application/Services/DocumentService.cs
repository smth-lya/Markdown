using Ardalis.Result;
using MD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MD.Application;

public interface IDocumentService
{
    Task<Result<Guid>> UploadDocumentAsync(IFormFile file);
    Task<Result<(Stream, string)>> DownloadDocumentAsync(Guid id);
    Task<Result> DeleteDocumentAsync(Guid id);
    
    Task<Result<string>> GetDocumentContentAsync(Guid id); 
}

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IDocumentRepository documentRepository, IFileStorage fileStorage, ICurrentUser currentUser, ILogger<DocumentService> logger)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<string>> GetDocumentContentAsync(Guid id)
    {
        var documentResult = await _documentRepository.GetByIdAsync(id);
        if (!documentResult.IsSuccess)
            return Result<string>.Error(string.Join(", ", documentResult.Errors));

        var contentResult = await _fileStorage.GetContentAsync(documentResult.Value.FileName);
        return contentResult.IsSuccess
            ? Result<string>.Success(contentResult.Value)
            : Result<string>.Error(string.Join(", ", contentResult.Errors));
    }

    public async Task<Result<Guid>> UploadDocumentAsync(IFormFile file)
    {
        var result = await _fileStorage.UploadFileAsync(file);

        if (!result.IsSuccess)
        {
            return Result.Error(string.Join(", ", result.Errors));
        }
        var fileName = result.Value;

        var document = new Document()
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalName = file.FileName,
            OwnerId = _currentUser.Id,
        };

        result = await _documentRepository.AddAsync(document);

        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));
        _logger.LogInformation(new string('I', 100));

        return document.Id;
    }

    public async Task<Result<(Stream, string)>> DownloadDocumentAsync(Guid id)
    {
        var result = await _documentRepository.GetByIdAsync(id);
        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));

        var document = result.Value;
        Console.WriteLine(result.Value.FileName);
        var streamResult = await _fileStorage.DownloadFileAsync(document.FileName);
        _logger.LogInformation("aaaaaaaaaaaaaaddddddddddddddddFFFFFFFFFFFFFFFd");

        if (!streamResult.IsSuccess)
            return Result.Error(string.Join(", ", streamResult.Errors));

        return Result.Success((streamResult.Value, document.OriginalName));
    }
    
    public async Task<Result> DeleteDocumentAsync(Guid id)
    {
        var result = await _documentRepository.GetByIdAsync(id);
        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));

        var document = result.Value;

        var deleteFSResult = await _fileStorage.DeleteFileAsync(document.FileName);

        if (!deleteFSResult.IsSuccess)
            return Result.Error(string.Join(", ", deleteFSResult.Errors));

        var deleteDRResult = await _documentRepository.DeleteAsync(id);

        if (!deleteDRResult.IsSuccess)
            return Result.Error(string.Join(", ", deleteDRResult.Errors));

        return Result.Success();
    }
}
