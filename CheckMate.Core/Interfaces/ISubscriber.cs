using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Interfaces
{
    public interface ISubscriber
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify(Object obj1, Object obj2);
    }
}
