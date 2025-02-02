using MD.Domain;
using Microsoft.AspNetCore.Http;

namespace MD.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public DocumentRepository(ApplicationDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Document> CreateAsync(Document document, IFormFile file)
    {
        // Сохраняем файл в MinIO
        document.MinioKey = await _fileStorage.UploadFileAsync(
            file.OpenReadStream(),
            Guid.NewGuid().ToString(),
            file.ContentType
        );

        await _context.Documents.AddAsync(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task DeleteAsync(Guid id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            await _fileStorage.DeleteFileAsync(document.MinioKey);
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    public Task<Document> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}