using CSLibrary.Unmanaged;
using static Common.Utility;

namespace CSUnitTest;

public class UnmanagedListTest
{
    private const int ELEMENT_CAPACITY = 100;
    private const int ADD_LOOP_COUNT = 100;
    private const int REMOVE_LOOP_COUNT = 100;
    private const int SEARCH_LOOP_COUNT = 100;

    private const int INITIAL_ELEMENT_CAPACITY = 10;
    
    [Test]
    public void ManualFixedListTest()
    {
        ManualFixedList<int> intList = new ManualFixedList<int>(ELEMENT_CAPACITY);
        
        Assert.That(intList.Count(), Is.EqualTo(0));

        AddTest(intList);
        
        PrintList(intList);
        
        SearchTest(intList);

        InsertTest(intList);
        
        RemoveTest(intList);
    }
    
    
    [Test]
    public void ManualResizableListTest()
    {
        ManualResizableList<int> intList = new ManualResizableList<int>(INITIAL_ELEMENT_CAPACITY);
        
        Assert.That(intList.Count(), Is.EqualTo(0));

        AddTest(intList);
        
        PrintList(intList);
        
        SearchTest(intList);

        InsertTest(intList);
        
        RemoveTest(intList);
    }
    
    
    void AddTest(CSLibrary.IList<int> list)
    {
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Add(i);
            Assert.That(list.Count(), Is.EqualTo(i+1));
            Assert.That(list.At(i), Is.EqualTo(i));
        }
    }

    void SearchTest(CSLibrary.IList<int> list)
    {
        for (int i = 0; i < SEARCH_LOOP_COUNT; i++)
        {
            var index = list.IndexOf(i);
            Assert.That(index, Is.EqualTo(i));
        }
    }

    void EnumeratorTest(IEnumerable<int> list)
    {
        var enumerator = list.GetEnumerator();
        int enumeratorIndex = 0;
        while (enumerator.MoveNext())
        {
            Assert.That(enumerator.Current, Is.EqualTo(enumeratorIndex++));
        }
    }
    
    void InsertTest(CSLibrary.IList<int> list)
    {
        list.Clear();
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Insert(i, 0);
            Assert.That(list.At(0), Is.EqualTo(i));
        }
    }
    
    void RemoveTest(CSLibrary.IList<int> list)
    {
        list.Clear();

        for (int i = 0; i < REMOVE_LOOP_COUNT; i++)
        {
            list.Add(i);
        }
        
        for (int i = 0; i < REMOVE_LOOP_COUNT; i++)
        {
            list.RemoveAt(0);
            Assert.That(list.Count(), Is.EqualTo(REMOVE_LOOP_COUNT-i-1));
            if (list.Count() >= 1)
                Assert.That(list.At(0), Is.EqualTo(i+1));
        }
    }
    
    void PrintList(CSLibrary.IList<int> list)
    {
        for (int i = 0; i < list.Count(); i++)
        {
            prints(list.At(i));
        }
        printl();
    }
}