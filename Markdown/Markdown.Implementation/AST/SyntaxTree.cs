using Markdown.Core.AST;
using System.Collections;

namespace Markdown.Implementation.AST;

public class SyntaxTree : ISyntaxTree
{
    public ISyntaxNode Root => throw new NotImplementedException();

    public void AddNode(ISyntaxNode node)
    {

    }

    public IEnumerator<ISyntaxNode> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
