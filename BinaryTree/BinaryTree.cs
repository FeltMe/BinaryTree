using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BinaryTree {
	public class BinaryTree<T> : IEnumerable<T> {

		private TreeNode<T> _head;
		private readonly IComparer<T> _comparer;
		/// <summary>
		/// Handles events for adding and removing elements
		/// </summary>
		/// <param name="sender">Instance of <see cref="BinaryTree<T>"/> that called the event</param>
		/// <param name="args">Arguments passed by sender for subscribers</param>
		public delegate void TreeEventHandler(object sender, TreeEventArgs<T> args);

		/// <summary>
		/// Event that should be called when new element is added
		/// </summary>
		public event TreeEventHandler ElementAdded;

		/// <summary>
		/// Event that should be called when element in tree is removed
		/// </summary>
		public event TreeEventHandler ElementRemoved;

		/// <summary>
		/// Defines how many elements tree contains
		/// </summary>
		private int _count;
		public int Count {
			get {
				return _count;
			}
			private set {
				_count = value;
			}
		}

		/// <summary>
		/// Checks if type T implements <see cref="IComparable<T>"/>
		/// If it does: saves and uses as default comparer
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when T doesn't implement <see cref="IComparable<T>"</exception>
		public BinaryTree() {
			if (!(typeof(T).GetInterfaces().Contains(typeof(IComparable<T>))))
				throw new ArgumentException("Shoud implement IComparable<T>");
			_comparer = Comparer<T>.Default;
		}

		/// <summary>
		/// Creates instance of tree and saves custom comparer passed by parameter
		/// </summary>s
		/// <param name="comparer"><see cref="IComparer<T>"/></param>
		public BinaryTree(IComparer<T> comparer) {
			_comparer = comparer;
		}

		/// <summary>
		/// Adds element to the tree according to comparer
		/// </summary>
		/// <param name="item">Object that should be added in tree</param>
		/// <exception cref="ArgumentNullException">Thrown if parameter was null</exception>
		public void Add(T item) {
			if (item is null)
				throw new ArgumentNullException(nameof(item));
			var newNode = new TreeNode<T>(_comparer) {
				Value = item
			};
			TreeNode<T> before = null;
			var after = _head;
			while (after != null) {
				before = after;
				if (_comparer.Compare(after.Value, item) > 0)
					after = after.Left;
				else if (_comparer.Compare(after.Value, item)< 0) 
					after = after.Right;
				else if (_comparer.Compare(item, after.Value) == 0) {
					while (!(after is null)) {
						if (after.Right is null) {
							after.Right = newNode;
							Count++;
							ElementAdded?.Invoke(this, new TreeEventArgs<T>(item, "Aded"));
							return;
						}
						after = after.Right;
					}
				}
			}

			if (_head == null)
				_head = newNode;
			else {
				if (before.CompareTo(item) > 0)
					before.Left = newNode;
				else
					before.Right = newNode;
			}
			ElementAdded?.Invoke(this, new TreeEventArgs<T>(item, "Aded"));
			Count++;
		}

		/// <summary>
		/// Removes element from tree by its reference
		/// </summary>
		/// <param name="item">Object that should be removed from tree</param>
		/// <returns>True if element was deleted succesfully, false if element wasn't found in tree</returns>
		public bool Remove(T item) {
			if (_head is null)
				return false;
			if (!(Contains(item)))
				return false;
			_head = Remove(_head, item);
			Count--;
			return true;
		}

		private TreeNode<T> Remove(TreeNode<T> parent, T key) {
			if (parent == null)
				return parent;
			if (parent.CompareTo(key) > 0)
				parent.Left = Remove(parent.Left, key);
			else if (parent.CompareTo(key) < 0)
				parent.Right = Remove(parent.Right, key);
			else {
				if (parent.Left == null)
					return parent.Right;
				else if (parent.Right == null)
					return parent.Left;

				parent.Value = MinValue(parent.Right);

				ElementRemoved?.Invoke(this, new TreeEventArgs<T>(key, "Element removed"));
				parent.Right = Remove(parent.Right, parent.Value);
			}
			return parent;
		}

		private T MinValue(TreeNode<T> node) {
			T minv = node.Value;

			while (node.Right != null) {
				minv = node.Right.Value;
				node = node.Right;
			}
			return minv;
		}

		private T MaxValue(TreeNode<T> node) {
			T maxv = node.Value;

			while (node.Left != null) {
				maxv = node.Left.Value;
				node = node.Left;
			}
			return maxv;
		}

		/// <summary>
		/// Returns item with the highest value
		/// </summary>
		/// <returns>The element with highest value</returns>
		/// <exception cref="InvalidOperationException">Thrown if tree is empty</exception> 
		public T TreeMax() {
			if (_head is null)
				throw new InvalidOperationException("Tree is empty");
			return MinValue(_head);
		}

		/// <summary>
		/// Returns item with the lowest value
		/// </summary>
		/// <returns>The element with lowest value</returns>
		/// <exception cref="InvalidOperationException">Thrown if tree is empty</exception>
		public T TreeMin() {
			if (_head is null)
				throw new InvalidOperationException("Tree is empty");
			return MaxValue(_head);
		}

		/// <summary>
		/// Checks if tree contains element by its reference
		/// </summary>
		/// <param name="item">Object that should (or not) be found in tree</param>
		/// <returns>True if tree contains item, false if it doesn't</returns>
		public bool Contains(T data) {
			return ifNodeExists(_head, data);
		}

		private bool ifNodeExists(TreeNode<T> node, T key) {
			if (node == null)
				return false;
			if (node.Value.Equals(key))
				return true;
			bool res1 = ifNodeExists(node.Left, key);
			if (res1)
				return true;
			bool res2 = ifNodeExists(node.Right, key);

			return res2;
		}

		/// <summary>
		/// Makes tree traversal
		/// </summary>
		/// <param name="traverseType"><see cref="TraverseType"></param>
		/// <returns>Sequense of elements of tree according to traverse type</returns>
		public IEnumerable<T> Traverse(TraverseType traverseType) {
			var list = new List<T>();
			if (traverseType == TraverseType.InOrder) {
				TraverseInOrder(_head, list);
				return list;
			} else if (traverseType == TraverseType.PostOrder) {
				TraversePostOrder(_head, list);
				return list;
			} else
				TraversePreOrder(_head, list);
			return list;
		}

		void TraversePreOrder(TreeNode<T> parent, List<T> list) {
			if (parent != null) {
				list.Add(parent.Value);
				TraversePreOrder(parent.Left, list);
				TraversePreOrder(parent.Right, list);
			}
		}

		void TraverseInOrder(TreeNode<T> parent, List<T> list) {
			if (parent != null) {
				TraverseInOrder(parent.Left, list);
				list.Add(parent.Value);
				TraverseInOrder(parent.Right, list);
			}
		}

		void TraversePostOrder(TreeNode<T> parent, List<T> list) {
			if (parent != null) {
				TraversePostOrder(parent.Left, list);
				TraversePostOrder(parent.Right, list);
				list.Add(parent.Value);
			}
		}

		/// <summary>
		/// Makes in-order traverse
		/// Serves as a default <see cref="TraverseType"/> for tree
		/// </summary>
		/// <returns>Enumerator for iterations in foreach cycle</returns>
		public IEnumerator<T> GetEnumerator() {
			var list = Traverse(TraverseType.InOrder);
			foreach (var item in list) {
				yield return item;
			}
		}

		/// <summary>
		/// Makes in-order traverse
		/// Serves as a default <see cref="TraverseType"/> for tree
		/// </summary>
		/// <returns>Enumerator for iterations in foreach cycle</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return Traverse(TraverseType.InOrder).GetEnumerator();
		}
	}
}
