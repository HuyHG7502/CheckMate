using CheckMate.Config;

namespace CheckMate.Entities
{
    public class Square
    {
        private readonly string _ranks = "abcdefgh";
        public int Rank { get; }
        public int File { get; }
        public Piece Piece { get; set; }

        public Square(int rank, int file)
        {
            File = file;
            Rank = rank;
            Piece = Piece.Null;
        }
        public Square(Square other)
        {
            File = other.File;
            Rank = other.Rank;
            Piece = other.Piece.Clone();
        }

        public Square()
        {
            File = -1;
            Rank = -1;
            Piece = Piece.Null;
        }

        public static Square Null => new();

        public bool IsNull()
        {
            return File == -1 || Rank == -1;
        }

        public bool IsOccupied()
        {
            return !Piece.IsNull();
        }

        public bool IsInBounds(int lower, int upper)
        {
            return Rank >= lower && Rank < upper
                && File >= lower && File < upper;
        }

        public string ToRank()
        {
            return _ranks[Rank].ToString();
        }

        public string ToFile()
        {
            return (Layout.SquareCount - File).ToString();
        }

        public override string ToString()
        {
            return $"{_ranks[Rank]}{Layout.SquareCount - File}";
        }

        public static Square operator +(Square sqr, int[] dir)
        {
            return new Square(sqr.Rank + dir[0], sqr.File + dir[1]);
        }

        public static Square operator -(Square sqr, int[] dir)
        {
            return new Square(sqr.Rank - dir[0], sqr.File - dir[1]);
        }

        public static Square operator *(Square sqr, int val)
        {
            return new Square(sqr.Rank * val, sqr.File * val);
        }
    }
}
