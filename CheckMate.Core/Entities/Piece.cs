using CheckMate.Pieces;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Entities
{
    /// <summary>
    /// Base class for Pieces
    /// </summary>
    public abstract class Piece
    {
        protected static Dictionary<PieceType, int> PieceScore = new()
        {
            [PieceType.Null] = 0,
            [PieceType.Pawn] = 1,
            [PieceType.Knight] = 3,
            [PieceType.Bishop] = 5,
            [PieceType.Rook] = 5,
            [PieceType.Queen] = 10,
            [PieceType.King] = 1000,
        };

        public bool HasMoved { get; set; }
        public int Value { get; private set; }
        public PieceType Type { get; protected set; }
        public PieceColour Colour { get; private set; }
        public int[][] Directions { get; protected set; }

        public Piece(PieceColour colour, PieceType type)
        {
            Type = type;
            Colour = colour;
            Value = PieceScore[Type];

            HasMoved = false;
        }

        public static Piece Null => new Null();

        public bool IsNull()
        {
            return Colour == PieceColour.Null || Type == PieceType.Null;
        }

        public abstract Piece Clone();

        public abstract List<Move> GetMoves(Board board, Square fromSqr);

        public bool IsMine(PieceColour colour)
        {
            return Colour == colour;
        }

        public static bool operator ==(Piece p1, Piece p2)
        {
            return p1.Type == p2.Type
                && p1.Colour == p2.Colour
                && p1.HasMoved == p2.HasMoved;
        }

        public static bool operator !=(Piece p1, Piece p2)
        {
            return !(p1 == p2);
        }

        public override string ToString()
        {
            return $"{Colour}{Type}";
        }
    }
}
