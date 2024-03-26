using CSLibrary;
using static Common.Utility;

namespace CSUnitTest;

public class StackTest
{
    private const int ELEMENT_CAPACITY = 100;
    private const int PUSH_LOOP_COUNT = 100;
    private const int POP_LOOP_COUNT = 100;

    private const int INITIAL_ELEMENT_CAPACITY = 10;

    [Test]
    public void FixedSizeStackTest()
    {
        FixedSizeStack<int> intStack = new FixedSizeStack<int>(ELEMENT_CAPACITY);
        
        StackValidationTest(intStack);
    }
    
    [Test]
    public void ResizableStackTest()
    {
        ResizableStack<int> intStack = new ResizableStack<int>(INITIAL_ELEMENT_CAPACITY);
        
        StackValidationTest(intStack);
    }
    
    [Test]
    public void LinkedListBasedStackTest()
    {
        LinkedListBasedStack<int> intStack = new LinkedListBasedStack<int>();
        
        StackValidationTest(intStack);
    }

    private void StackValidationTest(IStack<int> stack)
    {
        PushTest(stack);
        PopTest(stack);
    }
    
    private void PushTest(IStack<int> stack)
    {
        stack.Clear();

        printl("Stack PushTest");
        
        for (int i = 0; i < PUSH_LOOP_COUNT; i++)
        {
            stack.Push(i);
            Assert.That(stack.Count(), Is.EqualTo(i+1));
            Assert.That(stack.Peek(), Is.EqualTo(i));
            prints(stack.Peek());
        }
        printl();
    }
    
    private void PopTest(IStack<int> stack)
    {
        stack.Clear();

        printl("Stack PopTest");

        for (int i = 0; i < POP_LOOP_COUNT; i++)
        {
            stack.Push(i);
        }
        
        for (int i = POP_LOOP_COUNT-1; i >= 0; i--)
        {
            var popped = stack.Pop();
            Assert.That(popped, Is.EqualTo(i));
            Assert.That(stack.Count(), Is.EqualTo(i));
            prints(popped);
        }
        
        printl();
    }
}