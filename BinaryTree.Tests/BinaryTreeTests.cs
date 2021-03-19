using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BinaryTree.Tests
{
    [TestFixtureSource(typeof(FixtureArgsClass), "FixtureArgs")]
    public class BinaryTreeTests<T>
    {
        private readonly T[] testData;
        private readonly T objectNotInTree;
        private readonly IComparer<T> comparer;
        private readonly T[] preOrderTraverseData;
        private readonly T[] inOrderTraverseData;
        private readonly T[] postOrderTraverseData;
        private BinaryTree<T> tree;
        

        public BinaryTreeTests(T[] testData, T objectNotInTree, IComparer<T> comparer,
            int[] preOrderTraverseIndexes, int[] inOrderTraverseIndexes, int[] postOrderTraverseIndexes)
        {
            this.testData = testData;
            this.objectNotInTree = objectNotInTree;
            this.comparer = comparer;
            preOrderTraverseData = preOrderTraverseIndexes.Select(i => testData[i]).ToArray();
            inOrderTraverseData = inOrderTraverseIndexes.Select(i => testData[i]).ToArray();
            postOrderTraverseData = postOrderTraverseIndexes.Select(i => testData[i]).ToArray();
        }

        private BinaryTree<T> CreateTree()
        {
            if (comparer == Comparer<T>.Default)
                return new BinaryTree<T>();
            else
                return new BinaryTree<T>(comparer);
        }

        [SetUp]
        public void Setup()
        {
            tree = CreateTree();
            foreach (var number in testData)
                tree.Add(number);
        }

        #region Low
        [Test]
        public void Traverse_InOrder_ReturnsCollectionInRightOrder()
        {
            T[] expected = inOrderTraverseData;

            var actual = tree.Traverse(TraverseType.InOrder).ToList();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Contains_ElementThatIsInTree_ReturnsTrue()
        {
            var actual = tree.Contains(testData[0]);

            Assert.IsTrue(actual);
        }

        [Test]
        public void Contains_ElementThatIsNotInTree_ReturnsFalse()
        {
            var actual = tree.Contains(objectNotInTree);

            Assert.IsFalse(actual);
        }

        [Test]
        public void Contains_Null_ReturnsFalse()
        {
            ComparableClass nullElement = null;
            var tree = new BinaryTree<ComparableClass>();

            var actual = tree.Contains(nullElement);

            Assert.IsFalse(actual);
        }

        [Test]
        public void TreeMax_TreeHasElements_ReturnsElementWithHighestValue()
        {
            var expected = testData[2];

            var actual = tree.TreeMax();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TreeMin_TreeHasElements_ReturnsElementWithLowestValue()
        {
            var expected = testData[3];

            var actual = tree.TreeMin();

            Assert.AreEqual(expected, actual);
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        public void Add_AmountOfElements_CountEqualsAmount(int amount)
        {
            var expected = amount;

            var tree = CreateTree();
            for (int i = 0; i < amount; i++)
                tree.Add(testData[0]);
            var actual = tree.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetEnumerator_UsedInForeachCycle_ReturnsElementsByInOrderTraversal()
        {
            var expected = new List<T>(tree.Traverse(TraverseType.InOrder));

            var actual = new List<T>();
            foreach (var element in tree)
                actual.Add(element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetEnumerator_UsingIEnumerableExplicitly_ReturnsElementsByInOrderTraversal()
        {
            var expected = new List<T>(tree.Traverse(TraverseType.InOrder));
            IEnumerable enumerable = tree;

            var actual = new List<T>();
            foreach (var element in enumerable)
                actual.Add((T)element);

            Assert.AreEqual(expected, actual);
        }

        #endregion

		// UNCOMMENT TO CHECK MIDDLE PART
         #region Middle
         [Test]
         public void BinaryTree_CreateInstanceWithUncomparableClass_ThrowsArgumentException()
         {
           Assert.Throws<ArgumentException>(() =>
           {
             var tree = new BinaryTree<UncomparableClass>();
           }
           , "Constructor without parameters should throw ArgumentException if generic type T does not implement IComparable<T>.");
         }

         [Test]
         public void Traverse_PreOrder_ReturnsCollectionInRightOrder()
         {
           //arrange
           T[] expected = preOrderTraverseData;

           //act
           var actual = tree.Traverse(TraverseType.PreOrder).ToList();

           //assert
           Assert.AreEqual(expected, actual);
         }

         [Test]
         public void Traverse_PostOrder_ReturnsCollectionInRightOrder()
         {
           T[] expected = postOrderTraverseData;

           var actual = tree.Traverse(TraverseType.PostOrder).ToList();

           Assert.AreEqual(expected, actual);
         }

         [Test]
         public void Add_Null_ThrowsArgumentNullException()
         {
           ComparableClass nullElement = null;
           var tree = new BinaryTree<ComparableClass>();

           Assert.Throws<ArgumentNullException>(() => tree.Add(nullElement)
           , "Add method should throw ArgumentNullException if parameter is null.");
         }

         [Test]
         public void TreeMax_TreeIsEmpty_ThrowsInvalidOperationException()
         {
           var emptyTree = CreateTree();
           Assert.Throws<InvalidOperationException>(() => emptyTree.TreeMax()
           , "TreeMax method should throw InvalidOperationException if tree contains no elements.");
         }

         [Test]
         public void TreeMin_TreeIsEmpty_ThrowsInvalidOperationException()
         {
           var emptyTree = CreateTree();
           Assert.Throws<InvalidOperationException>(() => emptyTree.TreeMin()
           , "TreeMin method should throw InvalidOperationException if tree contains no elements.");
         }

         [Test]
         public void Add_Element_InvokesEventElementAdded()
         {
           var elementWasAdded = false;
           tree.ElementAdded += (sender, args) => elementWasAdded = true;

           tree.Add(testData[0]);

           Assert.IsTrue(elementWasAdded
               , "Add method should invoke ElementAdded event if parameter was added to tree.");
         }

         [Test]
         public void EventElementAdded_Invoked_ValueIsAddedItem()
         {
           var expected = testData[0];
           T actual = objectNotInTree;
           tree.ElementAdded += (sender, args) => actual = args.Value;

           tree.Add(testData[0]);

           Assert.AreEqual(expected, actual
               , "Added item should be passed to arguments of event when it was invoked.");
         }

         [Test]
         public void EventElementAdded_Invoked_SenderIsCurrentBinaryTree()
         {
           var expected = tree;
           BinaryTree<T> actual = null;
           tree.ElementAdded += (sender, args) => actual = sender as BinaryTree<T>;

           tree.Add(testData[0]);

           Assert.AreSame(expected, actual
               , "Current binary tree should be passed as sender in the event.");
         }

		#endregion



		//UNCOMMENT TO CHECK ADVANCED PART
		#region Advanced
		 [Test]
		 public void Remove_ElementThatIsInTree_ReturnsTrue() {
			var actual = tree.Remove(testData[3]);

			Assert.IsTrue(actual);
		}

		[Test]
		public void Remove_UnexistingElement_ReturnsFalse() {
			var actual = tree.Remove(objectNotInTree);

			Assert.IsFalse(actual);
		}

		[Test]
		public void Remove_Null_ReturnsFalse() {
			ComparableClass nullElement = null;
			var tree = new BinaryTree<ComparableClass>();

			var actual = tree.Remove(nullElement);

			Assert.IsFalse(actual);
		}

		[Test]
		public void Remove_Leaf_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[1], testData[4], testData[0], testData[2] };

			tree.Remove(testData[3]);
			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_NodeWithLeftChild_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[3], testData[0], testData[2] };

			tree.Remove(testData[4]);
			tree.Remove(testData[1]);

			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_NodeWithLeftChildAn_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[3], testData[1], testData[4] };

			tree.Remove(testData[2]);
			tree.Remove(testData[0]);

			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_NodeWithRightChild_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[4], testData[0], testData[2] };
			tree.Remove(testData[3]);

			tree.Remove(testData[1]);
			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_RootWithRightChild_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[2] };
			var tree = CreateTree();
			tree.Add(testData[0]);
			tree.Add(testData[2]);

			tree.Remove(testData[0]);
			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_NodeWithTwoChildren_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[3], testData[4], testData[0], testData[2] };

			tree.Remove(testData[1]);
			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_RootWithTwoChildren_TreeCanBeTraversedCorrectlyInAnyOrder() {
			T[] expected = { testData[3], testData[1], testData[4], testData[2] };

			tree.Remove(testData[0]);
			var actual = tree.Traverse(TraverseType.InOrder).ToList();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Remove_ElementThatIsInTree_InvokesEventElementRemoved() {
			var elementWasRemoved = false;
			tree.ElementRemoved += (sender, args) => elementWasRemoved = true;

			tree.Remove(testData[0]);

			Assert.IsTrue(elementWasRemoved
				, "Remove method should invoke ElementRemoved event if parameter was deleted from tree.");
		}

		[Test]
		public void EventElementRemoved_Invoked_ValueIsRemovedItem() {
			var expected = testData[0];
			T actual = objectNotInTree;
			tree.ElementRemoved += (sender, args) => actual = args.Value;

			tree.Remove(testData[0]);

			Assert.AreEqual(expected, actual
				, "Removed item should be passed to arguments of event when it was invoked.");
		}

		[Test]
		public void EventElementRemoved_Invoked_SenderIsCurrentBinaryTree() {
			var expected = tree;
			BinaryTree<T> actual = null;
			tree.ElementAdded += (sender, args) => actual = sender as BinaryTree<T>;
			tree.Add(testData[0]);

			tree.Remove(testData[0]);

			Assert.AreEqual(expected, actual
				, "Current binary tree should be passed as sender in the event.");
		}

		[Test]
		public void Remove_ElementThatIsNotInTree_DoesNotInvokeEventElementRemoved() {
			var elementWasRemoved = false;
			tree.ElementRemoved += (sender, args) => elementWasRemoved = true;

			tree.Remove(objectNotInTree);

			Assert.IsFalse(elementWasRemoved
				, "Remove method should not invoke event if parameter is not in tree.");
		}

		[TestCase(0)]
		[TestCase(10)]
		[TestCase(100)]
		public void Remove_AmountOfElements_CountEqualsHowManyElementsLeftInTree(int amount) {
			var expected = 100 - amount;
			var tree = CreateTree();
			for (int i = 0;i < 100;i++)
				tree.Add(testData[0]);

			for (int i = 0;i < amount;i++)
				tree.Remove(testData[0]);
			var actual = tree.Count;

			Assert.AreEqual(expected, actual);
		}

		#endregion

	}

	internal class UncomparableClass
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return $"Value - {Value}";
        }
    }

    internal class UncomparableClassComparer : IComparer<UncomparableClass>
    {
        public int Compare([AllowNull] UncomparableClass x, [AllowNull] UncomparableClass y)
        {
            return x.Value.CompareTo(y.Value);
        }

        
    }

    internal class ComparableClass : IComparable<ComparableClass>
    {
        public int Value { get; set; }

        public int CompareTo([AllowNull] ComparableClass other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return $"Value - {Value}";
        }
    }

    internal class FixtureArgsClass
    {
        static object[] FixtureArgs = {

            #region Low, Middle
            new object[]
            {
                typeof(int),
                new int[] { 100, 60, 400, 10, 90 },
                20,
                Comparer<int>.Default,
                new int[] { 0, 1, 3, 4, 2 },
                new int[] { 3, 1, 4, 0, 2 },
                new int[] { 3, 4, 1, 2, 0 }
            },
            new object[]
            {
                typeof(ComparableClass),
                new ComparableClass[]
                {
                    new ComparableClass{ Value = 100 },
                    new ComparableClass{ Value = 60 },
                    new ComparableClass{ Value = 400 },
                    new ComparableClass{ Value = 10 },
                    new ComparableClass{ Value = 90 }
                },
                new ComparableClass{ Value = 20 },
                Comparer<ComparableClass>.Default,
                new int[] { 0, 1, 3, 4, 2 },
                new int[] { 3, 1, 4, 0, 2 },
                new int[] { 3, 4, 1, 2, 0 }
            }
            #endregion

			// UNCOMMENT TO CHECK ADVANCED PART
             #region Advanced
             ,
			 new object[]
			 {
				 typeof(UncomparableClass),
				 new UncomparableClass[]
				 {
					 new UncomparableClass{ Value = 100 },
					 new UncomparableClass{ Value = 60 },
					 new UncomparableClass{ Value = 400 },
					 new UncomparableClass{ Value = 10 },
					 new UncomparableClass{ Value = 90 }
				 },
				 new UncomparableClass{ Value = 20 },
				 new UncomparableClassComparer(),
				 new int[] { 0, 1, 3, 4, 2 },
				 new int[] { 3, 1, 4, 0, 2 },
				 new int[] { 3, 4, 1, 2, 0 }
			 }
             #endregion

        };
    }
}