using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BinaryTree {
	class TreeNode<T> : IComparable{

		private readonly IComparer<T> _comparer;

		public T Value { get; set; }
		public TreeNode<T> Left { get; set; }
		public TreeNode<T> Right { get; set; }

		public TreeNode(IComparer<T> Comparer) {
			_comparer = Comparer;
		}

		public int CompareTo(object obj) {
			return _comparer.Compare(Value, (T)obj);
		}

	}
}
