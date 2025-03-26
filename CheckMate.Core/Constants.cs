using CheckMate;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace CheckMate
{
    public enum MoveType
    {
        First,
        Basic,
        Castling,
        Promotion,
        EnPassant,
    }

    public enum TileType
    {
        White,
        Black,
        Paused,
        Danger,
        Allowed,
        Disallowed,
        Selected,
        OuterBorder,
        InnerBorder,
        LightButton,
        DarkButton,
        Background,
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

    public enum PieceSet
    {
        alfonso,
        cardinal,
        chessicons,
        dubrovny,
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
        // Negamax,
        // Random
    }

    public enum AIDepth
    {
        Basic = 1,
        Easy,
        Medium,
        Hard,
        Expert
    }

    public enum FontStyle
    {
        Regular,
        Bold
    }

    public static class Constants
    {
        public const int WIN_PADDING = 24;

        public const double KEY_COOLDOWN = 10.0f;
    }
}
