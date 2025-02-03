namespace MD.Domain;

public class DocumentPermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public AccessLevel AccessLevel { get; set; }

    public Document Document { get; set; }
    public User User { get; set; }
}

public enum AccessLevel
{
    Read = 1,
    Edit = 2
}