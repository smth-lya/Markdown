using Markdown.Core.AST;
using Markdown.Core.Parsers;
using Markdown.Implementation.AST;
using Markdown.Implementation.Renders;
using System.Text;

namespace Markdown.Implementation.Parsers;

public class MarkdownParser : IMarkdownParser
{
    public ISyntaxTree Parse(string sourceMD)
    {
        var syntaxTree = new SyntaxTree();

        ParagraphNode? pNode = null;

        foreach (var line in sourceMD.Split('\n'))
        {
            if (pNode == null && line != string.Empty)
            {
                pNode = new ParagraphNode();
            }

            if (line == string.Empty)
            {
                if (pNode != null)
                    syntaxTree.AddNode(pNode);
                
                pNode = null;
                continue;
            }

            int position = 0;

            if (line.StartsWith(Token.x2Asterisk))
            {
                position += 2;

                for (int i = 0; i < line.Length - 1; i++)
                {
                    if (line[i] == Token.Asterisk && line[i + 1] == Token.Asterisk)
                    {
                        var node = new StrongNode(line.Substring(2, i - 2));
                        pNode!.AddChildren(node);
                    }
                }
            }
        }

        return syntaxTree;
    }

    private ISyntaxNode ParseInlineElements(string line)
    {
        int position = 0;
        var parsedText = new List<ISyntaxNode>();
        var builder = new StringBuilder();
        while (position < line.Length)
        {
            if (line[position] == '\\')
            {
                if (position + 1 < line.Length)
                {
                    builder.Append(line[position + 1]);
                    position += 2;
                }
            }
            else if (line[position] == '_' && position + 1 < line.Length && line[position + 1] == '_')
            {
                if (builder.Length > 0)
                {
                    parsedText.Add(new ParagraphNode(builder.ToString()));
                    builder.Clear();
                }
                position += 2;  // Пропускаем "__"
                int startPos = position;
                while (position + 1 < line.Length && !(line[position] == '_' && line[position + 1] == '_'))
                {
                    position++;
                }
                string strongText = line.Substring(startPos, position - startPos);
                parsedText.Add(new StrongNode(strongText));
                position += 2;
            }
            else if (line[position] == '_')  // Курсив
            {
                if (builder.Length > 0)
                {
                    parsedText.Add(new ParagraphNode(builder.ToString()));
                    builder.Clear();
                }
                position++;  // Пропускаем "_"
                int startPos = position;
                while (position < line.Length && line[position] != '_')
                {
                    position++;
                }
                string emphasisText = line.Substring(startPos, position - startPos);
                parsedText.Add(new EmphasisNode(emphasisText));
                position++;
            }
            else
            {
                builder.Append(line[position]);
                position++;
            }
        }

        if (builder.Length > 0)
        {
            parsedText.Add(new ParagraphNode(builder.ToString()));
        }

        // В простом случае возвращаем параграф
        return parsedText.Count == 1 ? parsedText[0] : new CompositeNode(parsedText);
    }

}

public static class Token
{
    public static string x2Asterisk => "**";
    public static char Asterisk => '*';
}

