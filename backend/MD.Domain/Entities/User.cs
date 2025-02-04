using System.Diagnostics.CodeAnalysis;

namespace MD.Domain;

public class User
{
    public Guid Id { get; init; }

    public required string Email { get; init; }
    public required string PasswordHash { get; init; }

    public List<Document> OwnedDocuments { get; set; } = new();
    public List<DocumentPermission> Permissions { get; set; } = new();

    public User() { }

    [SetsRequiredMembers]
    public User(string email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
    }
}
