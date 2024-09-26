using Markdown.Core.Renders;
using Markdown.Implementation.Renders;

namespace Markdown.UnitTests;

public class HtmlRendererTests
{
    private readonly IRenderer _renderer;

    public HtmlRendererTests()
    {
        _renderer = new HtmlRenderer();
    }

    [Fact]
    public void Render_ShouldReturnHtml()
    {
        // Arrange

        // Act

        // Assert
    }
}