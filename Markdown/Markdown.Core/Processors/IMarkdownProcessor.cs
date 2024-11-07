namespace Markdown.Core.Processors;

public interface IMarkdownProcessor
{
    void ConvertToHtml(string markdownm, out string htmlCode);
}