using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryTree
{
    public class TreeEventArgs<T> : EventArgs
    {
        public T Value { get; private set; }
        public string Message { get; private set; }

        public TreeEventArgs(T value, string message)
        {
            Value = value;
            Message = message;
        }
    }
}
