using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckMate.Entities
{
    public class Move
    {
        public Square From { get; set; }
        public Square To { get; set; }
        public Piece Moved { get; set; }
        public Piece Captured { get; set; }
        public MoveType Type { get; set; } = MoveType.Basic;

        public Move(Square from, Square to)
        {
            From = from;
            To = to;
            Moved = from.Piece;
            Captured = to.Piece;
        }

        public Move()
        {
            From = Square.Null;
            To = Square.Null;
            Moved = Piece.Null;
            Captured = Piece.Null;
        }

        public static Move Null => new();

        public bool IsNull()
        {
            return From.IsNull() || To.IsNull() || Moved.IsNull();
        }
    }
}
