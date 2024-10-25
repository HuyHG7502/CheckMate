using CheckMate.Pieces;
using System;
using System.Collections.Generic;

namespace CheckMate.Entities
{
    /// <summary>
    /// A Board class for visual management of Pieces and Squares
    /// </summary>
    public class Board
    {
        private PieceFactory _pieceFactory;
        public Square[,] Squares { get; private set; }
        public Board(PieceFactory pieceFactory)
        {
            _pieceFactory = pieceFactory;

            Squares = new Square[Constants.SQUARE_NUM, Constants.SQUARE_NUM];
            // Initialise squares
            InitSquares();

            // Initialise White pieces
            InitPieces(PieceColour.White, 7, 6);
            // Initialise Black pieces
            InitPieces(PieceColour.Black, 0, 1);
        }

        // Clone the Board, its Squares, and their Pieces
        public Board(Board other)
        {
            _pieceFactory = other._pieceFactory;
            Squares = new Square[Constants.SQUARE_NUM, Constants.SQUARE_NUM];

            foreach (Square square in other.Squares)
                if (square.IsInBounds(0, Constants.SQUARE_NUM))
                    Squares[square.Rank, square.File] = new Square(square);
        }

        public Action<Piece, Piece> OnPiecePromoted;
        public Action<Piece, Piece> OnPiecePromotedUndone;

        private void InitSquares()
        {
            for (int rank = 0; rank < Constants.SQUARE_NUM; rank++)
                for (int file = 0; file < Constants.SQUARE_NUM; file++)
                    Squares[rank, file] = new Square(rank, file);
        }

        // Set up Pieces on the Board Squares
        private void InitPieces(PieceColour colour, int upperRank, int lowerRank)
        {
            // Default setup of Pawns
            for (int file = 0; file < Constants.SQUARE_NUM; file++)
                Squares[file, lowerRank].Piece = _pieceFactory.GetPawn(colour);

            // Default setup of other Pieces
            Squares[0, upperRank].Piece = _pieceFactory.GetRook(colour);
            Squares[1, upperRank].Piece = _pieceFactory.GetKnight(colour);
            Squares[2, upperRank].Piece = _pieceFactory.GetBishop(colour);
            Squares[3, upperRank].Piece = _pieceFactory.GetQueen(colour);
            Squares[4, upperRank].Piece = _pieceFactory.GetKing(colour);
            Squares[5, upperRank].Piece = _pieceFactory.GetBishop(colour);
            Squares[6, upperRank].Piece = _pieceFactory.GetKnight(colour);
            Squares[7, upperRank].Piece = _pieceFactory.GetRook(colour);
        }

        // Operator overload for easy retrieval using Square
        public Square this[Square square]
        {
            get => square.IsInBounds(0, Constants.SQUARE_SIZE) ? Squares[square.Rank, square.File] : Square.Null;
            set
            {
                if (square.IsInBounds(0, Constants.SQUARE_SIZE))
                    Squares[square.Rank, square.File] = value;
            }
        }

        // Operator overload for easy retrieval using Rank, File
        public Square this[int rank, int file]
        {
            get => new Square(rank, file).IsInBounds(0, Constants.SQUARE_SIZE) ? Squares[rank, file] : Square.Null;
            set
            {
                if (new Square(rank, file).IsInBounds(0, Constants.SQUARE_SIZE))
                    Squares[rank, file] = value;
            }
        }

        // Return List of Pieces for a Side
        // Favoured over Player's Pieces during move simulation
        public List<Piece> Pieces(PieceColour colour)
        {
            List<Piece> pieces = new List<Piece>();
            foreach (Square sqr in Squares)
                if (sqr.IsOccupied() && sqr.Piece.IsMine(colour))
                    pieces.Add(sqr.Piece);

            return pieces;
        }

        // Move the Piece from one Square to another
        public bool MakeMove(Move move, out Piece captured)
        {
            if (move.IsNull())
            {
                captured = Piece.Null;
                return false;
            }

            switch (move.Type)
            {
                case MoveType.Castling:
                    return MakeCastlingMove(move, out captured);
                case MoveType.Promotion:
                    return MakePromotionMove(move, out captured);
                case MoveType.EnPassant:
                    return MakeEnPassantMove(move, out captured);
                default:
                    return PerformMove(move, out captured);
            }
        }

        private bool MakeCastlingMove(Move move, out Piece captured)
        {
            Square kingSqr = this[move.From];
            Square rookSqr = this[move.To];

            int[] kingStep = rookSqr.Rank > kingSqr.Rank ? [2, 0] : [-2, 0];
            int[] rookStep = rookSqr.Rank > kingSqr.Rank ? [-1, 0] : [1, 0];

            // Move King
            Square kingTarget = this[kingSqr + kingStep];
            MovePiece(kingSqr, kingTarget);

            // Move Rook
            Square rookTarget = this[kingTarget + rookStep];
            MovePiece(rookSqr, rookTarget);

            // No Piece is captured
            captured = Piece.Null;
            return true;
        }

        private bool MakePromotionMove(Move move, out Piece captured)
        {
            captured = CapturePiece(this[move.To]);

            Piece pawn = this[move.From].Piece;
            Piece queen = _pieceFactory.GetQueen(pawn.Colour);
            queen.HasMoved = true;

            this[move.To].Piece = queen;
            this[move.From].Piece = Piece.Null;

            // Invoke GameManager to update the Player's Pieces
            OnPiecePromoted?.Invoke(pawn, queen);

            return true;
        }

        private bool MakeEnPassantMove(Move move, out Piece captured)
        {
            Piece pawn = this[move.From].Piece;

            int[] pawnStep = pawn.IsMine(PieceColour.White) ? [0, 1] : [0, -1];

            captured = move.Captured;

            this[move.To].Piece = pawn;
            this[move.From].Piece = Piece.Null;
            this[move.To + pawnStep].Piece = Piece.Null;

            return true;
        }

        public void UndoMove(Move move)
        {
            if (move.IsNull()) return;

            switch (move.Type)
            {
                case MoveType.Castling:
                    UndoCastlingMove(move);
                    break;
                case MoveType.Promotion:
                    UndoPromotionMove(move);
                    break;
                case MoveType.EnPassant:
                    UndoEnPassantMove(move);
                    break;
                default:
                    RestoreMove(move);
                    break;
            }
        }

        private void UndoCastlingMove(Move move)
        {
            Square kingSqr = this[move.From];
            Square rookSqr = this[move.To];

            int[] kingStep = rookSqr.Rank > kingSqr.Rank ? [2, 0] : [-2, 0];
            int[] rookStep = rookSqr.Rank > kingSqr.Rank ? [-1, 0] : [1, 0];

            // Move King
            Square kingTarget = this[kingSqr + kingStep];
            MovePiece(kingTarget, kingSqr);

            // Move Rook
            Square rookTarget = this[kingTarget + rookStep];
            MovePiece(rookTarget, rookSqr);
        }

        private void UndoPromotionMove(Move move)
        {
            Piece pawn = move.Moved;
            Piece queen = this[move.To].Piece;

            this[move.From].Piece = pawn;
            this[move.To].Piece = move.Captured;

            // Invoke GameManager to restore Player's Pieces
            OnPiecePromotedUndone?.Invoke(pawn, queen);
        }

        private void UndoEnPassantMove(Move move)
        {
            Piece pawn = move.Moved;
            int[] pawnStep = pawn.IsMine(PieceColour.White) ? [0, 1] : [0, -1];

            ((Pawn)move.Captured).IsEnPassant = ((Pawn)move.Captured).IsEnPassant ? true : false;
            this[move.To + pawnStep].Piece = move.Captured;

            RestoreMove(move);
        }


        private bool PerformMove(Move move, out Piece captured)
        {
            Piece piece = this[move.From].Piece;
            if (piece is Pawn pawn)
                pawn.IsEnPassant = Math.Abs(move.To.File - move.From.File) == 2 ? true : false;

            captured = CapturePiece(this[move.To]);
            MovePiece(this[move.From], this[move.To]);

            return true;
        }

        private void RestoreMove(Move move)
        {
            MovePiece(this[move.To], this[move.From]);

            if (move.Type != MoveType.EnPassant)
                this[move.To].Piece = move.Captured;

            if (move.Type == MoveType.First)
                this[move.From].Piece.HasMoved = false;
        }

        public void MovePiece(Square from, Square to)
        {
            this[to].Piece = this[from].Piece;
            this[from].Piece = Piece.Null;
            if (this[to].IsOccupied())
                this[to].Piece.HasMoved = true;
        }

        public Piece CapturePiece(Square sqr)
        {
            Piece captured = this[sqr].Piece;
            this[sqr].Piece = Piece.Null;
            return captured;
        }

        // Hash the table for AI strategy
        public ulong GetZobristHash(ulong[,] zobristTable)
        {
            int GetPieceIndex(Piece piece)
            {
                int typeIndex = piece.Type switch
                {
                    PieceType.Pawn => 0,
                    PieceType.Knight => 1,
                    PieceType.Bishop => 2,
                    PieceType.Rook => 3,
                    PieceType.Queen => 4,
                    PieceType.King => 5,
                };

                return typeIndex + (piece.IsMine(PieceColour.White) ? 0 : 6);
            }

            ulong hash = 0;
            for (int rank = 0; rank < Constants.SQUARE_NUM; rank++)
                for (int file = 0; file < Constants.SQUARE_NUM; file++)
                {
                    Square sqr = Squares[rank, file];
                    if (sqr.IsOccupied())
                    {
                        Piece piece = sqr.Piece;
                        hash ^= zobristTable[rank, GetPieceIndex(piece)];
                    }
                }

            return hash;
        }
    }
}
