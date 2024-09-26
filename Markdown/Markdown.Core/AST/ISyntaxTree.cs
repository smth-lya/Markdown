namespace Markdown.Core.AST;

public interface ISyntaxTree : IEnumerable<INode>
{
    INode Root { get; }
}
