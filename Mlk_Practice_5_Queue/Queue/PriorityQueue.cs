using System;

namespace QueueLibrary.Queue
{
    public class EnqueuePriorityQueue<T> : IQueue<T> where T : IComparable<T>
    {
        private int _capacity;
        private int count;
        private T[] elements;

        public EnqueuePriorityQueue(int capacity = 16)
        {
            _capacity = capacity;
            elements = new T[capacity];
            count = 0;
        }

        public bool HasNext() => count > 0;
        public bool IsEmpty() => count == 0;

        public void Offer(T element)
        {
            if (count >= _capacity)
                Grow();

            int lo = 0, hi = count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (elements[mid].CompareTo(element) <= 0)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            for (int i = count; i > lo; i--)
                elements[i] = elements[i - 1];

            elements[lo] = element;
            count++;
        }

        public T Pull()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty");

            var result = elements[0];
            for (int i = 0; i < count - 1; i++)
                elements[i] = elements[i + 1];

            count--;
            elements[count] = default;
            return result;
        }

        public T Peek()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty");
            return elements[0];
        }

        private void Grow()
        {
            int newCapacity = _capacity * 2;
            var newArray = new T[newCapacity];
            for (int i = 0; i < count; i++)
                newArray[i] = elements[i];
            elements = newArray;
            _capacity = newCapacity;
        }
    }


    public class DequeuePriorityQueue<T> : IQueue<T> where T : IComparable<T>
    {
        private int _capacity;
        private int count;
        private T[] elements;

        public DequeuePriorityQueue(int capacity = 16)
        {
            _capacity = capacity;
            elements = new T[capacity];
            count = 0;
        }

        public bool HasNext() => count > 0;
        public bool IsEmpty() => count == 0;

        public void Offer(T element)
        {
            if (count >= _capacity)
                Grow();
            elements[count++] = element;
        }

        public T Pull()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty");

            int best = 0;
            for (int i = 1; i < count; i++)
                if (elements[i].CompareTo(elements[best]) < 0)
                    best = i;

            var result = elements[best];

            for (int i = best; i < count - 1; i++)
                elements[i] = elements[i + 1];

            count--;
            elements[count] = default;
            return result;
        }

        public T Peek()
        {
            if (count == 0)
                throw new IndexOutOfRangeException("PriorityQueue is empty");

            int best = 0;
            for (int i = 1; i < count; i++)
                if (elements[i].CompareTo(elements[best]) < 0)
                    best = i;

            return elements[best];
        }

        private void Grow()
        {
            int newCapacity = _capacity * 2;
            var newArray = new T[newCapacity];
            for (int i = 0; i < count; i++)
                newArray[i] = elements[i];
            elements = newArray;
            _capacity = newCapacity;
        }
    }

}