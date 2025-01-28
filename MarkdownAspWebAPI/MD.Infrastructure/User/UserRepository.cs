using Ardalis.Result;
using MD.Domain;
using Microsoft.EntityFrameworkCore;

namespace MD.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return user is null ? Result<User>.NotFound() : Result.Success(user);
    }

    public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken: cancellationToken);
        return user is null ? Result<User>.NotFound() : Result.Success(user);
    }

    public async Task<Result> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var loadedUser = await _dbContext.Users.FindAsync(user.Id);
        if (loadedUser is not null)
        {
            return Result.Error();
        }

        _dbContext.Add(user);
        int written = await _dbContext.SaveChangesAsync(cancellationToken);

        return written > 0 ? Result.Success() : Result.Error();
    }

    public async Task<Result> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var loadedUser = await _dbContext.Users.FindAsync([user.Id], cancellationToken: cancellationToken);
        if (loadedUser is null)
        {
            return Result.NotFound();
        }

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        var loadedUser = await _dbContext.Users.FindAsync([user.Id], cancellationToken: cancellationToken);
        if (loadedUser is null)
        {
            return Result.NotFound();
        }

        _dbContext.Remove(loadedUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
