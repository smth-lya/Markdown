namespace MD.Domain;

public class DocumentParticipant
{
    public Guid Guid { get; set; }
    public Guid UserId { get; set; }
    public Guid DocumentId { get; set; }
    public Role Role { get; set; }
    public User User { get; set; }
    public Document Document { get; set; }
}


public enum Role
{
    Editor,
    Viewer
}