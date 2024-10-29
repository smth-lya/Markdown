using Markdown.Core.Processors;
using Markdown.Core.Renders;
using Markdown.Implementation.Parsers;

namespace Markdown.Implementation;

public class MarkdownProcessor : IMarkdownProcessor
{
    public bool TryConvertToHtml(string markdownm, out string htmlCode)
    {
        var parser = new MarkdownParser();
        var tree = parser.Parse(markdownm);
        htmlCode = ((IRenderer)tree.Root).Render();

        return true;
    }
}

// https://github.com/lostmsu/TextPlainInputFormatter/blob/master/TextPlainInputFormatterForAspNetCore/TextPlainInputFormatter.cs
/*
[HttpPost]
[Consumes("text/plain")]
public ContentResult Process([FromBody] string input)*/