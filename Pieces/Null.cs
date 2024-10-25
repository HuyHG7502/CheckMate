using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckMate.Entities;

namespace CheckMate.Pieces
{
    public class Null : Piece
    {
        // A default Piece for the purpose of not using null
        public Null() : base(PieceColour.Null, PieceType.Null) { }

        public override Piece Clone() => this;
        public override List<Move> GetMoves(Board board, Square fromSqr) => new List<Move>();
    }
}
