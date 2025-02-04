namespace MD.Domain;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string OriginalName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid OwnerId { get; set; }
    public User Owner { get; set; }

    public List<DocumentPermission> Permissions { get; set; } = new();
}