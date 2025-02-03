using Ardalis.Result;

namespace MD.Domain;

public interface IPermissionRepository : IPermissionReadRepository
{
    Task<Result> AddAsync(DocumentPermission permission);
    Task<Result> UpdateAsync(DocumentPermission permission);
    Task<Result> DeleteAsync(DocumentPermission permission);
    
}

public interface IPermissionReadRepository
{
    Task<DocumentPermission?> GetAsync(Guid documentId, Guid userId);
    Task<bool> IsOwnerAsync(Guid documentId, Guid userId);
    Task<bool> HasAccessAsync(Guid documentId, Guid userId, AccessLevel requiredLevel);
}