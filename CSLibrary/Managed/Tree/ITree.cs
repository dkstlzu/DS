namespace CSLibrary;

public interface ITree<T> : ICollection
{
    void Insert(T item);
    void Remove(T item);
    bool Contains(T item);
}

/// <summary>
/// Parameter로 받는 각 action이 true를 return하면 trevarsing을 멈춥니다.
/// </summary>
public interface ITreeTraversal<out T>
{
    void PreorderTraversal(Func<T?, bool> action);
    void InorderTraversal(Func<T?, bool> action);
    void PostorderTraversal(Func<T?, bool> action);
}