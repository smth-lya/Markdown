using Markdown.Core.AST;
using Markdown.Core.Parsers;
using Markdown.Implementation.AST;
using Markdown.Implementation.Nodes;
using System.Text;

namespace Markdown.Implementation.Parsers
{
    public class MarkdownParser : IMarkdownParser
    {
        public ISyntaxTree Parse(string sourceMD)
        {
            var syntaxTree = new SyntaxTree();

            foreach (var line in sourceMD.Split('\n'))
            {
                var level = 0;
                var i = 0;
                while (i < line.Length && line[i] == '#')
                {
                    level++;
                    i++;
                }

                if (level > 0 && i < line.Length && line[i] == ' ')
                {
                    syntaxTree.ProcessNode(new HeaderNode(level));
                    ProcessLine(line.Substring(i + 1), syntaxTree);
                    
                    syntaxTree.CloseUnmatchedNodesAsText();
                    continue;
                }

                // Обработка остальных строк
                ProcessLine(line, syntaxTree);
                syntaxTree.CloseUnmatchedNodesAsText();
            }

            return syntaxTree;
        }

        private void ProcessLine(string line, in SyntaxTree syntaxTree)
        {
            var sb = new StringBuilder();
            var position = 0;

            for (var i = 0; i < line.Length;)
            {
                if (line[i] == '\\') // Экранирование
                {
                    if (i + 1 < line.Length && (line[i + 1] == '_' || line[i + 1] == '\\'))
                    {
                        sb.Append(line[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        sb.Append(line[i]);
                        i++;
                    }
                    continue;
                }

                if (line[i] == '_' && i + 1 < line.Length && line[i + 1] == '_') // Полужирный
                {
                    if (i - position > 0)
                        syntaxTree.ProcessNode(new TextNode(line.Substring(position, i - position)));

                    syntaxTree.ProcessNode(new StrongNode());
                    i += 2;
                    position = i;
                }
                else if (line[i] == '_') // Курсив
                {
                    if (i - position > 0)
                        syntaxTree.ProcessNode(new TextNode(line.Substring(position, i - position)));

                    syntaxTree.ProcessNode(new EmphasisNode());
                    i++;
                    position = i;
                }
                else if (char.IsWhiteSpace(line[i]) || char.IsDigit(line[i])) // Пробелы или цифры внутри подчерков не должны выделяться
                {
                    sb.Append(line[i]);
                    i++;
                }
                else
                {
                    sb.Append(line[i]);
                    i++;
                }
            }

            if (position < line.Length)
                syntaxTree.ProcessNode(new TextNode(sb.ToString()));
        }
    }
}