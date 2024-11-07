namespace Markdown.Implementation.Nodes;

public class StrongNode : Node
{
    public override bool IsSelfClosing => false;
    protected override string Tag => $"strong";

    public override string ToString() => IsOpen ? "__" : "____";

    public bool IsRawRender;

    public override string Render()
    {
        if (!IsRawRender)
            return base.Render();

        var text = string.Join("", Childrens.Select(node => node.Render()));

        return $"__{text}__";
    }
}
