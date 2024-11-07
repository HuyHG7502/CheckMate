namespace CheckMate
{
    public enum TileType
    {
        White,
        Black,
        Paused,
        Danger,
        Allowed,
        Disallowed,
        Selected,
    }

    public enum MoveType
    {
        First,
        Basic,
        Castling,
        Promotion,
        EnPassant,
    }

    public enum PieceType
    {
        Null,
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public enum PieceColour
    {
        Null = 0,
        White = 1,
        Black = -1
    }

    public enum ChessState
    {
        Default,
        Opening,
        WhiteCheck,
        BlackCheck
    }

    public enum GameState
    {
        Start,
        Playing,
        Paused,
        End
    }

    public enum AIStrategy
    {
        AlphaBeta
        // Negamax,
        // Randomized
    }

    public enum AIDepth
    {
        Basic = 1,
        Easy,
        Medium,
        Hard,
        Expert
    }

    static class Constants
    {
        public const int WIN_SIZE = 800;

        public const int WIN_PADDING = 25;

        public const int BOARD_SIZE = WIN_SIZE - WIN_PADDING * 2;

        public const int SQUARE_NUM  = 8;
        public const int SQUARE_SIZE = BOARD_SIZE / SQUARE_NUM;

        public const int PIECE_SIZE  = (int)(SQUARE_SIZE * 0.6);

        public const double KEY_COOLDOWN = 10.0f;
    }
}
