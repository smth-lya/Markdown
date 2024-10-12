using Markdown.Implementation;

namespace Markdown.UnitTests;

public class MarkdownProcessorTests
{
    private readonly MarkdownProcessor _processor;

    public MarkdownProcessorTests()
    {
        _processor = new MarkdownProcessor();
    }

    [Fact]
    public void Convert_To_Html_Should_Return_Bold_Html()
    {
        // Arrange
        var text = "**This is Bold Text**";
        var excepted = "<strong>This is Bold Text</strong>";
        // Act

        _processor.TryConvertToHtml(text, out var result);

        // Assert
        Assert.Equal(excepted, result);
    }

    [Fact]
    public void ConvertToHtml_ShouldThrowException_WhenNullMarkdownProvided()
    {

    }

    [Fact]
    public void ConvertTo_ShouldReturnCorrectOutput_WithCustomRenderer()
    {

    }
}

