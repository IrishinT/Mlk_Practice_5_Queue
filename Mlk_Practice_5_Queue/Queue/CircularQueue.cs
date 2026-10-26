using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLibrary.Queue
{
    public class CircularQueue<T> : IQueue<T>
    {
        private int count;
        private int headPtr, tailPtr;
        private readonly T[] buffer;

        public CircularQueue(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException();

            buffer = new T[capacity];
        }

        public bool HasNext()
        {
            return count > 0;
        }

        public bool IsEmpty()
        {
            return count == 0;
        }

        public int GetCount()
        {
            return count;
        }

        public int GetCapacity()
        {
            return buffer.Length;
        }

        public virtual void Offer(T element)
        {
            if (count == buffer.Length)
                throw new InvalidOperationException("Queue is full");

            buffer[tailPtr] = element;
            tailPtr = (tailPtr + 1) % buffer.Length;

            count++;
        }

        public T Pull()
        {
            if (count == 0)
                throw new InvalidOperationException("Queue is empty");

            var value = buffer[headPtr];
            buffer[headPtr] = default;
            headPtr = (headPtr + 1) % buffer.Length;

            count--;
            return value;
        }

        public T Peek()
        {
            if (count == 0)
                throw new InvalidOperationException("Queue is empty");

            return buffer[headPtr];
        }

    }


    public class OverwritingCircularQueue<T> : CircularQueue<T>
    {
        public int DroppedCount { get; private set; }

        public OverwritingCircularQueue(int capacity) : base(capacity) { }

        public override void Offer(T element)
        {
            // Если буфер полон — выбрасываем самый старый элемент
            if (GetCount() == GetCapacity())
            {
                Pull();
                DroppedCount++;
            }

            // Добавляем новый элемент через базовую реализацию
            base.Offer(element);
        }
    }

}
