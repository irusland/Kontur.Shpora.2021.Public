using System;
using System.Threading;

namespace LockFree
{
    public class LockFreeStack<T> : IStack<T>
    {
        private Node<T> head;

        public void Push(T obj)
        {
            Node<T> oldHead;
            var newHead = new Node<T> {
                Value = obj
            };
            do
            {
                oldHead = head;
                newHead.Next = oldHead;
            } while (Interlocked.CompareExchange(ref head, newHead, oldHead) != oldHead);
            
        }

        public T Pop()
        {
            T value;

            Node<T> oldHead;
            do
            {
                value = head.Value;
                oldHead = head;
                head = head.Next;
            } while (Interlocked.CompareExchange(ref head, head, oldHead) != oldHead);
                

            return value;
        }
    }
}