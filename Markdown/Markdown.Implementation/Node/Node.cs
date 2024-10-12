using Markdown.Core.AST;
using Markdown.Core.Renders;
using Markdown.Implementation.Node;
using Markdown.Implementation.Parsers;
using Markdown.Implementation.Renders;
using System.Diagnostics;
using System.Xml.Linq;

namespace Markdown.Implementation.Node
{
    public class Node : ISyntaxNode, IRenderer
    {
        public NodeType Type { get; private set; }

        public List<ISyntaxNode> Childrens { get; private set; }

        public Node()
        {
            Childrens = new();
        }

        public void AddChildren(ISyntaxNode node)
        {
            Childrens.Add(node);
        }

        public virtual string Render()
        {
            return string.Join(' ', Childrens.Cast<Node>().Select(node => node.Render()));
        }
    }
}
public class HeaderNode : Node
{
    public int Level { get; }

    public HeaderNode(int level)
    {
        Level = level;
    }

    public override string Render()
    {
        return $"<h{Level}>{base.Render()}</h{Level}>";
    }
}
public class EmphasisNode : Node
{
    public string Text { get; }

    public EmphasisNode(string text)
    {
        Text = text;
    }

    public override string Render()
    {
        return $"<em>{base.Render()}</em>";
    }
}
public class StrongNode : Node
{
    public override string Render()
    {
        return $"<strong>{base.Render()}</strong>";
    }
}
public class ParagraphNode : Node
{
    public override string Render()
    {
        return $"<p>{base.Render()}</p>";
    }
}

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class TextNode : Node
{
    public string Content { get; }

    public TextNode(string content)
    {
        Content = content;
    }

    public override string Render()
    {
        if (Childrens.Count != 0)
            throw new InvalidOperationException();

        return Content;
    }

    private string? GetDebuggerDisplay()
    {
        return ToString();
    }
}

