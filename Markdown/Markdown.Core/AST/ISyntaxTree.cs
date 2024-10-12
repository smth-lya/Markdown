namespace Markdown.Core.AST;

public interface ISyntaxTree : IEnumerable<ISyntaxNode>
{
    ISyntaxNode Root { get; }
}
