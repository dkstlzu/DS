namespace CSLibrary;

public interface ICollection
{
    void Clear();
    int Count { get; }
    bool IsEmpty();
}