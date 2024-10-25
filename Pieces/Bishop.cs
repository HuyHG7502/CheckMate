
using CheckMate.Entities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(PieceColour colour) : base(colour, PieceType.Bishop)
        {
            Directions = [[1, -1], [1, 1], [-1, 1], [-1, -1]];
        }

        public override Piece Clone()
        {
            return new Bishop(Colour) { HasMoved = this.HasMoved };
        }

        public override List<Move> GetMoves(Board board, Square fromSqr)
        {
            List<Move> moves = new List<Move>();

            if (!fromSqr.IsNull() && fromSqr.IsOccupied())
            {
                for (int i = 0; i < Directions.GetLength(0); i++)
                {
                    Square toSqr = new(fromSqr + Directions[i]);
                    while (toSqr.IsInBounds(0, Constants.SQUARE_NUM))
                    {
                        toSqr = board[toSqr];
                        if (!toSqr.IsOccupied())
                            moves.Add(new Move(fromSqr, toSqr) { Type = !HasMoved ? MoveType.First : MoveType.Basic });
                        else
                        {
                            if (!toSqr.Piece.IsMine(Colour))
                                moves.Add(new Move(fromSqr, toSqr) { Type = !HasMoved ? MoveType.First : MoveType.Basic });
                            break;
                        }

                        toSqr += Directions[i];
                    }
                }
            }

            return moves;
        }
    }
}
