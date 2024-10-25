using CheckMate.Entities;
using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Threading.Tasks;

namespace CheckMate.Players
{
    public class HumanPlayer : Player
    {
        MouseState prev;

        public HumanPlayer(PieceColour colour, Board board)
            : base(colour, board)
        {

        }
        
        public async override Task<Move> MakeMove(GameTime gameTime, StateManager state)
        {
            MouseState current = Mouse.GetState();

            Move move = Move.Null;

            // On Mouse click, execute Moves based on Mouse position
            if (Util.IsMouseClicked(current, prev)) 
                await Task.Run(() => move = ExecuteMove(current, state));

            prev = current;

            return move;
        }
        
        public Move ExecuteMove(MouseState mouse, StateManager state)
        {
            // Get Square based on Mouse position
            Square sqr = Util.GetSquare(Board, mouse);
            Move move = Move.Null;

            // Valid Square on the Board
            if (!sqr.IsNull())
            {
                // A starting Square has been selected
                if (!state.SelectedSquare.IsNull())
                {
                    // Check that Move is allowed
                    move = state.AllowedMoves.FirstOrDefault(m => m.To == sqr) ?? Move.Null;

                    if (!move.IsNull() || sqr == state.SelectedSquare)
                    {
                        ClearMoves(state);
                        return move;
                    }
                }
                
                // Check that your Piece is selected
                if (sqr.IsOccupied() && HavePiece(sqr.Piece))
                {
                    // Reset state
                    state.AllowedMoves.Clear();
                    state.DisallowedMoves.Clear();

                    // Assign state
                    state.SelectedSquare = sqr;
                    state.SelectedPiece = sqr.Piece;

                    // Get Moves for the selected Piece
                    foreach (var m in state.SelectedPiece.GetMoves(Board, state.SelectedSquare))
                    {
                        // If Move leads to Check, disallow it
                        if (MoveValidator.DetectCheck(Board, Colour, m))
                            state.DisallowedMoves.Add(m);
                        else
                            state.AllowedMoves.Add(m);
                    }

                    return Move.Null;
                }

                ClearMoves(state);
            }

            return move;
        }

        public void ClearMoves(StateManager state)
        {
            state.SelectedSquare = Square.Null;
            state.SelectedPiece = Piece.Null;
        }
    }
}
