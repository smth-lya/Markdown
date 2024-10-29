using Markdown.Implementation;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = "# Это заголовок";
            var excepted = "<strong>This is Bold Text</strong>";
            // Act
            var _processor = new MarkdownProcessor();
            _processor.TryConvertToHtml(input, out var result);
            Console.WriteLine(result);
        }
    }/*
    return base.Content(result, "text/html");
    public ContentResult Process([FromBody] string input)*/
}
