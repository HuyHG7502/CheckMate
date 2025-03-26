    using CheckMate.Interfaces;
    using CheckMate.Managers;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace CheckMate.Entities
    {
        public abstract class Player : IObserver
        {
            public Board Board { get; set; }
            public List<Piece> Pieces { get; protected set; } = new List<Piece>();
            public List<Piece> CapturedPieces { get; set; } = new List<Piece>();
            public List<Move> Moves { get; protected set; } = new List<Move>();
            public PieceColour Colour { get; private set; }

            public Player(PieceColour colour, Board board)
            {
                Colour = colour;
                Board = board;
            }

            public void Update(Object obj1, Object obj2)
            {
                if (obj1 is not Piece && obj2 is not Piece) return;

                Piece oldPiece = (Piece) obj1;
                Piece newPiece = (Piece) obj2;
                if (HavePiece(oldPiece))
                {
                    RemovePiece(oldPiece);
                    AddPiece(newPiece);
                }
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
                if (HavePiece(piece))
                    Pieces.Remove(piece);
            }

            public void AddCaptured(Piece piece)
            {
                if (!CapturedPieces.Contains(piece))
                    CapturedPieces.Add(piece);
            }

            public void RemoveCaptured(Piece piece)
            {
                if (CapturedPieces.Contains(piece))
                    CapturedPieces.Remove(piece);
            }

            public override string ToString()
            {
                return $"{GetType()}: {Colour}";
            }
        }
    }
