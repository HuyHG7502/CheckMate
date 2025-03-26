using CheckMate.Config;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Helpers
{
    public static class LayoutUtil
    {
        public static Color GetColorFromHex(string hex)
        {
            // Remove # if present
            hex = hex.Replace("#", "");

            // ARGB or RGB support
            if (hex.Length == 6)
            {
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new Color(r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = Convert.ToByte(hex.Substring(0, 2), 16);
                byte r = Convert.ToByte(hex.Substring(2, 2), 16);
                byte g = Convert.ToByte(hex.Substring(4, 2), 16);
                byte b = Convert.ToByte(hex.Substring(6, 2), 16);
                return new Color(r, g, b, a);
            }

            throw new ArgumentException("Invalid hex format. Use RRGGBB or AARRGGBB.");
        }

        public static Rectangle CenterHorizontally(int y, int width, int height)
            => new(
                (Layout.WinWidth - width) / 2,
                y,
                width,
                height
            );

        public static Rectangle CenterVertically(int x, int width, int height)
            => new(
                x,
                (Layout.WinHeight - height) / 2,
                width,
                height
            );

        // Center both axes
        public static Rectangle Center(int width, int height)
            => new(
                (Layout.WinWidth - width) / 2,
                (Layout.WinHeight - height) / 2,
                width,
                height
            );

        // Center with offset from center
        public static Rectangle Center(int width, int height, int offsetX, int offsetY)
            => new(
                (Layout.WinWidth - width) / 2 + offsetX,
                (Layout.WinHeight - height) / 2 + offsetY,
                width,
                height
            );

        public static Rectangle Below(Rectangle above, int spacing, int width = -1, int height = -1)
        {
            int w = width < 0 ? above.Width : width;
            int h = height < 0 ? above.Height : height;

            return new(above.X, above.Bottom + spacing, w, h);
        }

        public static Rectangle BelowCenter(Rectangle reference, int spacing, int width, int height)
            => new(
                (Layout.WinWidth - width) / 2,
                reference.Bottom + spacing,
                width,
                height
            );

        public static Rectangle WithY(this Rectangle rect, int y)
            => new(
                rect.X,
                y,
                rect.Width,
                rect.Height
            );

        public static Rectangle WithX(this Rectangle rect, int x)
            => new(
                x,
                rect.Y,
                rect.Width,
                rect.Height
            );

        public static string ToSetName(this PieceSet set)
        {
            return set switch
            {
                PieceSet.dubrovny => "Dubrovny",
                PieceSet.cardinal => "Cardinal",
                PieceSet.alfonso => "Alfonso",
                PieceSet.chessicons => "Chess Icons",
                _ => set.ToString()
            };
        }
    }
}
