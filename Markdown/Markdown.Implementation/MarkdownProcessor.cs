using Markdown.Core.Processors;
using Markdown.Implementation.Parsers;

namespace Markdown.Implementation;

public class MarkdownProcessor : IMarkdownProcessor
{
    public bool TryConvertToHtml(string markdownm, out string htmlCode)
    {
        var parser = new MarkdownParser();
        parser.
    }
}

