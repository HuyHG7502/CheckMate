using CheckMate.Entities;
using CheckMate;
using CheckMate.Managers;

namespace Tests
{
    [TestFixture]
    public class BoardTests
    {
        private Board _board;

        [SetUp]
        public void Setup()
        {
            _board = new Board();
        }

        [Test]
        public void TestBoardInitialise()
        {
            // Test if there are 32 Pieces: 16 of each Colour
            Assert.That(_board.Pieces(PieceColour.White).Count + _board.Pieces(PieceColour.Black).Count, Is.EqualTo(32));
            // Test that King pieces are in correct init positions
            Assert.That(MoveValidator.LocateKing(_board, PieceColour.White), Is.EqualTo(_board[4, 7]));
            Assert.That(MoveValidator.LocateKing(_board, PieceColour.Black), Is.EqualTo(_board[4, 0]));
        }

        [Test]
        public void TestBoardMakeMove()
        {
            // Move a White Pawn
            var fromSqr = _board[4, 6];
            var toSqr = _board[4, 4];

            var move = new Move(fromSqr, toSqr);
            _board.MakeMove(move, out var captured);

            Assert.That(fromSqr.Piece.IsNull(), Is.True);
            Assert.That(!toSqr.Piece.IsNull()
                && toSqr.Piece.IsMine(PieceColour.White)
                && toSqr.Piece.Type == PieceType.Pawn, Is.True);

            _board.UndoMove(move);
            Assert.That(toSqr.Piece.IsNull(), Is.True);
            Assert.That(!fromSqr.Piece.IsNull()
                && fromSqr.Piece.IsMine(PieceColour.White)
                && fromSqr.Piece.Type == PieceType.Pawn, Is.True);
        }
    }
}