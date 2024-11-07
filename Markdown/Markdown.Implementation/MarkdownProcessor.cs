using Markdown.Core.Processors;
using Markdown.Implementation.Parsers;

namespace Markdown.Implementation;

public class MarkdownProcessor : IMarkdownProcessor
{
    public void ConvertToHtml(string markdownm, out string htmlCode)
    {
        var parser = new MarkdownParser();
        var tree = parser.Parse(markdownm);
        htmlCode = tree.Root.Render();
    }
}