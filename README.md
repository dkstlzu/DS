# DS
DS 공부

cpp과 c#으로 공부합니다.

1. Array


2. List 
   1. Array Based Fixed Size List
   2. Array Based Resizable List
   3. Single linked list
   4. Double linked list
   5. Circular Single linked list
   6. Circular Double linked list
   7. Skip List


3. Stack
   1. Array base fixed size
   2. Array base resizable
   3. Linked list base


4. Queue
   1. Array base fixed size
   2. Array base resizable
   3. Linked list base


5. Hash Table
   1. Direct Access Hash Table
   2. Custom Hashing Hash Table
   3. Separate Chaining Hash Table
   4. Coalesced Chaining Hash Table
   5. Linear Probing Hash Table
   6. Quadratic Probing Hash Table
   7. Double Hashing Hash Table
   8. Cuckoo Hashing Hash Table
   9. Hopscotch Hashing Hash Table
   10. Robin Hood Hashing Hash Table

6. Graph : Each with Adjacency List, Adjacency Matrix, Incident Matrix
   1. Directed Graph
   2. Undirected Graph
   3. Weighted Directed Graph
   4. Weighted Undirected Graph

7. Tree : 구현 방식이 매우 다양하여 동일한 인터페이스로 묶을 수 없다는 판단입니다. 각각의 구현을 가집니다.

   1. Binary Tree : Add node as complete tree shape
   2. Binary Search Tree : Linked Nodes
   3. Priority Queue with Heap : [Priority, object] pair with linked list
   4. B-Tree : Simple implementation with integer
   5. AVL Tree
   6. 2-3-4 Tree
   7. Red-Black Tree



구현 및 사용의 정확성을 우선시하는데 목적을 두어 의도치 않게 동작 수행시에 과하다 싶을 정도의 Exception을 발생시킵니다.
예를 들어 Add()등의 함수에서 이미 값이 존재할 경우에도 Exception을 발생시킵니다. 혹은 Remove(key)등의 함수에서
해당 key가 이미 존재하지 않을경우 그대로 return하는 것이 아니라 InvalidOperationException을 발생시킵니다. 
실용적으로 예외 사항 발생시 그대로 return 하거나 값을 수정할 수도 있지만 해당 자료구조의
동작원리에 좀더 집중하고자 함입니다. 따라서 최소구현만을 유지한채 CsUnitTest.csproj의
UnitTest에 따라 올바른 사용례에서의 유효성을 검사합니다. 이에 따라 여러 요소를 동시에 추가하는 AddRange(), Add~~~s()등의
편의성 함수는 구현하지 않습니다.