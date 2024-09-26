namespace Markdown.Core.Processors;

public interface IMarkdownProcessor
{
    bool TryConvertToHtml(string markdownm, out string htmlCode);
}


