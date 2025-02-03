namespace Markdown.Implementation.Nodes;

public class EmphasisNode : Node
{
    public override bool IsSelfClosing => false;
    protected override string Tag => "em";

    public override string Render()
    {
        if (_childrens.Count != 0 && _childrens.All(c => c is TextNode text && text.IsOnlyDigits()))
            return $"_{string.Join("", Childrens.Select(node => node.Render()))}_";

        foreach (StrongNode node in _childrens.Where(c => c is StrongNode))
            node.IsRawRender = true;

        return base.Render();
    }

    public override string ToString() => IsOpen ? "_" : "__";
}
