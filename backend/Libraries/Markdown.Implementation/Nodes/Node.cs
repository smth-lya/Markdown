using Markdown.Core.AST;

namespace Markdown.Implementation.Nodes;

public abstract class Node : ISyntaxNode
{
    protected readonly List<ISyntaxNode> _childrens = new();
    public IReadOnlyList<ISyntaxNode> Childrens => _childrens;
    public virtual bool IsOpen => _childrens.Count == 0;
    public abstract bool IsSelfClosing { get; }

    protected abstract string Tag { get; }

    public virtual void AddChildrenFirst(ISyntaxNode node)
        => _childrens.Insert(0, node);
    public virtual void AddChildrensFirst(IEnumerable<ISyntaxNode> nodes)
    {
        foreach (var node in nodes)
            AddChildrenFirst(node);
    }
    public virtual void AddChildrenLast(ISyntaxNode node)
        => _childrens.Add(node);
    public virtual void AddChildrensLast(IEnumerable<ISyntaxNode> nodes)
        => _childrens.AddRange(nodes);

    public virtual string Render()
    {
        var text = string.Join("", Childrens.Select(node => node.Render()));

        if (string.IsNullOrEmpty(Tag))
        {
            if (string.IsNullOrEmpty(text))
                return ToString()!;

            return text;
        }

        if (string.IsNullOrEmpty(text))
            return ToString()!;

        return $"<{Tag}>{text}</{Tag}>";
    }
}