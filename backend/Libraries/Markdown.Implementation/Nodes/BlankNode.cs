namespace Markdown.Implementation.Nodes;

public class BlankNode : Node
{
    public override bool IsSelfClosing => true;
    protected override string Tag => string.Empty;

    public override string ToString() => string.Empty;
}