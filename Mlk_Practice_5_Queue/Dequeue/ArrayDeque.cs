using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLibrary.Dequeue
{
    public class ArrayDeque<T> : IDeque<T>
    {
        private T[] buffer;
        private int headPtr;
        private int tailPtr;
        private int count;

        public ArrayDeque(int capacity = 8)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            buffer = new T[capacity];
            headPtr = 0;
            tailPtr = 0;
            count = 0;
        }

        public bool HasNext() => count > 0;
        public bool IsEmpty() => count == 0;
        public int GetCount() => count;
        public int GetCapacity() => buffer.Length;

        public void OfferFirst(T element)
        {
            if (count == buffer.Length)
                Grow();

            // При выходе за 0 переходим в конец массива
            headPtr = (headPtr - 1 + buffer.Length) % buffer.Length;
            buffer[headPtr] = element;
            count++;
        }

        public void OfferLast(T element)
        {
            if (count == buffer.Length)
                Grow();

            buffer[tailPtr] = element;
            tailPtr = (tailPtr + 1) % buffer.Length;
            count++;
        }

        public T PullFirst()
        {
            if (count == 0)
                throw new InvalidOperationException("Deque is empty");

            T value = buffer[headPtr];
            buffer[headPtr] = default;
            headPtr = (headPtr + 1) % buffer.Length;
            count--;

            return value;
        }

        public T PullLast()
        {
            if (count == 0)
                throw new InvalidOperationException("Deque is empty");

            // Сдвиг влево к последнему занятому индексу
            tailPtr = (tailPtr - 1 + buffer.Length) % buffer.Length;
            T value = buffer[tailPtr];
            buffer[tailPtr] = default;
            count--;

            return value;
        }

        public T PeekFirst()
        {
            if (count == 0)
                throw new InvalidOperationException("Deque is empty");

            return buffer[headPtr];
        }

        public T PeekLast()
        {
            if (count == 0)
                throw new InvalidOperationException("Deque is empty");

            int lastIdx = (tailPtr - 1 + buffer.Length) % buffer.Length;
            return buffer[lastIdx];
        }

        private void Grow()
        {
            int newCapacity = buffer.Length * 2;
            T[] newBuffer = new T[newCapacity];

            // Разворачиваем кольцевой массив в прямой последовательный порядок
            for (int i = 0; i < count; i++)
            {
                newBuffer[i] = buffer[(headPtr + i) % buffer.Length];
            }

            buffer = newBuffer;
            headPtr = 0;
            tailPtr = count;
        }
    }
}
