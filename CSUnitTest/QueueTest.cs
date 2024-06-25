using CSLibrary;
using static Common.Utility;

namespace CSUnitTest;

public class QueueTest
{
    private const int ELEMENT_CAPACITY = 100;
    private const int ENQUEUE_LOOP_COUNT = 100;
    private const int DEQUEUE_LOOP_COUNT = 100;

    private const int INITIAL_ELEMENT_CAPACITY = 10;

    [Test]
    public void FixedSizeQueueTest()
    {
        FixedSizeQueue<int> intQueue = new FixedSizeQueue<int>(ELEMENT_CAPACITY);
        
        QueueValidationTest(intQueue);
    }
    
    [Test]
    public void ResizableQueueTest()
    {
        ResizableQueue<int> intQueue = new ResizableQueue<int>(INITIAL_ELEMENT_CAPACITY);
        
        QueueValidationTest(intQueue);
    }
    
    [Test]
    public void LinkedListBasedQueueTest()
    {
        LinkedListBasedQueue<int> intQueue = new LinkedListBasedQueue<int>();
        
        QueueValidationTest(intQueue);
    }

    private void QueueValidationTest(IQueue<int> queue)
    {
        EnqueueTest(queue);
        DequeueTest(queue);
    }
    
    private void EnqueueTest(IQueue<int> queue)
    {
        queue.Clear();

        printl("Queue EnqueueTest");
        
        for (int i = 0; i < ENQUEUE_LOOP_COUNT; i++)
        {
            queue.Enqueue(i);
            Assert.That(queue.Count, Is.EqualTo(i+1));
            Assert.That(queue.Peek(), Is.EqualTo(0));
            prints(queue.Count);
        }
        printl();
    }
    
    private void DequeueTest(IQueue<int> queue)
    {
        queue.Clear();

        printl("Queue DequeueTest");

        for (int i = 0; i < DEQUEUE_LOOP_COUNT; i++)
        {
            queue.Enqueue(i);
        }
        
        for (int i = 0; i < DEQUEUE_LOOP_COUNT; i++)
        {
            var popped = queue.Dequeue();
            Assert.That(popped, Is.EqualTo(i));
            Assert.That(queue.Count, Is.EqualTo(DEQUEUE_LOOP_COUNT-1-i));
            prints(popped);
        }
        
        printl();
    }
}