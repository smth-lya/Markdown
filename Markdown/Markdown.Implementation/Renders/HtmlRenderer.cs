using Markdown.Core.AST;
using Markdown.Core.Processors;
using Markdown.Core.Renders;
using Markdown.Implementation.Parsers;
using System.Text;

namespace Markdown.Implementation.Renders;

public class HtmlRenderer : IRenderer
{
    private StringBuilder _htmlBuilder = new StringBuilder();

    public string GetHtml()
    {
        return _htmlBuilder.ToString();
    }

    public void RenderHeader(HeaderNode node)
    {
        _htmlBuilder.Append($"<h{node.Level}>{node.Text}</h{node.Level}>");
    }

    public void RenderEmphasis(EmphasisNode node)
    {
        _htmlBuilder.Append($"<em>{node.Text}</em>");
    }

    public void RenderStrong(StrongNode node)
    {
        _htmlBuilder.Append($"<strong>{node.Text}</strong>");
    }

    public void RenderParagraph(ParagraphNode node)
    {
        _htmlBuilder.Append($"<p>{node.Text}</p>");
    }

    public HtmlRenderer Render(ISyntaxTree syntaxTree)
    {
        foreach (var node in syntaxTree)
        {
            Render(node);
        }

        return this;
    }

    private void Render(ISyntaxNode syntaxNode)
    {
        if (syntaxNode.Childrens == null || syntaxNode.Childrens.Count() == 0)
        {
            if (syntaxNode is StrongNode strongNode)
                RenderStrong(strongNode);

            else if (syntaxNode is ParagraphNode paragraphNode)
                RenderParagraph(paragraphNode);

            else if (syntaxNode is TextNode textNode)
                Render(textNode); 
            
            return;
        }

        foreach (var node in syntaxNode.Childrens)
        {
            Render(node);
        }
    }
}

public class CompositeNode : ISyntaxNode
{
    private List<ISyntaxNode> _children;

    public CompositeNode(List<ISyntaxNode> children)
    {
        _children = children;
    }

    public void Render(HtmlRenderer renderer)
    {
        foreach (var child in _children)
        {
            child.Render(renderer);
        }
    }
}
