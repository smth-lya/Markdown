using Microsoft.AspNetCore.Http;

namespace MD.Domain;

public interface IDocumentRepository
{
    Task<Document> GetByIdAsync(Guid id);
    Task<Document> CreateAsync(Document document, IFormFile file);
    Task DeleteAsync(Guid id);
}
