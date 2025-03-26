using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Config
{
    public static class Layout
    {
        public const int SquareCount = 8;
        public const int SquareSize = 96;
        public const int BorderSize = 36;
        
        public const int ButtonWidth = 240;
        public const int ButtonHeight = 48;
        public const int ButtonMargin = 10;

        public const int PieceSize = (int)(SquareSize * 0.6);
        public const int BoardSize = SquareCount * SquareSize;

        public static int WinWidth { get; set; } = 800;
        public static int WinHeight { get; set; } = 800;
        public static int BoardOffsetX { get; set; } = 0;
        public static int BoardOffsetY { get; set; } = 0;
    }
}
