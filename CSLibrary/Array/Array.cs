using System.Runtime.InteropServices;
using static Common.Utility;

namespace CSLibrary;

// Array는 구현이 필요한 자료형이 아닙니다.
// 구문작성의 편의를 위해 함수속에 작성합니다.
public class Array
{
    private const int LOOP_COUNT = 10;
    private const int PREALLOC_SIZE = 10000;

    struct Int
    {
        public int value;

        public Int(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator int(Int I) => I.value;
        public static implicit operator Int(int i) => new Int(i);
    }
    
    public static void Sample()
    {
        #region 배열 공통

        printl("배열 샘플 시작.");
        
        PrepareNextTest();
        
        // 선언은 다음과 같이 원하는 자료형에 []를 붙여 선언합니다.
        // type[5]와 같이 고정된 사이즈의 배열을 선언하는 것은 불가능합니다.
        int[] intArr;

        /*
        할당은 다음과 같이 new 키워드를 이용하여 할당합니다.
        c#에서 배열은 기본적으로 배열에 대한 참조값(포인터)로 취급됩니다.
        실제값은 Heap 메모리 영역에 할당되게 되며 IntArr는 이에 대한 주소값을 가지고 (로컬 변수일 경우) Stack 영역에 할당 됩니다.
        os의 word size에 기인하여 달라질 수 있으며 32bit os의 경우에 4byte만큼의 메모리를 사용할 것입니다.
        이때 int와 같은 primitive 타입 혹은 struct와 같은 value type의 경우 해당 heap영역에 값이 저장되고
        class와 같은 reference type의 경우 해당 heap영역에 객체에 대한 주소값이 저장됩니다.
        배열이 선언될때 기본값으로 0에 해당하는 값이 할당됩니다. int의 경우 0, enum값의 경우 0에 해당하는값등이 들어가게 되며
        선언된 struct의 경우 default() 값이 들어갑니다.
        reference type의 경우 null값이 들어가게 됩니다.
        배열의 할당은 기본적으로 GC의 대상이 되니 이를 인지하고 사용해야 합니다.
        */

        // GC.GetTotalAllocatedBytes 를 사용할 경우 24byte가 더 커진 값이 나옵니다. 배열에 만들고 할당하는 내부 과정에서 임시로 사용되는 메모리로
        // 추측됩니다. 정확히 정보를 찾지 못했습니다.
        var beforeNewArrAllocation = GC.GetTotalMemory(true);
        intArr = new int[4];
        var afterNewArrAllocation = GC.GetTotalMemory(true);
        prints(ALLOCATED_MEM);
        printl(afterNewArrAllocation - beforeNewArrAllocation);
        printl($"{intArr.Length}크기의 정수배열의 경우 4byte정수 {intArr.Length}개 {4*intArr.Length}byte의 메모리가 소모됩니다.");
        
        // ============================================
        
        PrepareNextTest();
        
        /*
        배열이 기본적으로 참조값인 이유는 과도한 메모리 사용을 막기 위해서입니다.
        배열을 이용한다는것 자체가 한개 두개의 데이터가 아닌 많은 수의 일련의 데이터를 이용하겠다는 뜻이고 
        이 크기가 몇십이 될지 몇백만이 될지 알 수 없기에 몇백만과 같이 큰 데이터를 사용하게될 상황을 고려하지 않을 수 없습니다.
        컴퓨터의 동작방식중 함수는 호출될때마다 stack에 새로운 지역변수들에 대한 메모리를 할당하여 사용하게 되는데
        이때 함수에 넘겨주게 되는 파라미터들에 대해서 기본적으로 값복사를 통한 전달이 이루어지게 됩니다.
        */
        void Function(int localInt)
        {
            localInt++;
        }

        // outerInt의 값이 0이고
        int outerInt = 0;
        
        // 함수의 매개변수로 넘겨줌으로써 값을 1증가시키는 듯할 수 있으나
        Function(outerInt);
        
        // 실제값은 변경되지 않은 0으로 남아있게 됩니다.
        prints("지역 변수 테스트");
        printl(outerInt.ToString());
        
        /*
        여기서 고려할 것은 Function()함수가 실행되는 동안은 stack 메모리 영역에 파라미터로 넘겨준 int만큼의 메모리가 할당된다는 점입니다.
        만약 몇백만 크기의 배열을 함수에 인자로 넘겨줄때에 배열 요소에 대한 값복사가 일일이 넘겨주게 된다면 과도한 메모리 사용을 컴퓨터가 감당할 수 없게 되겠죠.
        메모리 사용뿐많이 아니라 메모리를 확보하고 값을 복사하여 할당하는것도 비싼 연산이기도 합니다.
        이러한 문제로 인해서 배열은 기본적으로 주소값을 저장하는 참조값으로 취급됩니다.
        이는 c, cpp등의 언어에서도 동일하며 언어에 대한 특징이라기 보다 컴퓨터의 동작방식 그 자체에 따른 구현 특징으로 보는게 맞습니다.
        */

        // ============================================

        PrepareNextTest();

        void ArrayFunction(int[] localArr)
        {
            for (int i = 0; i < localArr.Length; i++)
            {
                localArr[i] = i * i;
            }
        }
        
        // 여기서 5개의 int에 해당하는 20byte만큼의 메모리가 heap에 할당되고 그 주소를 가리키는
        // word size만큼의 주소 포인터값이 stack속 mainIntArr에 할당되게 됩니다.
        int[] mainIntArr = new int[5];
        
        // 여기서 함수인자로 넘겨지는 mainIntArr에 대한 값복사가 이전과 동일하게 이루어지며 ArrayFunction() 안에는 새로운 localArr라는 이름의 변수에
        // 주소 포인터값이 할당되며 이 값이 가리키는 메모리 영역은 mainIntArr가 가리키는 메모리 주소와 동일합니다.
        // 따라서 서로 다른 주소에 저장되어 있는 주소값을 사용하지만 같은 주소값을 가지기에 mainIntArr가 가리키는 메모리 주소의 값을 변경하게 됩니다.
        ArrayFunction(mainIntArr);

        // mainIntArr가 가리키고 있는 heap영역의 값이 실제로 변경됩니다.
        printl("배열이 참조임을 확인");
        for (int i = 0; i < mainIntArr.Length; i++)
        {
            prints(mainIntArr[i]);
        }
        printl();
        
        // ============================================

        PrepareNextTest();
        
        /*
        따라서 함수를 통해 배열을 주고받을때 주의할 점이 함수 안에서 배열을 할당하여 반환하는 동작입니다.
        함수 외부에서 배열을 할당하고 이를 함수 파라미터로 넘겨 사용할때에는 몇번을 함수를 호출하여도 추가적인 heap영역의 메모리 할당이 이루어 지지 않지만
        함수 내부에서 배열을 할당하고 이를 반환하는 함수의 경우 함수를 호출할때마다 새로운 메모리 할당이 이루어지게 됩니다.
        */
        
        Int[] ReturnArrayFunction(int number)
        {
            var beforeAllocated = GC.GetTotalMemory(true);
            Int[] arr = new Int[number];
            var afterAllocated = GC.GetTotalMemory(true);
            // 1, 3, 5 등의 길이의 배열에서는 bit padding에 의해서 8byte, 16byte, 24byte등으로 나타나지는것을 알 수 있습니다.
            printl($"{number} 길이의 배열 할당, {ALLOCATED_MEM} {afterAllocated - beforeAllocated}");

            for (int i = 0; i < number; i++)
            {
                arr[i] = i;
            }
            
            return arr;
        }

        // 처음 함수가 불리울 때 72byte만큼 더 크게 값이 출력되는 현상이 있습니다.
        // 함수속 함수에 대한 late initialization 최적화 인것으로 개인적으로 추측하지만
        // 정확한 정보를 찾지 못했습니다. 일단 정확한 정보 출력을 위해 테스트 이전에 한번 호출합니다.
        ReturnArrayFunction(0);
        
        // 0의 길이의 배열을 만들지 않기 위해 1부터 시작합니다.
        printl("매번 함수속에서 새로운 배열을 할당");
        for (int i = 1; i <= LOOP_COUNT; i++)
        {
            // 다음과 같이 사용하게 되면 LOOP_COUNT번의 반복문이 도는 동안 계속해서 새로운 메모리 할당이 이루어지게 됩니다.
            Int[] tempArr = ReturnArrayFunction(i);
        }
        
        PrepareNextTest();
        
        // 이경우 다음과 같이 최적화를 할 수 있지만
        printl("미리 배열을 할당받음으로써 앞선 케이스 최적화");
        prints("미리 할당"); 
        Int[] preAllocatedArr = ReturnArrayFunction(LOOP_COUNT);
        var beforePreAllocateTestMem = GC.GetTotalMemory(true);
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            for (int j = 0; j < i; j++)
            {
                // 무언가 작업을 하고
                int dummy = preAllocatedArr[j] + 1;
            }
            
            var whilePreallocateLoopMem = GC.GetTotalMemory(true);
            prints(ALLOCATED_MEM);
            printl((whilePreallocateLoopMem - beforePreAllocateTestMem).ToString());
        }
        
        // 위와 같은 최적화는 함수 내부에서 메모리할당이 이루어지는지 여부에 대한 문맥적 이해가 필요하므로 함수 사용자로 하여금 잘못된 사용을 할 여지를 남기게 됩니다.

        
        // ============================================

        PrepareNextTest();

        #region 배열 GC 최적화

        /*
        이를 해결하기 위한 몇가지 방법이 있는데 첫번째는 미리 할당된 배열에서 값을 복사하여 사용하라는 의도를 함수 시그니쳐에 나타내는 방법이 있습니다.
        */

        // 필요한 데이터를 전역 범위에 미리 정의한 뒤
        Int[] staticArr = new Int[PREALLOC_SIZE];
        for (int i = 0; i < staticArr.Length; i++)
        {
            staticArr[i] = i;
        }
        
        // 함수 시그니쳐에서 그 의도를 표현할 수 있습니다.
        void CopyArrayFunction(Int[] arr, int copyNumber)
        {
            System.Array.Copy(staticArr, arr, copyNumber);
        }

        // 실제 사용할 메모리를 할당하고
        printl($"함수 시그니쳐로 표현하는방법");
        Int[] preAllocatedMemoryArr = new Int[LOOP_COUNT];
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            var beforeCopyTestMem = GC.GetTotalMemory(true);

            // 값을 복사 받습니다.
            CopyArrayFunction(preAllocatedMemoryArr, i);
            var afterCopyTestMem = GC.GetTotalMemory(true);
            
            prints(ALLOCATED_MEM);
            printl(afterCopyTestMem - beforeCopyTestMem);
        }
        
        // 하지만 이 방법의 경우 메모리 할당량은 줄일 수 있지만 많은 양의 데이터의 값복사 연산이 이루어지게 되므로 바람직하지 못하고
        // 실제 사용하게 되는 코드도 복잡해질 뿐더러 많이 조잡하다는 것을 알 수 있습니다.
        
        // ============================================

        PrepareNextTest();

        /*
        두번째 방법으로 System.ArraySegment를 활용하는 방법이 있습니다.
        ArraySegment는 배열의 특정 부분을 잘라내어 가리킬 수 있는 포인터로 활용되기 위한 클래스로 이해할 수 있습니다.
        크기가 10인 배열이 있고 이 배열의 2번째부터 6번째까지의 요소에 대해서 특정 동작을 수행하고 싶을경우
        for (int i = 1; i < 6; i++)
        {
            Do Something
        }
        과 같이 수행할 수도 있으나 동작을 함수화 하여 인자로 넘겨 받아야 하는 경우 
        void function(int[] arr, int startIndex, int length)
        와 같이 시작 index와 length를 같이 넘겨줘야 하는 큰 불편함이 생기게 됩니다.
        */

        // 필요한 배열은 전역이든 지역이든 한번만 메모리 할당한뒤
        Int[] arrForSegment = new Int[PREALLOC_SIZE];
        for (int i = 0; i < arrForSegment.Length; i++)
        {
            arrForSegment[i] = i;
        }

        printl("ArraySegment 테스트 시작");
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            var beforeArraySegmentConstruct = GC.GetTotalMemory(true);
            // ArraySegment를 넘겨줌으로써 몇백만 크기의 메모리를 계속 재할당하는 것을 피합니다.
            var arraySegment = new ArraySegment<Int>(arrForSegment, 0, i+1);

            var afterArraySegmentConstruct = GC.GetTotalMemory(true);
            
            prints(ALLOCATED_MEM);
            printl(afterArraySegmentConstruct - beforeArraySegmentConstruct);
            
            // ArraySegment는 struct 이기 때문에 추가적인 GC가 없다고 기대됩니다.
        }
        
        // ============================================

        PrepareNextTest();

        /*
        세번째 방법으로는 System.Memory, System.ReadOnlyMemory, System.Span, System.ReadOnlySpan을 사용하는 방법입니다.
        개괄적인 이해는 ArraySegment와 동일합니다. 배열등의 일부분을 잘라 사용하기 위해 특정 위치에 대한 포인터를 가져온다는 것입니다.
        하지만 ArraySegment의 경우 indexer의 내부구현을 통해 0번째 인덱스가 마치 타겟하는 위치부터 시작하는 것처럼 눈속임을 해둔 wrapper에 불과하다면
        Span의 경우 실제 동작하도록 기대되는 것과 동일하게 중간지점에 대한 포인팅을 제공합니다. 따라서 더 적은 연산을 통해 요소에 접근할 수 있어 성능상에서 일종의 
        ArraySegment 상위호환쯤 된다고 볼 수 있습니다.
        
        하지만 Span의 경우 ref struct로 선언되어 사용상의 몇가지 제한사항이 있으니 이는 고려하여 사용합시다.
        (Span과 Memory의 메모리 영역에 대한 이야기는 여기서 언급하지 않았습니다.)
        */

        // 필요한 배열은 전역이든 지역이든 한번만 메모리 할당한뒤
        Int[] arrForSpan = new Int[PREALLOC_SIZE];

        
        printl("Span 테스트 시작");
        for (int i = 0; i < LOOP_COUNT; i++)
        {
            var beforeArraySegmentConstruct = GC.GetTotalMemory(true);
            // Span을 사용하여 반복적인 메모리할당을 피합니다.
            var readOnlySpan = new ReadOnlySpan<Int>(arrForSpan, 0, i);

            var afterArraySegmentConstruct = GC.GetTotalMemory(true);
            
            prints(ALLOCATED_MEM);
            printl(afterArraySegmentConstruct - beforeArraySegmentConstruct);
            
            // Span은 ref struct이기 때문에 추가적인 GC가 없다고 기대됩니다.
        }
        
        // ============================================

        PrepareNextTest();

        /*
        세번째 방법으로도 이미 c#에서 배열을 사용하기에 충분히 GC없이 사용할 수 있는 API가 제공되고 있습니다.
        실제로 왠만한 상황에서 필요한 기능은 다 만들어뒀으니 만들어둔 기능을 사용하라고 문서에서도 말하고 있습니다.
        (근데 unsafe구문으로 구현된게 생각보다 종종 보임)
        
        만약 이보다도 더 디테일한 수준까지 배열을 조작하고 싶다면 네번째 방법 unsafe 구문을 이용하는 방법이 있습니다.
        unsafe 구문은 간단히 말하자면 c#이 unmanaged code 작성에 사용되는 방법입니다.
        이 방법을 이용할 경우 배열의 메모리 해제까지도 GC가 아니라 프로그래머가 직접 수행할 수 있습니다. 간단히 이야기 하면
        unsafe syntax에서는 코드를 cpp처럼 쓸 수 있습니다. 포인터도 쓸 수 있습니다. 
        unsafe syntax를 이용하기 위해서는 c# 프로젝트 세팅에서 unsafe 구문 사용을 허가해 주어야합니다.
        .csproj 파일 속에 <AllowUnsafeBlocks>true</AllowUnsafeBlocks> 를 추가해서 변경할 수 있습니다.
        */
        
        // unsafe syntax는 unsafe{} 블럭으로 구분해야 합니다.
        unsafe
        {
            // fixed 구문은 기존 방식대로 new int[100]과 같이 heap에 할당된 메모리가 c#의 GC에 의해 메모리 주소가 재할당 되는것을 막아줍니다.
            // 아래와 같이 작성하는 것으로 c#에서 기존에 제공하던 메모리할당 방식인 new 키워드를 이용할 수 있습니다.
            // 하지만 이 경우 new키워드를 통해 메모리 할당이 이루어 지기 때문에 해당 메모리가 GC를 통해 해제되는것을 바꿀 수 없습니다.

            printl("new 키워드로 할당하는 배열에 대한 정수 포인터");
            var beforeIntPtrToNewArr = GC.GetTotalMemory(true);
            Int[] newIntArrForIntPtr = new Int[PREALLOC_SIZE];
            fixed (Int* unsafeIntPtr = &newIntArrForIntPtr[0])
            {
                var inFixedIntPtrToNewArr = GC.GetTotalMemory(true);
                prints("fixed 속 :");
                prints(ALLOCATED_MEM);
                printl(inFixedIntPtrToNewArr - beforeIntPtrToNewArr);
            }
            var afterIntPtrToNewArr = GC.GetTotalMemory(true);
            prints("fixed 탈출후 :");
            prints(ALLOCATED_MEM);
            printl(afterIntPtrToNewArr - beforeIntPtrToNewArr);
            
            // 포인팅 되었던 배열은 GC에 의해서 메모리가 관리됩니다.
            // 아래와 같이 fixed 문 안에서 포인팅된 배열에 대한 접근이 fixed 문밖에 있을경우 fixed 문이 끝날때 메모리가 자동으로 해제 되지 않고
            // 다른곳에서 사용되지 않을때에는 fixed문이 끝나면서 자동으로 해제가 되고 있습니다. (주석처리하여 확인할 수 있습니다)
            // 문서상에서 이와 같은 체크를 항상 한다고 언급되어 있지는 않아서, 최적화를 위한 세부 구현요소인것으로 보입니다.
            printl(newIntArrForIntPtr[0]);
            
            PrepareNextTest();
            
            // 따라서 실제로 메모리 할당과 해제를 직접 하고 싶은 경우 해당 기능을 제공하는 API를 활용하거나
            // 많은 Memory Allocator가 구현되어 배포되어 있습니다.
            // 대표적으로는 MSDN에도 소개 되어 있는 System.Runtime.InteropServices 네임스페이스에 여러 API가 정의되어 있습니다.
            // .Net 1버전부터 지원됐던 Marshal 클래스와 .Net 6버전부터 지원되는 NativeMemory 클래스가 있습니다.
            // Marshal class는 이 동작에 대한 안전한(safe syntax)동작을 지원하는 wrapper이기 때문에(근본적인 목적, 역할은 따로있음)
            // unsafe에서 사용할 필요는 없습니다.

            // NativeMemory로 할당된 메모리는 GC로 관리되지 않기 때문에 fixed 구문을 사용할 필요가 없습니다.
            var beforeNativeMemoryAlloc = GC.GetTotalMemory(true);
            Int* nativeIntPtr = (Int*)NativeMemory.Alloc((nuint)(10000 * sizeof(Int)));
            // Int* nativeIntPtr = stackalloc int [1000]; // stackalloc을 사용하는 방법도 있습니다(조건부로)
            
            Int* ptr = nativeIntPtr;
            for (int i = 0; i < 10000; i++, ptr++)
            {
                *ptr = i;
            }
            
            var afterNativeMemoryAlloc = GC.GetTotalMemory(true);
            prints("NativeMemory.Alloc 할당후");
            prints(ALLOCATED_MEM);
            printl(afterNativeMemoryAlloc - beforeNativeMemoryAlloc);

            ptr = nativeIntPtr;
            for (int i = 0; i < 10000; i++, ptr++)
            {
                prints(*ptr);
            }
            printl();
            
            var afterNativeMemoryUse = GC.GetTotalMemory(true);
            prints("NativeMemory.Alloc된 배열 사용후");
            prints(ALLOCATED_MEM);
            printl(afterNativeMemoryUse - afterNativeMemoryAlloc);
            
            NativeMemory.Free(nativeIntPtr);
            
            var afterNativeMemoryFree = GC.GetTotalMemory(true);
            prints("NativeMemory.Free 호출후");
            prints(ALLOCATED_MEM);
            printl(afterNativeMemoryFree - afterNativeMemoryUse);
        }

        PrepareNextTest();
        
        #endregion

        #region 배열 Boxing

        /*
        배열과 관련된 내용이라기 보다는 c# 그 자체의 특징이긴 하지만 배열에서 발생될 경우(많은 오브젝트를 다루기때문에) 문제가 커서 한번 다룹니다
        박싱은 int등과 같은 value type들도 모두 기본 System.Object를 상속받도록 되어있어 다음과 같은 할당이 가능합니다.
        object o = 1;
        그러나 위와 같은 경우는 애초에 object 타입의 변수를 선언하기를 원한것이므로 heap상에 메모리가 할당될 것이라는 것은 쉽게 알 수 있고 의도된 사항일 수 있습니다.
        잘못사용된 극단적인 사례를 예시를 들어보겠습니다. python과 같은 동적 타입 언어에 익숙했던 사람이 그와 같은 방식으로 코드를 짜고 싶어하게 되는 상황입니다. 
        */

        // 함수등에 사용되는 타입도 다 object로 쓰고
        object AddNumber(object aNumber, object bNumber)
        {
            return (int)aNumber + (int)bNumber;  // boxing 1회, unboxing 2회
        }

        // 변수 선언도 object로 사용하고
        object aNumber = 1; // boxing 1회
        object bNumber = 2; // boxing 1회

        // 결과도 object로 받고
        object result = AddNumber((int)aNumber, (int)bNumber); // unboxing 2회
        
        printl("Boxing 테스트");
        prints(aNumber);
        prints("더하기");
        prints(bNumber);
        prints("=");
        printl(result);

        /*
        위와 같은 식으로 코드를 작성할 경우 Boxing은 총 3회, unboxing은 총 4회 발생하게 됩니다.
        GC를 발생시키는것뿐만 아니라 boxing, unboxing은 연산 자체도 굉장히 비싼축에 속하기 때문에 지양합시다.
        배열에서 boxing이 발생하는 경우는 다음과 같은 경우가 있겠죠
        */

        object[] allInstances = new object[100];
        allInstances[0] = 1;
        allInstances[1] = "Hi"; // 이와 같이 여러 타입을 동시에 담고 싶은 상황을 가정함
        allInstances[2] = new Int(2);

        #endregion

        #endregion

    }

    static void PrepareNextTest()
    {
        printl();
        Thread.Sleep(1000);
    }

    private const string ALLOCATED_MEM = "할당된 메모리 :";
}