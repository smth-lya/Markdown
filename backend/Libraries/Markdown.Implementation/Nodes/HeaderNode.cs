namespace Markdown.Implementation.Nodes;

public class HeaderNode : Node
{
    public int Level { get; }
    public override bool IsSelfClosing => true;
    protected override string Tag => $"h{Level}";
    
    public HeaderNode(int level)
    {
        Level = level;
    }

    public override string ToString() => new string('#', Level);    
}