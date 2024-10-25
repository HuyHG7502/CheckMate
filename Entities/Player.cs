using CheckMate.Managers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheckMate.Entities
{
    public abstract class Player
    {
        public Board Board { get; set; }
        public List<Piece> Pieces { get; protected set; } = new List<Piece>();
        public List<Move> Moves { get; protected set; } = new List<Move>();
        public PieceColour Colour { get; private set; }

        public Player(PieceColour colour, Board board)
        {
            Colour = colour;
            Board = board;
        }

        public virtual async Task<Move> MakeMove(GameTime gameTime, StateManager state)
        {
            return await Task.Run(() => Move.Null);
        }

        public bool HavePiece(Piece piece)
        {
            if (piece.IsNull())
                return false;

            return Pieces.Contains(piece);
        }

        public void AddPiece(Piece piece)
        {
            if (!HavePiece(piece))
                Pieces.Add(piece);
        }

        public void RemovePiece(Piece piece)
        {
            Pieces.Remove(piece);
        }

        public override string ToString()
        {
            return $"{GetType()}: {Colour}";
        }
    }
}
