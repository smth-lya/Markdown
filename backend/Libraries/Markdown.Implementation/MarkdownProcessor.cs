using Markdown.Core.Parsers;
using Markdown.Core.Processors;

namespace Markdown.Implementation;

public class MarkdownProcessor : IMarkdownProcessor
{
    private readonly IMarkdownParser _parser;

    public MarkdownProcessor(IMarkdownParser parser)
    {
        _parser = parser;
    }

    public string ConvertToHtml(string markdownm)
    {
        var tree = _parser.Parse(markdownm);
        var htmlCode = tree.Root.Render();

        return htmlCode;
    }
}