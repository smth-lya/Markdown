using Markdown.Core.AST;

namespace Markdown.Core.Renders;

public interface IRenderer
{ 
    string Render(INode input);
}
