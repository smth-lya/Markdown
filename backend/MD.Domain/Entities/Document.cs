namespace MD.Domain;

public sealed class Document
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string OriginalName { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<DocumentParticipant> DocumentParticipants { get; set; } = new();

}
