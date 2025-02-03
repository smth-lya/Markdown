using Ardalis.Result;
using MD.Domain;

namespace MD.Application;

public interface IPermissionService
{
    Task<Result> GrantAccessAsync(Guid documentId, Guid userId, AccessLevel level);
    Task<Result> RevokeAccessAsync(Guid documentId, Guid userId);
    Task<bool> CheckAccessAsync(Guid documentId, Guid userId, AccessLevel requiredLevel);
    Task<List<DocumentPermission>> GetDocumentPermissionsAsync(Guid documentId);
}

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IDocumentRepository _documentRepository;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IDocumentRepository documentRepository)
    {
        _permissionRepository = permissionRepository;
        _documentRepository = documentRepository;
    }

    public async Task<Result> GrantAccessAsync(Guid documentId, Guid userId, AccessLevel level)
    {
        if (await _permissionRepository.IsOwnerAsync(documentId, userId))
            Result.Conflict("Owner already has full access");

        var existing = await _permissionRepository.GetAsync(documentId, userId);

        if (existing != null)
        {
            existing.AccessLevel = level;
            await _permissionRepository.UpdateAsync(existing);
        }
        else
        {
            await _permissionRepository.AddAsync(new DocumentPermission
            {
                DocumentId = documentId,
                UserId = userId,
                AccessLevel = level
            });
        }

        return Result.Success();
    }

    public async Task<Result> RevokeAccessAsync(Guid documentId, Guid userId)
    {
        var permission = await _permissionRepository.GetAsync(documentId, userId);
        if (permission != null)
        {
            await _permissionRepository.DeleteAsync(permission);
        }

        return Result.Success();
    }

    public async Task<bool> CheckAccessAsync(Guid documentId, Guid userId, AccessLevel requiredLevel)
    {
        if (await _permissionRepository.IsOwnerAsync(documentId, userId))
            return true;

        return await _permissionRepository.HasAccessAsync(documentId, userId, requiredLevel);
    }

    public async Task<List<DocumentPermission>> GetDocumentPermissionsAsync(Guid documentId)
    {
        var result = await _documentRepository.GetDocumentWithPermissionsAsync(documentId);

        return result?.Value.Permissions ?? new List<DocumentPermission>();
    }
}