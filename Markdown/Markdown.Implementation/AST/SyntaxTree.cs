using Markdown.Core.AST;
using Markdown.Core.Renders;
using Markdown.Implementation.Nodes;
using System.Collections;
using System.Text;

namespace Markdown.Implementation.AST
{
    public class SyntaxTree : ISyntaxTree
    {
        public ISyntaxNode Root { get; } = new BlankNode();

        private readonly Stack<ISyntaxNode> _openNodes = new();

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

            var list = new List<ISyntaxNode>() { new TextNode(string.Empty) };

            for (int i = 0; i < nodeIndex; i++)
            {
                var currentNode = _openNodes.Pop();
                list.Add(currentNode);
            }

            _openNodes.Peek().AddChildrensFirst(list);
            
            return true;
        }   
        private void AddNode(Node node)
            => _openNodes.Push(node);

        /// <summary>
        /// Завершает все оставшиеся незакрытые ноды и превращает их в текстовые.
        /// </summary>
        public void CloseUnmatchedNodesAsText()
        {
            ISyntaxNode node = new BlankNode();

            while (_openNodes.Count > 0)
            {
                var popedNode = _openNodes.Pop();

                if (popedNode.IsSelfClosing && popedNode is not TextNode)
                {
                    popedNode.AddChildrensLast(node.Childrens);
                    node = popedNode;

                    continue;
                }

                node.AddChildrenFirst(popedNode);
            }

            Root.AddChildrenFirst(node);
        }

        /// <summary>
        /// Находит индекс первой ноды такого же типа в стеке.
        /// </summary>
        /// <param name="node">Нода, которую ищем в стеке</param>
        /// <returns>Индекс ноды или -1, если нода не найдена</returns>
        private int FindOpenNodeIndex(Node node)
        {
            if (node.IsSelfClosing)
                return -1;

            var index = 0;
            foreach (var openNode in _openNodes)
            {
                if (openNode.IsOpen && !openNode.IsSelfClosing && openNode.GetType() == node.GetType())
                    return index;
                index++;
            }
            return -1;
        }
    }
}