using CheckMate.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CheckMate.Config;

namespace CheckMate.Managers
{
    public class BoardRenderer
    {
        private readonly Board Board;
        private readonly Dictionary<PieceColour, Player> Players;

        private readonly AssetManager Asset;
        private readonly StateManager State;

        private SpriteBatch _spriteBatch;

        public BoardRenderer(Board board, Dictionary<PieceColour, Player> players, AssetManager assetManager, StateManager stateManager)
        {
            Board = board;
            Players = players;
            Asset = assetManager;
            State = stateManager;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State.GameState == GameState.Start) return;

            _spriteBatch = spriteBatch;

            DrawBackground();
            DrawBorders();
            DrawBoard();
            DrawHighlights();
            DrawPieces();
            DrawCaptured();
            DrawState();
        }

        private void DrawBackground()
        {
            // Draw background
            Rectangle screen = new Rectangle(0, 0, Layout.WinWidth, Layout.WinHeight);
            _spriteBatch.Draw(Asset.Tiles[TileType.Background], screen, Color.White);
        }

        private void DrawBorders()
        {
            var outer = Asset.LoadRectangle(
                Layout.BoardOffsetX - Layout.BorderSize,
                Layout.BoardOffsetY - Layout.BorderSize,
                Layout.BoardSize + Layout.BorderSize * 2,
                Layout.BoardSize + Layout.BorderSize * 2);

            var inner = Asset.LoadRectangle(
                Layout.BoardOffsetX - Layout.BorderSize / 4,
                Layout.BoardOffsetY - Layout.BorderSize / 4,
                Layout.BoardSize + Layout.BorderSize / 2,
                Layout.BoardSize + Layout.BorderSize / 2);

            _spriteBatch.Draw(Asset.Tiles[TileType.OuterBorder], outer, Color.White);
            _spriteBatch.Draw(Asset.Tiles[TileType.InnerBorder], inner, Color.White);
        }

        private void DrawBoard()
        {
            var font = Asset.Font(2);

            for (int rank = 0; rank < Layout.SquareCount; rank++)
            {
                for (int file = 0; file < Layout.SquareCount; file++)
                {
                    var square = new Square(rank, file);
                    var tileType = (rank + file) % 2 == 0 ? TileType.White : TileType.Black;
                    var rect = Asset.LoadSquare(square);

                    // Draw square
                    _spriteBatch.Draw(Asset.Tiles[tileType], rect, Color.White);

                    // Labels
                    if (file == 0 || file == Layout.SquareCount - 1)
                    {
                        var rankLabel = square.ToRank();
                        Vector2 size = font.MeasureString(rankLabel);
                        Vector2 pos = new(
                            rect.Center.X - size.X / 2,
                            file == 0 ? rect.Top - size.Y - 12 : rect.Bottom + 12);
                        
                        _spriteBatch.DrawString(font, rankLabel, pos, Color.White);
                    }

                    if (rank == 0 || rank == Layout.SquareCount - 1)
                    {
                        var fileLabel = square.ToFile();
                        Vector2 size = font.MeasureString(fileLabel);
                        Vector2 pos = new(
                            rank == 0 ? rect.Left - size.X - 16 : rect.Right + 16,
                            rect.Center.Y - size.Y / 2);

                        _spriteBatch.DrawString(font, fileLabel, pos, Color.White);
                    }
                }
            }
        }

        private void DrawHighlights()
        {
            if (!State.SelectedSquare.IsNull())
            {
                // Draw selected square
                var selected = Asset.LoadSquare(State.SelectedSquare);
                _spriteBatch.Draw(Asset.Tiles[TileType.Selected], selected, Color.White * 0.8f);

                // Draw allowed moves
                foreach (Move move in State.AllowedMoves)
                {
                    var sqr = Asset.LoadSquare(move.To);
                    _spriteBatch.Draw(Asset.Tiles[TileType.Allowed], sqr, Color.White * 0.8f);
                }

                // Draw disallowed moves
                foreach (Move move in State.DisallowedMoves)
                {
                    var sqr = Asset.LoadSquare(move.To);
                    _spriteBatch.Draw(Asset.Tiles[TileType.Disallowed], sqr, Color.White * 0.8f);
                }
            }

            // Draw checked king
            if (State.ChessState == ChessState.WhiteCheck || State.ChessState == ChessState.BlackCheck)
            {
                var colour = State.ChessState == ChessState.WhiteCheck ? PieceColour.White : PieceColour.Black;
                var kingPos = MoveValidator.LocateKing(Board, colour);
                var sqr = Asset.LoadSquare(kingPos);
                
                _spriteBatch.Draw(Asset.Tiles[TileType.Danger], sqr, Color.White);
            }
        }

        private void DrawPieces()
        {
            foreach (Square square in Board.Squares)
            {
                if (!square.IsOccupied()) continue;

                var rect = Asset.LoadPiece(square);
                var key = (int)square.Piece.Colour * (int)square.Piece.Type;

                _spriteBatch.Draw(Asset.Pieces[key], rect, Color.White);
            }
        }

        private void DrawCaptured()
        {
            foreach (Player player in Players.Values)
            {
                int baseX = player.Colour == PieceColour.White
                    ? Layout.BoardOffsetX - Layout.BorderSize - Layout.SquareSize * 2  // Left side
                    : Layout.BoardOffsetX + Layout.BorderSize + Layout.BoardSize + 24; // Right side

                int baseY = Layout.BoardOffsetY + 12;

                int squareSize = Layout.SquareSize - 24;

                for (int i = 0; i < Layout.SquareCount * 2; i++)
                {
                    int x = i / Layout.SquareCount; // Column
                    int y = i % Layout.SquareCount; // Row

                    var rect = Asset.LoadSquare(x, y, Layout.SquareSize, baseX, baseY);
                    var tile = player.Colour == PieceColour.White ? TileType.White : TileType.Black;

                    rect.Width = rect.Height = squareSize;

                    // Draw slot
                    _spriteBatch.Draw(Asset.Tiles[tile], rect, Color.White);

                    // Draw captured piece
                    if (i < player.CapturedPieces.Count)
                    {
                        Piece piece = player.CapturedPieces[i];
                        
                        var key = (int)piece.Colour * (int)piece.Type;
                        var pieceRect = Asset.LoadPiece(rect);

                        _spriteBatch.Draw(Asset.Pieces[key], pieceRect, Color.White);
                    }
                }
            }
        }

        private void DrawState()
        {
            var font = Asset.Font(2);

            var turn = Asset.LoadRectangle(
                Layout.WinWidth - Layout.ButtonWidth - 10, 10,
                Layout.ButtonWidth, Layout.ButtonHeight);

            var turnTile = Asset.LoadTile(State.CurrentPlayer);

            _spriteBatch.Draw(Asset.Tiles[TileType.DarkButton], turn, Color.White);
            _spriteBatch.Draw(Asset.Tiles[turnTile],
                new Rectangle(turn.Right - 72, turn.Top + 8, 64, 32), Color.White);
            _spriteBatch.DrawString(font, "Current Player",
                new Vector2(turn.Left + 12, turn.Top + (turn.Height - font.MeasureString("Current Player").Y) / 2), Color.Black);

            if (State.SinglePlayer)
            {
                var player = Asset.LoadRectangle(
                    turn.Left, turn.Bottom + 10,
                    Layout.ButtonWidth, Layout.ButtonHeight);

                var playerTile = Asset.LoadTile(State.PlayerColour);

                _spriteBatch.Draw(Asset.Tiles[TileType.DarkButton], player, Color.White);
                _spriteBatch.Draw(Asset.Tiles[playerTile],
                    new Rectangle(player.Right - 72, player.Top + 8, 64, 32), Color.White);
                _spriteBatch.DrawString(font, "Player Colour",
                    new Vector2(player.Left + 12, player.Top + (player.Height - font.MeasureString("Player Colour").Y) / 2), Color.Black);
            }
        }
    }
}
