using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QueueLibrary.Queue
{
    public interface IQueue<T>
    {

        T Pull();

        T Peek();

        void Offer(T element);

        bool HasNext();

        bool IsEmpty();

    }
}
