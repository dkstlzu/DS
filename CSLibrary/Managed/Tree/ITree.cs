namespace CSLibrary;

/// <summary>
/// 기본적으로 모든 TreeNode가 재귀적으로 Tree의 RootNode로써 기능할 수 있어야 하고 이를 만족하기 위해 TreeNode는 Tree와 동일하게 취급됩니다.<br></br>
/// 하지만 Binary Search Tree, RB Tree, Heap등의 여러 Tree 구현이 목적하는 바가 다르고 이를 통일하여 하나의 인터페이스로 정의하는것은 무리가 있어보입니다.<br></br>
/// 실질적인 구현은 각각의 구현체를 참고하는 것이 옳아보입니다.
/// </summary>
public interface ITree<TRoot>
{
    void Add(TRoot childTree);
    void Remove(TRoot childTree);
    bool Contains(TRoot childTree);

}

public interface IArrayBasedTree : ITree<int>
{

}