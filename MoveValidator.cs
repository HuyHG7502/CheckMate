using System.Collections.Generic;
using System.Linq;
using CheckMate.Entities;

namespace CheckMate
{
    public static class MoveValidator
    {
        // Given Board, locate a Side's King Square
        public static Square LocateKing(Board board, PieceColour colour)
        {
            return board.Squares.Cast<Square>()
                .FirstOrDefault(sqr => sqr.IsOccupied() &&
                                       sqr.Piece.IsMine(colour) &&
                                       sqr.Piece.Type == PieceType.King) ?? Square.Null;
      
        }

        // Check if a given Side is being checked
        public static bool DetectCheck(Board board, PieceColour colour)
        {
            Board _board = new(board);

            PieceColour opp = Util.GetOpponent(colour);
        
            Square kingSqr = LocateKing(_board, colour);
            // King not found, return true for Check
            if (kingSqr.IsNull()) return true;

            // Check if the opponent Pieces can reach the King
            foreach (var piece in _board.Pieces(opp))
            {
                List<Move> oppMoves = piece.GetMoves(_board, Util.GetSquare(_board, piece));
                if (oppMoves.Any(move => move.To == kingSqr))
                    return true;
            }

            return false;
        }

        // Check if a given Move will lead to Check
        public static bool DetectCheck(Board board, PieceColour colour, Move move)
        {
            PieceColour opp = Util.GetOpponent(colour);
            // Clone the board!!!
            Board _board = new(board);

            // Simulate move
            _board.MakeMove(move, out Piece captured);

            Square kingSqr = LocateKing(_board, colour);
            if (kingSqr.IsNull())
            {
                _board.UndoMove(move);
                return true;
            }

            foreach (var piece in _board.Pieces(opp))
            {
                // Opponent pieces can reach King
                List<Move> oppMoves = piece.GetMoves(_board, Util.GetSquare(_board, piece));
                if (oppMoves.Any(move => move.To == kingSqr))
                {
                    _board.UndoMove(move);
                    return true;
                }
            }

            // Undo move
            _board.UndoMove(move);
            return false;
        }

        public static bool DetectCheckmate(Board board, PieceColour colour)
        {
            Board _board = new(board);

            // Not Checkmate if there is at least one escape move
            foreach (var piece in _board.Pieces(colour))
            {
                List<Move> moves = piece.GetMoves(_board, Util.GetSquare(_board, piece));
                foreach (var move in moves)
                    if (!DetectCheck(_board, colour, move))
                        return false;
            }   

            return true;
        }
    }
}
