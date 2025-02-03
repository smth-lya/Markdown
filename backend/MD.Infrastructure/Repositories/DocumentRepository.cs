using Ardalis.Result;
using MD.Domain;
using Microsoft.EntityFrameworkCore;

namespace MD.Infrastructure;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DocumentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> AddAsync(Document document)
    {
        var loadedDocument = await _dbContext.Documents.FindAsync(document.Id);
        if (loadedDocument is not null)
        {
            return Result.Error("[Document Reposityry] {Create Async} Файл уже существует в бд");
        }

        _dbContext.Add(document);
        var written = await _dbContext.SaveChangesAsync();

        return written > 0 ? Result.Success() : Result.Error();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var loadedDocument = await _dbContext.Documents.FindAsync(id);
        if (loadedDocument is null)
        {
            return Result.NotFound("[Document Reposityry] {Delete Async} Файл не существует в бд");
        }

        _dbContext.Remove(loadedDocument);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Document>> GetByIdAsync(Guid id)
    {
        var document = await _dbContext.Documents.FirstOrDefaultAsync(x => x.Id == id);
        return document is null ? Result<Document>.NotFound() : Result.Success(document); 
    }
}