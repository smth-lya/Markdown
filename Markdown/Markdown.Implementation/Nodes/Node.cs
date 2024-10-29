using Markdown.Core.AST;
using Markdown.Core.Renders;
using Markdown.Implementation.Nodes;
using Markdown.Implementation.Parsers;
using System.Diagnostics;
using System.Xml.Linq;

namespace Markdown.Implementation.Nodes
{
    public abstract class Node : ISyntaxNode, IRenderer
    {
        public NodeType Type { get; private set; }
        public List<ISyntaxNode> Childrens { get; init; }

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
            return string.Join("", Childrens.Cast<Node>().Select(node => node.Render()));
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

    public override string ToString()
    {
        return new string('#', Level);
    }
}
public class EmphasisNode : Node
{
    public override string Render()
    {
        return $"<em>{base.Render()}</em>";
    }

    public override string ToString()
    {
        return "*";
    }
}
public class StrongNode : Node
{
    public override string Render()
    {
        return $"<strong>{base.Render()}</strong>";
    }

    public override string ToString()
    {
        return "**";
    }
}
public class ParagraphNode : Node
{
    public override string Render()
    {
        return $"{base.Render()}";
    }

    public override string ToString()
    {
        return string.Empty;
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

    public override string ToString()
    {
        return Content;
    }
}

