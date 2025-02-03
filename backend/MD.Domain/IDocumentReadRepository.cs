using Ardalis.Result;
namespace MD.Domain;

public interface IDocumentReadRepository
{
    Task<Result<Document>> GetByIdAsync(Guid id);
    Task<Result<Document>> GetDocumentWithPermissionsAsync(Guid documentId);
}