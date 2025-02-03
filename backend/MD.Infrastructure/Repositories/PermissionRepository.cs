using Ardalis.Result;
using MD.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading;

namespace MD.Infrastructure;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PermissionRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Result> AddAsync(DocumentPermission permission)
    {
        var loadedPermission = await _dbContext.DocumentPermissions.FindAsync(permission.Id);
        if (loadedPermission is not null)
        {
            return Result.Error();
        }

        _dbContext.Add(permission);
        int written = await _dbContext.SaveChangesAsync();

        return written > 0 ? Result.Success() : Result.Error();
    }
    
    public async Task<Result> UpdateAsync(DocumentPermission permission)
    {
        var loadedPermission = await _dbContext.DocumentPermissions.FindAsync([permission.Id]);
        if (loadedPermission is null)
        {
            return Result.NotFound();
        }

        _dbContext.DocumentPermissions.Update(permission);
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DocumentPermission permission)
    {
        var loadedPermission = await _dbContext.DocumentPermissions.FindAsync([permission.Id]);
        if (loadedPermission is null)
        {
            return Result.NotFound();
        }

        _dbContext.Remove(loadedPermission);
        await _dbContext.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<DocumentPermission?> GetAsync(Guid documentId, Guid userId)
    {
        return await _dbContext.DocumentPermissions
            .FirstOrDefaultAsync(p => p.DocumentId == documentId && p.UserId == userId);
    }

    public async Task<bool> HasAccessAsync(Guid documentId, Guid userId, AccessLevel requiredLevel)
    {
        return await _dbContext.DocumentPermissions
            .AnyAsync(p => p.DocumentId == documentId &&
                          p.UserId == userId &&
                          p.AccessLevel >= requiredLevel);
    }

    public async Task<bool> IsOwnerAsync(Guid documentId, Guid userId)
    {
        return await _dbContext.Documents
            .AnyAsync(d => d.Id == documentId && d.OwnerId == userId);
    }

}
