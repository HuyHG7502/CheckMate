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
        AlphaBeta,
        Negamax,
        Random
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
        public const int SQUARE_NUM  = 8;
        public const int SQUARE_SIZE = 108;

        public const int BOARD_SIZE = SQUARE_NUM * SQUARE_SIZE;

        public const int WIN_PADDING = 24;

        // For Side Menu logging
        // public const int WIN_WIDTH   = BOARD_SIZE * 4 / 3 + WIN_PADDING * 3;
        public const int WIN_WIDTH  = BOARD_SIZE + WIN_PADDING * 2;
        public const int WIN_HEIGHT = BOARD_SIZE + WIN_PADDING * 2;

        public const int MENU_SIZE  = (WIN_WIDTH - WIN_PADDING * 3) / 4;
        public const int PIECE_SIZE = (int)(SQUARE_SIZE * 0.6);

        public const double KEY_COOLDOWN = 10.0f;

        // TODO: Define UI dimension calculations here
    }
}
