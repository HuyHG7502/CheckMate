using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Interfaces
{
    public interface IObserver
    {
        void Update(Object obj1, Object obj2);
    }
}
