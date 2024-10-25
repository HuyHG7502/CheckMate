using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CheckMate.Entities;
using CheckMate.Managers;


namespace CheckMate.Pieces
{
    /// <summary>
    /// PieceFactory class for Factory method
    /// </summary>
    public class PieceFactory
    {
        public PieceFactory() { }

        public Piece GetPawn(PieceColour colour)
        {
            return new Pawn(colour);
        }

        public Piece GetRook(PieceColour colour)
        {
            return new Rook(colour);
        }

        public Piece GetKnight(PieceColour colour)
        {
            return new Knight(colour);
        }

        public Piece GetBishop(PieceColour colour)
        {
            return new Bishop(colour);
        }

        public Piece GetQueen(PieceColour colour)
        {
            return new Queen(colour);
        }

        public Piece GetKing(PieceColour colour)
        {
            return new King(colour);
        }
    }
}
