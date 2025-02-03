using Ardalis.Result;
using MD.Domain;
using Microsoft.AspNetCore.Http;

namespace MD.Application;

public interface IDocumentService
{
    Task<Result<Guid>> UploadDocumentAsync(IFormFile file);
    Task<Result<(Stream, string)>> DownloadDocumentAsync(Guid id);
    Task<Result> DeleteDocumentAsync(Guid id);
}

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorage _fileStorage;

    public DocumentService(IDocumentRepository documentRepository, IFileStorage fileStorage)
    {
        _documentRepository = documentRepository;
        _fileStorage = fileStorage;
    }

    public async Task<Result<Guid>> UploadDocumentAsync(IFormFile file)
    {
        var result = await _fileStorage.UploadFileAsync(file);

        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));

        var fileName = result.Value;

        var document = new Document()
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            OriginalName = file.FileName,
        };

        result = await _documentRepository.AddAsync(document);

        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));

        return document.Id;
    }

    public async Task<Result<(Stream, string)>> DownloadDocumentAsync(Guid id)
    {
        var result = await _documentRepository.GetByIdAsync(id);
        if (!result.IsSuccess)
            return Result.Error(string.Join(", ", result.Errors));

        var document = result.Value;

        var streamResult = await _fileStorage.DownloadFileAsync(document.FileName);

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
