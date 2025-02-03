using Markdown.Implementation;
using Markdown.Implementation.Parsers;

namespace Markdown.UnitTests;

public class MarkdownProcessorTests
{
    private readonly MarkdownProcessor _processor = new MarkdownProcessor(new MarkdownToHtmlParser());

    [Fact]
    public void Italics_ShouldBeConvertedToEmTag()
    {
        // Arrange
        var input = "Текст, _окруженный с двух сторон_ одинарными символами подчерка";
        var expected = "Текст, <em>окруженный с двух сторон</em> одинарными символами подчерка";

        // Act
        var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Bold_ShouldBeConvertedToStrongTag()
    {
        // Arrange
        var input = "__Текст выделенный двумя символами__";
        var expected = "<strong>Текст выделенный двумя символами</strong>";

        // Act
        var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Escaping_ShouldPreventFormatting()
    {
        // Arrange
        var input = @"\_Экранированный текст\_ не должен выделяться курсивом";
        var expected = "_Экранированный текст_ не должен выделяться курсивом";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Escaping_ShouldBeVisibleIfNotUsedForFormatting()
    {
        // Arrange
        var input = @"Здесь сим\волы экранирования\ \должны остаться.";
        var expected = @"Здесь сим\волы экранирования\ \должны остаться.";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapedUnderscore_ShouldConvertToItalics()
    {
        // Arrange
        var input = @"\\_Это выделится курсивом_";
        var expected = "\\<em>Это выделится курсивом</em>";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BoldAndItalics_ShouldWorkTogether()
    {
        // Arrange
        var input = "__Полужирный и _курсив_ внутри__";
        var expected = "<strong>Полужирный и <em>курсив</em> внутри</strong>";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ItalicsInsideBold_ShouldNotWork()
    {
        // Arrange
        var input = "_Курсив и __полужирный внутри__ не работают_";
        var expected = "<em>Курсив и __полужирный внутри__ не работают</em>";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UnderscoreInNumbers_ShouldNotBeFormatted()
    {
        // Arrange
        var input = "Текст с цифрами_12_3";
        var expected = "Текст с цифрами_12_3";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ItalicsInTheMiddleOfWord_ShouldWork()
    {
        // Arrange
        var input = "Выделение в _нач_але и сер_еди_не";
        var expected = "Выделение в <em>нач</em>але и сер<em>еди</em>не";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UnpairedUnderscores_ShouldNotBeFormatted()
    {
        // Arrange
        var input = "Это непарные_ символы";
        var expected = "Это непарные_ символы";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Heading_ShouldBeConvertedToH1Tag()
    {
        // Arrange
        var input = "# Это заголовок";
        var expected = "<h1>Это заголовок</h1>";// Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HeadingWithFormatting_ShouldWork()
    {
        // Arrange
        var input = "# Заголовок с _разными_ символами";
        var expected = "<h1>Заголовок с <em>разными</em> символами</h1>";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EmptyUnderscores_ShouldRemain()
    {
        // Arrange
        var input = "____";
        var expected = "____";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void MixedBoldAndItalics_ShouldNotBeFormatted()
    {
        // Arrange
        var input = "пересечение __двойных и одинарных_ подчерков";
        var expected = "пересечение __двойных и одинарных_ подчерков";

        // Act
         var result = _processor.ConvertToHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }
}