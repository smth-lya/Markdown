using Markdown.Core.AST;
using System.Text;

namespace Markdown.Implementation.Nodes;

public class TextNode : Node
{
    private readonly StringBuilder _content = new();

    public string Content => _content.ToString();
    public override bool IsSelfClosing => true;

    protected override string Tag => string.Empty;

    public TextNode(string? content)
        => _content.Append(content);
    public TextNode(ReadOnlySpan<char> content)
        => _content.Append(content);

    public override void AddChildrenFirst(ISyntaxNode node)
        => _content.Insert(0, node.Render());
    public override void AddChildrensFirst(IEnumerable<ISyntaxNode> nodes)
    {
        foreach (ISyntaxNode node in nodes)
            AddChildrenFirst(node);
    }

    public override void AddChildrenLast(ISyntaxNode node)
        => _content.Append(node.Render());
    public override void AddChildrensLast(IEnumerable<ISyntaxNode> nodes)
    {
        foreach (var node in nodes)
            AddChildrenLast(node);
    }

    public override string Render()
    {
        if (Childrens.Count != 0)
            throw new InvalidOperationException();

        return Content;
    }
    public override string ToString() 
        => Content;

    public bool IsOnlyDigits()
    {
        foreach (var c in Content)
        {
            if (!char.IsDigit(c))
                return false;
        }

        return true;
    }
}
