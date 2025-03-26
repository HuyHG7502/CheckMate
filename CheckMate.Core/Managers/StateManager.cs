using CheckMate.Entities;

namespace CheckMate.Managers
{
    public class StateManager
    {
        public GameState GameState { get; set; } = GameState.Start;
        public ChessState ChessState { get; set; } = ChessState.Opening;
        public PieceColour PlayerColour { get; set; } = PieceColour.White;
        public PieceColour CurrentPlayer { get; set; } = PieceColour.White;
        public AIStrategy Strategy { get; set; } = AIStrategy.AlphaBeta;
        public AIDepth Depth { get; set; } = AIDepth.Basic;
        public bool SinglePlayer { get; set; } = true;
        public double ElapsedTime { get; set; } = 0.0f;
        public Action Exit { get; set; }

        public Move Move = Move.Null;
        public Piece SelectedPiece = Piece.Null;
        public Square SelectedSquare = Square.Null;

        public List<Move> AllowedMoves = [];
        public List<Move> DisallowedMoves = [];

        public Stack<Move> Moves = new();

        public bool IsPlayerTurn()
        {
            if (!SinglePlayer)
                return true;

            return CurrentPlayer == PlayerColour;
        }

        public void Init()
        {
            ChessState = ChessState.Opening;
            CurrentPlayer = PieceColour.White;

            ElapsedTime = 0.0f;

            Clear();
            Moves.Clear();
        }

        public void Clear()
        {
            Move = Move.Null;
            SelectedPiece = Piece.Null;
            SelectedSquare = Square.Null;

            AllowedMoves.Clear();
            DisallowedMoves.Clear();
        }
    }
}
