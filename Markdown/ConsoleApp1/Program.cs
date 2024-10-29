using Markdown.Implementation;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = "fdf s**This is * d *Bold Text**";
            var excepted = "<strong>This is Bold Text</strong>";
            // Act
            var _processor = new MarkdownProcessor();
            _processor.TryConvertToHtml(text, out var result);
            Console.WriteLine(result);
        }
    }
}
