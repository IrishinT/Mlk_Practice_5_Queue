using QueueLibrary.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLibrary.Dequeue
{
    public class LinkedDeque<T> : IDeque<T>
    {
        private QueueNode<T> head;
        private QueueNode<T> tail;
        private int count;

        public LinkedDeque()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public bool HasNext() => count > 0;
        public bool IsEmpty() => count == 0;
        public int GetCount() => count;


        public void OfferFirst(T element)
        {
            var node = new QueueNode<T>(element, prev: null, next: head);

            if (head != null)
                head.Prev = node;
            else
                tail = node; // Если дек был пуст, хвост указывает туда же

            head = node;
            count++;
        }

        public void OfferLast(T element)
        {
            var node = new QueueNode<T>(element, prev: tail, next: null);

            if (tail != null)
                tail.Next = node;
            else
                head = node; // Если дек был пуст

            tail = node;
            count++;
        }

        public T PullFirst()
        {
            if (head == null)
                throw new InvalidOperationException("Deque is empty");

            var value = head.Value;
            head = head.Next;

            if (head != null)
                head.Prev = null;
            else
                tail = null; // Если элементов больше не осталось

            count--;
            return value;
        }

        public T PullLast()
        {
            if (tail == null)
                throw new InvalidOperationException("Deque is empty");

            var value = tail.Value;
            tail = tail.Prev;

            if (tail != null)
                tail.Next = null;
            else
                head = null;

            count--;
            return value;
        }


        public T PeekFirst()
        {
            if (head == null)
                throw new InvalidOperationException("Deque is empty");
            return head.Value;
        }

        public T PeekLast()
        {
            if (tail == null)
                throw new InvalidOperationException("Deque is empty");
            return tail.Value;
        }
    }
}
