using CheckMate.Entities;
using CheckMate.Managers;
using CheckMate.Strategies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheckMate.Players
{
    public class AIPlayer : Player
    {
        private IStrategy _strategy;

        private Dictionary<AIStrategy, IStrategy> _strategies = new()
        {
            // Add other AI strategies here when available
            { AIStrategy.AlphaBeta, new AlphaBetaStrategy() },
        };

        public AIPlayer(PieceColour colour, Board board, AIStrategy strategy, AIDepth depth)
            : base(colour, board)
        {
            _strategy = _strategies[strategy];
        }

        public async override Task<Move> MakeMove(GameTime gameTime, StateManager state)
        {
            // Delegate Move to underlying AI strategy
            return await Task.Run(() => _strategy.CalculateMove(this, state));
        }
    }
}
