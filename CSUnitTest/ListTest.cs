using CSLibrary;
using static Common.Utility;

namespace CSUnitTest;

public class ListTest
{
    private const int ELEMENT_CAPACITY = 100;
    private const int ADD_LOOP_COUNT = 100;
    private const int REMOVE_LOOP_COUNT = 10;

    private const int INITIAL_ELEMENT_CAPACITY = 10;
    
    [Test]
    public void FixedListTest()
    {
        FixedSizeList<int> intList = new FixedSizeList<int>(ELEMENT_CAPACITY);
        
        Assert.That(intList.Count(), Is.EqualTo(0));
        
        ListValidationTest(intList);
    }

    [Test]
    public void ResizableListTest()
    {
        ResizableList<int> intList = new ResizableList<int>(INITIAL_ELEMENT_CAPACITY);
        
        Assert.That(intList.Count(), Is.EqualTo(0));

        ListValidationTest(intList);
    }

    [Test]
    public void SingeLinkedListTest()
    {
        SingleLinkedList<int> intList = new SingleLinkedList<int>();
        
        Assert.That(intList.IsEmpty(), Is.EqualTo(true));
        
        ListValidationTest(intList);
    }
    
    [Test]
    public void DoubleLinkedListTest()
    {
        DoubleLinkedList<int> intList = new DoubleLinkedList<int>();
        
        Assert.That(intList.IsEmpty(), Is.EqualTo(true));
        
        ListValidationTest(intList);
    }
    
    [Test]
    public void CircularSingleLinkedListTest()
    {
        CircularSingleLinkedList<int> intList = new CircularSingleLinkedList<int>();
        
        Assert.That(intList.IsEmpty(), Is.EqualTo(true));

        ListValidationTest(intList);
    }
    
    [Test]
    public void CircularDoubleLinkedListTest()
    {
        CircularDoubleLinkedList<int> intList = new CircularDoubleLinkedList<int>();
        
        Assert.That(intList.IsEmpty(), Is.EqualTo(true));

        ListValidationTest(intList);
    }

    void ListValidationTest(CSLibrary.IList<int> list)
    {
        AddTest(list);
        InsertTest(list);
        SearchTest(list);
        RemoveTest(list);
        RemoveAtTest(list);
        EnumeratorTest(list);
    }

    void AddTest(CSLibrary.IList<int> list)
    {
        list.Clear();
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Add(i);
            Assert.That(list.Count(), Is.EqualTo(i+1));
            Assert.That(list.At(i), Is.EqualTo(i));
        }
        
        printl("AddTest");
        PrintList(list);
    }

    void SearchTest(CSLibrary.IList<int> list)
    {
        list.Clear();
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Add(i);
        }
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            var index = list.IndexOf(i);
            Assert.That(index, Is.EqualTo(i));
        }
    }

    void EnumeratorTest(CSLibrary.IList<int> list)
    {
        list.Clear();
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Add(i);
        }
        
        var enumerator = list.GetEnumerator();
        int enumeratorIndex = 0;
        
        printl("Enumerator Test");
        while (enumerator.MoveNext())
        {
            prints(enumerator.Current);
            Assert.That(enumerator.Current, Is.EqualTo(enumeratorIndex++));
        }
        printl();
        
        enumerator.Dispose();
    }
    
    void InsertTest(CSLibrary.IList<int> list)
    {
        list.Clear();
        
        for (int i = 0; i < ADD_LOOP_COUNT; i++)
        {
            list.Insert(i, 0);
            Assert.That(list.Count(), Is.EqualTo(i+1));
            Assert.That(list.At(0), Is.EqualTo(i));
        }
        
        printl("InsertTest");
        PrintList(list);
    }
    
    void RemoveTest(CSLibrary.IList<int> list)
    {
        list.Clear();

        for (int i = 0; i < REMOVE_LOOP_COUNT/2; i++)
        {
            list.Add(i);
        }
        
        for (int i = REMOVE_LOOP_COUNT/2; i < REMOVE_LOOP_COUNT; i++)
        {
            list.Insert(i, 0);
        }
        
        printl("Remove Test");
        prints("0th : ");
        PrintList(list);
        
        for (int i = 0; i < REMOVE_LOOP_COUNT; i++)
        {
            list.Remove(i);
            Assert.That(list.Count(), Is.EqualTo(REMOVE_LOOP_COUNT-i-1));
            
            prints($"{i+1}th : ");
            PrintList(list);
        }
        
        Assert.That(list.Count(), Is.EqualTo(0));
    }

    void RemoveAtTest(CSLibrary.IList<int> list)
    {
        list.Clear();

        for (int i = REMOVE_LOOP_COUNT-1; i >= 0; i--)
        {
            list.Add(i);
        }
        
        printl("RemoveAt Test");
        prints("0th : ");
        PrintList(list);
        
        for (int i = REMOVE_LOOP_COUNT-1; i >= 0; i--)
        {
            
            list.RemoveAt(0);
            Assert.That(list.Count(), Is.EqualTo(i));
            if (list.Count() >= 1)
                Assert.That(list.At(0), Is.EqualTo(i-1));
            
            prints($"{i+1}th : ");
            PrintList(list);
        }
        
        Assert.That(list.Count(), Is.EqualTo(0));
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