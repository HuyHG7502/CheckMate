using CheckMate.Entities;
using Microsoft.Xna.Framework.Input;
using System;

namespace CheckMate
{
    public static class Util
    {
        public static bool IsMouseClicked(MouseState current, MouseState previous)
            => current.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released;

        public static bool IsKeyPressed(Keys key, KeyboardState current, KeyboardState previous)
            => current.IsKeyDown(key) && !previous.IsKeyDown(key);

        public static PieceColour GetOpponent(PieceColour colour)
            => colour == PieceColour.White ? PieceColour.Black : PieceColour.White;

        public static Square GetSquare(Board board, MouseState mouse)
        {
            int x = (mouse.X - Constants.WIN_PADDING) / Constants.SQUARE_SIZE;
            int y = (mouse.Y - Constants.WIN_PADDING) / Constants.SQUARE_SIZE;

            Square sqr = new(x, y);
            return sqr.IsInBounds(0, Constants.SQUARE_NUM) ? board[sqr] : Square.Null;
        }

        public static Square GetSquare(Board board, Piece piece)
        {
            foreach (var sqr in board.Squares)
                if (sqr.IsOccupied() && sqr.Piece.Equals(piece))
                    return sqr;

            return Square.Null;
        }

        public static T GetNextEnumValue<T>(T current) where T : Enum
        {
            T[] values = (T[])Enum.GetValues(typeof(T));

            int currentIdx = Array.IndexOf(values, current);
            int nextIdx = (currentIdx + 1) % values.Length;

            return values[nextIdx];
        }

        public static int GetScore(Board board, PieceColour colour)
        {
            int score = 0;
            foreach (Square sqr in board.Squares)
                if (sqr.IsOccupied())
                    score += sqr.Piece.Value * (sqr.Piece.IsMine(colour) ? 1 : -1);

            return score;
        }
    }
}
