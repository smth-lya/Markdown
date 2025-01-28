using System.Diagnostics.CodeAnalysis;

namespace MD.Domain;

public class User
{
    public Guid Id { get; init; }

    public required string Email { get; init; }
    public required string PasswordHash { get; init; }

    public User() { }

    [SetsRequiredMembers]
    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
    }
}
