using Markdown.Core.AST;

namespace Markdown.Core.Parsers;

public interface IMarkdownParser
{
    ISyntaxTree Parse(string sourceMD);
}
