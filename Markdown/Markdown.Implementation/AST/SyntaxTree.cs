using Markdown.Core.AST;
using Markdown.Implementation.Nodes;
using System.Collections;
using System.Text;

namespace Markdown.Implementation.AST
{
    public class SyntaxTree : ISyntaxTree
    {
        public ISyntaxNode Root { get; }
        private readonly Stack<Node> _openNodes = new();

        public SyntaxTree()
        {
            Root = new ParagraphNode();
        }

        /// <summary>
        /// Метод для обработки узлов. Определяет, нужно ли завершить узел или добавить новый.
        /// Если такой узел уже существует в стеке, закрывает его и все промежуточные ноды превращает в текст.
        /// </summary>
        /// <param name="node">Текущая нода, которую встретили</param>
        public void ProcessNode(Node node)
        {
            if (TryCloseNode(node))
                return;

            AddNode(node);
        }

        /// <summary>
        /// Завершает все оставшиеся незакрытые ноды и превращает их в текстовые.
        /// </summary>
        public void CloseUnmatchedNodesAsText()
        {
            var sb = new StringBuilder();

            while (_openNodes.Count > 0)
            {
                var node = _openNodes.Pop();

                if (node.Childrens.Count != 0)
                    (Root as ParagraphNode)?.AddChildren(node);
                else
                    (Root as ParagraphNode)?.AddChildren(new TextNode(node.ToString()));
            }

            (Root as ParagraphNode)?.Childrens.Reverse();
        }

        /// <summary>
        /// Проверяет, есть ли в стеке нода того же типа, и закрывает её вместе с промежуточными нодами.
        /// Превращает промежуточные ноды в текст и добавляет их к закрывающейся ноде.
        /// </summary>
        /// <param name="node">Нода, которую необходимо закрыть</param>
        /// <returns>Возвращает true, если нода была найдена и закрыта</returns>
        private bool TryCloseNode(Node node)
        {
            var nodeIndex = FindOpenNodeIndex(node);

            if (nodeIndex == -1)
                return false;

            var sb = new StringBuilder();
            var collectedNodes = new List<Node>();

            for (int i = 0; i < nodeIndex; i++)
            {
                var currentNode = _openNodes.Pop();
                sb.Insert(0, currentNode.Render());
                collectedNodes.Add(currentNode);
            }

            var textNode = new TextNode(sb.ToString());

            _openNodes.Peek().AddChildren(textNode);
            
            return true;
        }

        /// <summary>
        /// Находит индекс первой ноды такого же типа в стеке.
        /// </summary>
        /// <param name="node">Нода, которую ищем в стеке</param>
        /// <returns>Индекс ноды или -1, если нода не найдена</returns>
        private int FindOpenNodeIndex(Node node)
        {
            if (node.GetType() == typeof(TextNode))
                return -1;

            var index = 0;
            foreach (var openNode in _openNodes)
            {
                if (openNode.Childrens.Count == 0 && openNode.GetType() == node.GetType())
                    return index;
                index++;
            }
            return -1;
        }

        private void AddNode(Node node)
        {
            _openNodes.Push(node);
        }

        public IEnumerator<ISyntaxNode> GetEnumerator()
        {
            yield return Root;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}