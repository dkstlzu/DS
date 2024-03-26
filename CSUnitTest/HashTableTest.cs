using CSLibrary;
using static Common.Utility;

namespace CSUnitTest;

public class HashTableTest
{
    private const int ELEMENT_CAPACITY = 32;
    private const int SET_TEST_SIZE = 28;

    [Test]
    public void DirectAccessHashTableTest()
    {
        DirectAccessHashTable<string> intQueue = new DirectAccessHashTable<string>(0, ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
    
    [Test]
    public void CustomHashingHashTableTest()
    {
        CustomHashingHashTable<string> intQueue = new CustomHashingHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
    
    [Test]
    public void SeparateChainingHashTableTest()
    {
        ChainingHashTable<string> intQueue = new ChainingHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
        
    [Test]
    public void CoalescedChainingHashTableTest()
    {
        CoalescedHashTable<string> intQueue = new CoalescedHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
    
    [Test]
    public void LinearProbingHashTableTest()
    {
        LinearProbingHashTable<string> intQueue = new LinearProbingHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
        
    [Test]
    public void QuadraticProbingHashTableTest()
    {
        QuadraticProbingHashTable<string> intQueue = new QuadraticProbingHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
    
    [Test]
    public void DoubleHashingHashTable()
    {
        DoubleHashingHashTable<string> intQueue = new DoubleHashingHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
        
    [Test]
    public void CuckooHashTable()
    {
        CuckooHashTable<string> intQueue = new CuckooHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
        
    [Test]
    public void HopscotchHashTable()
    {
        HopscotchHashTable<string> intQueue = new HopscotchHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
        
    [Test]
    public void RobinHoodHashTable()
    {
        RobinHoodHashTable<string> intQueue = new RobinHoodHashTable<string>(ELEMENT_CAPACITY);
        
        HashTableValidationTest(intQueue);
    }
    
    private void HashTableValidationTest(IHashTable<string> hashTable)
    {
        hashTable.Clear();
        
        Assert.That(hashTable.IsEmpty(), Is.EqualTo(true));
        
        SetTest(hashTable);
        DuplicateSetTest(hashTable);
        RemoveTest(hashTable);
    }

    private void SetTest(IHashTable<string> hashTable)
    {
        hashTable.Clear();

        printl("HashTable SetTest");

        if (hashTable is DirectAccessHashTable<string> || hashTable is CustomHashingHashTable<string>)
        {
            // 이 둘은 hash값의 collision을 처리하지 않습니다.
            printl("Collision을 처리하지 않는 HashTable");
            for (int i = 0; i < SET_TEST_SIZE; i++)
            {
                hashTable.Set(i, $"James{i}");
                Assert.That(hashTable.Get(i), Is.EqualTo($"James{i}"));
                Assert.That(hashTable.Contains(i), Is.EqualTo(true));
            }
        }
        else
        {
            for (int i = 0; i < SET_TEST_SIZE; i++)
            {
                Assert.That(hashTable.Count(), Is.EqualTo(i));
                Assert.That(hashTable.Contains(i), Is.EqualTo(false));
                hashTable.Set(i, $"James{i}");
                Assert.That(hashTable.Count(), Is.EqualTo(i+1));
                Assert.That(hashTable.Get(i), Is.EqualTo($"James{i}"));
                Assert.That(hashTable.Contains(i), Is.EqualTo(true));
            }
        }
        
        printl($"Count : {hashTable.Count()}");
        PrintHashTable(hashTable);
    }

    private void DuplicateSetTest(IHashTable<string> hashTable)
    {
        hashTable.Clear();
        
        hashTable.Set(1, "Jira");
        
        Assert.That(hashTable.Count(), Is.EqualTo(1));
        Assert.That(hashTable.Get(1), Is.EqualTo("Jira"));
        
        hashTable.Set(1, "Jason");

        Assert.That(hashTable.Count(), Is.EqualTo(1)); // 크기 안늘어나야됨
        Assert.That(hashTable.Get(1), Is.EqualTo("Jason")); // 값은 바뀌어야됨
    }

    private void RemoveTest(IHashTable<string> hashTable)
    {
        hashTable.Clear();
        
        for (int i = 0; i < SET_TEST_SIZE; i++)
        {
            hashTable.Set(i, $"James{i}");
        }
        
        for (int i = 0; i < SET_TEST_SIZE; i++)
        {
            Assert.That(hashTable.Count(), Is.EqualTo(SET_TEST_SIZE-i));
            Assert.That(hashTable.Contains(i), Is.EqualTo(true));
            hashTable.Remove(i);
            Assert.That(hashTable.Count(), Is.EqualTo(SET_TEST_SIZE-i-1));
            Assert.That(hashTable.Contains(i), Is.EqualTo(false));
        }
        
        Assert.That(hashTable.IsEmpty, Is.EqualTo(true));
    }

    private void PrintHashTable(IHashTable<string> hashTable)
    {
        var enumerator = hashTable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current != null)
            {
                prints(enumerator.Current);
            }
            else
            {
                prints("Null");
            }
        }
        printl();
        
        enumerator.Dispose();
    }
}