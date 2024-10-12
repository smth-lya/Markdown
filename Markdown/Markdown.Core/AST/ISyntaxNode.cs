namespace Markdown.Core.AST;

public interface ISyntaxNode
{
    NodeType Type { get; }
    List<ISyntaxNode> Childrens { get; }
    /*string Content {  get; }*/

    void AddChildren(ISyntaxNode node);
}

public enum NodeType
{
    Text,
    Emphasis,     // <em>
    Strong,       // <strong>
    Heading,      // <h1>, <h2>,
    Paragraph,
    EscapeSymbol,
}
