using Markdown.Core.Renders;

namespace Markdown.Core.AST;

public interface ISyntaxNode : IRenderer
{
    /// <summary>
    /// Определяет, открыт ли текущий тег.
    /// </summary>
    /// 
    /// <returns> 
    /// true, если тег открыт и ожидает закрытия,
    /// что позволяет обработчику добавлять соответствующий закрывающий тег.
    /// </returns>
    bool IsOpen { get; }

    /// <summary>
    /// Указывает, является ли текущий тег самозакрывающимся.
    /// </summary>
    /// 
    /// <returns> 
    /// Если true, обработка не требует добавления закрывающего тега в стеке.
    /// </returns>
    bool IsSelfClosing { get; }

    IReadOnlyList<ISyntaxNode> Childrens { get; }

    void AddChildrenFirst(ISyntaxNode node);
    void AddChildrensFirst(IEnumerable<ISyntaxNode> nodes);

    void AddChildrenLast(ISyntaxNode node);
    void AddChildrensLast(IEnumerable<ISyntaxNode> nodes);
}
