using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckMate.Config;
using CheckMate.Entities;
using CheckMate.Helpers;
using CheckMate.Managers;


namespace CheckMate.Strategies
{
    public class AlphaBetaStrategy : IStrategy
    {
        private Player _player;
        private Board _board;
        private Random _random;
        private Dictionary<ulong, int> _transpositionTable;
        private ulong[,] _zobristTable;

        public AlphaBetaStrategy()
        {
            _transpositionTable = new Dictionary<ulong, int>();

            // Initialise Zobrist Table
            _random = new Random();
            _zobristTable = new ulong[Layout.SquareCount, 12];    // 6 PieceTypes x 2 PieceColours

            for (int rank = 0; rank < Layout.SquareCount; rank++)
                for (int file = 0; file < 12; file++)
                    _zobristTable[rank, file] = (ulong)_random.NextInt64() & 0xFFFFFFFFFFFFFFFF;
        }

        public Move CalculateMove(Player player, StateManager state)
        {
            _player = player;
            _board  = new Board(_player.Board);

            PieceColour colour = _player.Colour;
            
            // Default opening move
            if (state.ChessState == ChessState.Opening)
                return new Move(_board[new Square(4, 6)], _board[new Square(4, 4)]);

            Move best = Move.Null;

            int alpha = -int.MaxValue + 1;
            int beta = int.MaxValue - 1;

            for (int depth = 1; depth <= (int) state.Depth; depth++)
                best = IterativeAlphaBeta(alpha, beta, depth, colour, out _);

            return best;
        }

        private Move IterativeAlphaBeta(int alpha, int beta, int depth, PieceColour colour, out int bestScore)
        {
            Move best = Move.Null;
            bestScore = -int.MaxValue + 1;

            List<Move> allMoves = GetOrderedMoves(_board, colour);
            foreach (var move in allMoves)
            {
                if (MoveValidator.DetectCheck(_board, colour)
                    && MoveValidator.DetectCheck(_board, colour, move))
                    continue;
                
                _board.MakeMove(move, out var captured);

                if (MoveValidator.DetectCheck(_board, colour))
                {
                    _board.UndoMove(move);
                    continue;
                }

                int score = -AlphaBeta(-beta, -alpha, depth - 1, GameUtil.GetOpponent(colour));
                
                _board.UndoMove(move);
                
                if (score > bestScore)
                {
                    bestScore = score;
                    best = move; 
                }

                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                    break;
            }

            return best;
        }

        private int AlphaBeta(int alpha, int beta, int depth, PieceColour colour)
        {
            if (depth == 0)
                return Quiescence(alpha, beta, colour);

            ulong boardHash = _board.GetZobristHash(_zobristTable);
            if (_transpositionTable.TryGetValue(boardHash, out int cachedScore))
                return cachedScore;

            // Null Move Pruning
            if (depth >= 3 && !MoveValidator.DetectCheck(_board, colour))
            {
                int r = 2;
                int score = -AlphaBeta(-beta, -beta + 1, depth - 1 - r, GameUtil.GetOpponent(colour));

                if (score >= beta)
                    return score;
            }

            int orgAlpha = alpha;
            PieceColour oppColour = GameUtil.GetOpponent(colour);

            List<Move> allMoves = GetOrderedMoves(_board, colour);
            foreach (var move in allMoves)
            {
                _board.MakeMove(move, out var captured);
                if (MoveValidator.DetectCheck(_board, colour))
                {
                    _board.UndoMove(move);
                    continue;
                }

                int score = -AlphaBeta(-beta, -alpha, depth - 1, oppColour);
                _board.UndoMove(move);

                if (score >= beta)
                {
                    _transpositionTable[boardHash] = beta;
                    return beta;
                }

                if (score > alpha)
                    alpha = score;
            }

            if (alpha != orgAlpha)
                _transpositionTable[boardHash] = alpha;

            return alpha;
        }

        private int Quiescence(int alpha, int beta, PieceColour colour)
        {
            int standPat = GameUtil.GetScore(_board, colour);
            if (standPat >= beta)
                return beta;

            if (alpha < standPat)
                alpha = standPat;

            List<Move> captureMoves = GetCaptureMoves(_board, colour);
            foreach (var move in captureMoves)
            {
                _board.MakeMove(move, out var captured);
                if (MoveValidator.DetectCheck(_board, colour))
                {
                    _board.UndoMove(move);
                    continue;
                }

                int score = -Quiescence(-beta, -alpha, GameUtil.GetOpponent(colour));
                _board.UndoMove(move);

                if (score >= beta)
                    return beta;

                if (score > alpha)
                    alpha = score;
            }

            return alpha;
        }

        private List<Move> GetOrderedMoves(Board board, PieceColour colour)
        {
            var allMoves = new List<Move>();
            foreach (var piece in board.Pieces(colour))
                allMoves.AddRange(piece.GetMoves(board, GameUtil.GetSquare(_board, piece)));

            allMoves.Sort((a, b) => CompareMoves(a, b));
            return allMoves;
        }

        private List<Move> GetCaptureMoves(Board board, PieceColour colour)
        {
            var captureMoves = new List<Move>();
            foreach (var piece in board.Pieces(colour))
            {
                var moves = piece.GetMoves(board, GameUtil.GetSquare(_board, piece));
                foreach (var move in moves)
                    if (!move.Captured.IsNull())
                        captureMoves.Add(move);
            }

            return captureMoves;
        }

        private int CompareMoves(Move a, Move b)
        {
            // Prioritise promotion moves
            if (a.Type == MoveType.Promotion && b.Type != MoveType.Promotion)
                return 1;
            if (a.Type != MoveType.Promotion && b.Type == MoveType.Promotion)
                return -1;

            // Prioritise capturs using MVV-LVA heuristic
            if (!a.Captured.IsNull() && !b.Captured.IsNull())
            {
                int aValue = a.Captured.Value - a.Moved.Value;
                int bValue = b.Captured.Value - b.Moved.Value;

                return bValue.CompareTo(aValue);
            }

            if (!a.Captured.IsNull())
                return -1;

            if (!b.Captured.IsNull())
                return 1;

            // No priority
            return 0;
        }
    }
}
