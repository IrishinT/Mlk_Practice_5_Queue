using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLibrary.Queue
{
    internal class QueueNode<T>
    {
        public T Value { get; set; }
        public QueueNode<T> Prev { get; set; }
        public QueueNode<T> Next { get; set; }

        public QueueNode(T value, QueueNode<T> prev, QueueNode<T> next)
        {
            Value = value;
            Prev = prev;
            Next = next;
        }
    }

    public class LinkedQueue<T> : IQueue<T>
    {
        private QueueNode<T> head;
        private QueueNode<T> tail;

        public LinkedQueue()
        {
            head = null;
            tail = null;
        }

        public bool HasNext()
        {
            return head != null && tail != null;
        }

        public bool IsEmpty()
        {
            return head == null || tail == null;
        }

        public void Offer(T element)
        {
            var node = new QueueNode<T>(element, tail, null);

            if (tail != null)
                tail.Next = node;
            else
                head = node;

            tail = node;
        }

        public T Pull()
        {
            if (head == null)
                throw new InvalidOperationException("Queue is empty");

            var value = head.Value;
            head = head.Next;

            if (head != null)
                head.Prev = null;
            else
                tail = null;

            return value;
        }

        public T Peek()
        {
            if (head == null)
                throw new InvalidOperationException("Queue is empty");

            return head.Value;
        }

    }
}
