
using CheckMate.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Pieces
{
    public class Pawn : Piece
    {
        private int _startRank, _endRank;
        public bool IsEnPassant { get; set; } = false;
        public Pawn(PieceColour colour) : base(colour, PieceType.Pawn)
        {
            Directions = [[0, -1], [0, 1]];

            _startRank = Colour == PieceColour.White ? 6 : 1;
            _endRank = Colour == PieceColour.White ? 0 : 7;
        }
        public override Piece Clone()
        {
            return new Pawn(Colour) { 
                HasMoved = this.HasMoved,
                IsEnPassant = this.IsEnPassant,
            };
        }

        public override List<Move> GetMoves(Board board, Square fromSqr)
        {
            List<Move> moves = new List<Move>();

            if (!fromSqr.IsNull() && fromSqr.IsOccupied())
            {
                var dir = Colour == PieceColour.White ? Directions[0] : Directions[1];

                Square toSqr = new(fromSqr + dir);
                if (toSqr.IsInBounds(0, Constants.SQUARE_NUM))
                {
                    toSqr = board[toSqr];
                    if (!toSqr.IsOccupied())
                    {
                        moves.Add(new Move(fromSqr, toSqr) { Type = !HasMoved ? MoveType.First : MoveType.Basic });

                        // Two-square forward first move
                        if (!HasMoved
                            && !board[toSqr + dir].IsOccupied()
                            && fromSqr.File == _startRank)
                        {
                            moves.Add(new Move(fromSqr, board[toSqr + dir]) { Type = MoveType.First });
                        }
                    }

                    // Diagonal capture
                    var captureDirs = new int[][] { [1, 0], [-1, 0] };
                    foreach (var captureDir in captureDirs)
                    {
                        if (new Square(toSqr + captureDir).IsInBounds(0, Constants.SQUARE_NUM)
                            && board[toSqr + captureDir].IsOccupied()
                            && !board[toSqr + captureDir].Piece.IsMine(Colour))
                                moves.Add(new Move(fromSqr, board[toSqr + captureDir]));
                    }

                    // En Passsant
                    foreach (var captureDir in captureDirs)
                    {
                        if (new Square(fromSqr + captureDir).IsInBounds(0, Constants.SQUARE_NUM)
                            && board[fromSqr + captureDir].IsOccupied()
                            && board[fromSqr + captureDir].Piece is Pawn target)
                        {
                            if (target.IsEnPassant && !target.IsMine(Colour))
                                if (new Square(fromSqr + dir + captureDir).IsInBounds(0, Constants.SQUARE_NUM))
                                    moves.Add(new(fromSqr, board[fromSqr + captureDir + dir]) { Type = MoveType.EnPassant, Captured = target });
                        }
                    }
                }

                // Promotion
                foreach (var move in moves)
                    if (move.To.File == _endRank)
                        move.Type = MoveType.Promotion;
            }

            return moves;
        }
    }
}
