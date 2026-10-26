using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueLibrary.Dequeue
{
    public interface IDeque<T>
    {
        void OfferFirst(T element);
        void OfferLast(T element);

        T PullFirst();
        T PullLast();

        T PeekFirst();
        T PeekLast();

        bool HasNext();
        bool IsEmpty();
        int GetCount();
    }
}
