namespace Markdown.Core.AST;

public interface INode
{
    NodeType Type { get; }
    IEnumerable<INode> Children { get; }
    string Content {  get; }

    void AddChildren(INode node);
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
