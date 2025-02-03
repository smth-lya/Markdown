using Ardalis.Result;
namespace MD.Domain;

public interface IDocumentRepository : IDocumentReadRepository
{
    Task<Result> AddAsync(Document document);
    Task<Result> DeleteAsync(Guid id);
}
