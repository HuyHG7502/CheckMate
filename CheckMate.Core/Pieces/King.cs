
using CheckMate.Config;
using CheckMate.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Pieces
{
        public class King : Piece
        {
            public King(PieceColour colour) : base(colour, PieceType.King)
            {
                Directions = [
                    [ 0, -1 ], [ 0, 1 ], [ -1, 0 ], [ 1, 0 ],
                    [ 1, -1 ], [ 1, 1 ], [ -1, 1 ], [ -1, -1 ]
                ];
            }

            public override Piece Clone()
            {
                return new King(Colour) { HasMoved = this.HasMoved };
            }

            public override List<Move> GetMoves(Board board, Square fromSqr)
            {
                List<Move> moves = [];

                if (!fromSqr.IsNull() && fromSqr.IsOccupied())
                {
                    for (int i = 0; i < Directions.GetLength(0); i++)
                    {
                        Square toSqr = new(fromSqr + Directions[i]);
                        if (toSqr.IsInBounds(0, Layout.SquareCount))
                        {
                            toSqr = board[toSqr];
                            if (!toSqr.IsOccupied())
                                moves.Add(new Move(fromSqr, toSqr) { Type = !HasMoved ? MoveType.First : MoveType.Basic });
                            else
                            {
                                if (!toSqr.Piece.IsMine(Colour))
                                    moves.Add(new Move(fromSqr, toSqr) { Type = !HasMoved ? MoveType.First : MoveType.Basic });
                            }
                        }
                    }

                    moves.AddRange(GetCastlingMoves(board, fromSqr));
                }

                return moves;
            }

            // Get Castling Moves for King in relation to both Rooks
            public List<Move> GetCastlingMoves(Board board, Square fromSqr)
            {
                List<Move> moves = [];
            
                if (!HasMoved)
                {
                    Square toSqr = Square.Null;
                    Move move = Move.Null;

                    if (CanCastle(board, fromSqr, true, out toSqr))
                        moves.Add(new(fromSqr, toSqr) { Type = MoveType.Castling });

                    if (CanCastle(board, fromSqr, false, out toSqr))
                        moves.Add(new(fromSqr, toSqr) { Type = MoveType.Castling });
                }

                return moves;
            }

            // Check if there is a Piece between King and Rook
            // Check if Rook has moved
            private bool CanCastle(Board board, Square fromSqr, bool kingSide, out Square rookSqr)
            {
                rookSqr = board[kingSide ? 0 : 7, fromSqr.File];
                Piece rook = rookSqr.Piece;

                if (!rook.IsNull()
                    && rook.Type == PieceType.Rook
                    && rook.IsMine(Colour)
                    && !rook.HasMoved)
                {
                    int[] step = kingSide ? [-1, 0] : [1, 0];
                    Square intSqr = new(fromSqr + step);

                    while (intSqr.IsInBounds(0, Layout.SquareCount)
                        && board[intSqr] != rookSqr)
                    {
                        intSqr = board[intSqr];
                        if (intSqr.IsOccupied())
                            return false;

                        intSqr += step;
                    }
                    return true;
                }
                return false;
            }
        }
}
