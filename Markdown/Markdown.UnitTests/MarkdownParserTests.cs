using Markdown.Core.Parsers;
using Markdown.Implementation.Parsers;

namespace Markdown.UnitTests;

public class MarkdownParserTests
{
    private readonly IMarkdownParser _parser;

    public MarkdownParserTests()
    {
        _parser = new MarkdownParser();
    }

    [Fact]
    public void Parse_ShouldReturnDocument_WhenValidMarkdownProvided()
    {

    }

    [Fact]
    public void Parse_ShouldHandleEmptyInput()
    {
        // Arrange
        string markdown = "";

        // Act
        var result = _parser.Parse(markdown);

        // Assert
    }

    [Fact]
    public void Parse_ShouldThrowException_WhenInputIsNull()
    {
        // Arrange
        string? markdown = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _parser.Parse(markdown));
    }
}
