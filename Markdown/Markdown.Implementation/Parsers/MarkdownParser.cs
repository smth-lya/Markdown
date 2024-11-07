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
                var lineSpan = line.AsSpan(); 

                var level = 0;
                var i = 0;

                while (i < line.Length && level <= 5 && line[i] == '#')
                {
                    level++;
                    i++;
                }

                if (level > 0 && level <= 5 && i < lineSpan.Length && line[i] == ' ')
                {
                    syntaxTree.ProcessNode(new HeaderNode(level));
                    ProcessLine(lineSpan[(i + 1)..], syntaxTree);
                   
                    syntaxTree.CloseUnmatchedNodesAsText();
                    
                    continue;
                }

                ProcessLine(lineSpan, syntaxTree);
                syntaxTree.CloseUnmatchedNodesAsText();
            }

            return syntaxTree;
        }

        private void ProcessLine(ReadOnlySpan<char> line, in SyntaxTree syntaxTree)
        {
            var position = 0;

            while (position < line.Length)
            {
                var currentChar = line[position];

                if (currentChar == '\\')
                    ProcessEscapeSequence(ref position, line, syntaxTree);
                else if (currentChar == '_')
                    ProcessUnderscoreSequence(ref position, line, syntaxTree);
                else
                {
                    int textEnd = position;

                    while (textEnd < line.Length && line[textEnd] != '\\' && line[textEnd] != '_')
                        textEnd++;

                    syntaxTree.ProcessNode(new TextNode(line.Slice(position, textEnd - position)));
                    position = textEnd;
                }
            }
        }

        private void ProcessEscapeSequence(ref int position, ReadOnlySpan<char> line, in SyntaxTree syntaxTree)
        {
            if (position + 1 < line.Length && (line[position + 1] == '_' || line[position + 1] == '\\'))
            {
                syntaxTree.ProcessNode(new TextNode(line.Slice(position + 1, 1)));
                position += 2;

                return;
            }

            syntaxTree.ProcessNode(new TextNode(line.Slice(position, 1)));
            position++;
        }
        private void ProcessUnderscoreSequence(ref int position, ReadOnlySpan<char> line, in SyntaxTree syntaxTree)
        {
            if (position + 1 < line.Length && line[position + 1] == '_')
            {
                syntaxTree.ProcessNode(new StrongNode());
                position += 2;

                return;
            }

            syntaxTree.ProcessNode(new EmphasisNode());
            position++;
        }
    }
}