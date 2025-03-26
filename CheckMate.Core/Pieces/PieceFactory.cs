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
    public static class PieceFactory
    {
        public static Piece CreatePiece(PieceType type, PieceColour colour)
        {
            return type switch
            {
                PieceType.Pawn => new Pawn(colour),
                PieceType.Rook => new Rook(colour),
                PieceType.Knight => new Knight(colour),
                PieceType.Bishop => new Bishop(colour),
                PieceType.Queen => new Queen(colour),
                PieceType.King => new King(colour),
                _ => throw new ArgumentException("Invalid piece type")
            };
        }
    }
}
