using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckMate.Entities;
using CheckMate.Managers;

namespace CheckMate
{
    public interface IStrategy
    {
        public Move CalculateMove(Player player, StateManager state);
    }
}
