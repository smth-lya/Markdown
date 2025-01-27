using Ardalis.Result;

namespace MD.Domain;

public interface IUserReadRepository
{
    Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}