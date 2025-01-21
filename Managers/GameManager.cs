using CheckMate.Entities;
using CheckMate.Pieces;
using CheckMate.Players;
using CheckMate.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CheckMate.Managers
{
    public class GameManager
    {
        readonly StateManager _stateManager;
        readonly AssetManager _assetManager;

        private Board _board;

        private Player _current;
        private Dictionary<PieceColour, Player> _players;

        private PieceFactory _pieceFactory;

        private Task<Move> _task;

        double calculationTime;

        public GameManager(StateManager stateManager, AssetManager assetManager)
        {
            _assetManager = assetManager;
            _stateManager = stateManager;
            _pieceFactory = new PieceFactory();

            Init();
        }

        // Init method
        // - Initialises the board
        // - Initialises the players according to StateManager
        public void Init()
        {
            _stateManager.Init();

            _board = new Board(_pieceFactory);

            _players = new Dictionary<PieceColour, Player>()
            {
                { _stateManager.PlayerColour, new HumanPlayer(_stateManager.PlayerColour, _board) },
                {  Util.GetOpponent(_stateManager.PlayerColour),
                    _stateManager.SinglePlayer
                    ? new AIPlayer(Util.GetOpponent(_stateManager.PlayerColour), _board, _stateManager.Strategy, _stateManager.Depth)
                    : new HumanPlayer(Util.GetOpponent(_stateManager.PlayerColour), _board) }
            };

            // Set current player to White
            _current = _players[_stateManager.CurrentPlayer];
            foreach (Square sqr in _board.Squares)
                if (sqr.IsOccupied())
                    // Add pieces to the respective players
                    if (sqr.Piece.IsMine(_stateManager.CurrentPlayer))
                        _players[_current.Colour].AddPiece(sqr.Piece);
                    else
                        _players[Util.GetOpponent(_current.Colour)].AddPiece(sqr.Piece);

            // Action invocation from _board to update Piece list on Pawn Promotion
            _board.OnPiecePromoted = (pawn, queen) =>
            {
                _players[queen.Colour].RemovePiece(pawn);
                _players[queen.Colour].AddPiece(queen);
            };

            // Action invocation from _board to update Piece list on undoing Pawn Promotion
            _board.OnPiecePromotedUndone = (pawn, queen) =>
            {
                _players[pawn.Colour].AddPiece(pawn);
                _players[pawn.Colour].RemovePiece(queen);
            };
        }

        public void Undo()
        {
            // Move stack is empty == Chess state is Opening
            if (_stateManager.Moves.Count == 0)
            {
                _stateManager.ChessState = ChessState.Opening;
                return;
            }

            // Get the last move to undo
            Move last = _stateManager.Moves.Pop();

            _board.UndoMove(last);
            if (!last.Captured.IsNull())
                _players[last.Captured.Colour].AddPiece(last.Captured);

            SwitchTurn();

            // If Human vs AI
            // Undo one more time
            if (_stateManager.SinglePlayer && _stateManager.Moves.Count > 0)
            {
                last = _stateManager.Moves.Pop();

                _board.UndoMove(last);
                if (!last.Captured.IsNull())
                    _players[last.Captured.Colour].AddPiece(last.Captured);

                SwitchTurn();
            }
        }

        public async Task Update(GameTime gameTime)
        {
            if (_stateManager.GameState == GameState.End) return;

            // End game upon Checkmate
            if (MoveValidator.DetectCheckmate(_board, _stateManager.CurrentPlayer))
            {
                _stateManager.GameState = GameState.End;
                return;
            }

            if (MoveValidator.DetectCheck(_board, _stateManager.CurrentPlayer))
                _stateManager.ChessState = _current.Colour == PieceColour.White ? ChessState.WhiteCheck : ChessState.BlackCheck;
            else
                _stateManager.ChessState = _stateManager.ChessState == ChessState.Opening ? ChessState.Opening : ChessState.Default;

            _task = _current.MakeMove(gameTime, _stateManager);
            Move move = await _task;

            if (_board.MakeMove(move, out Piece captured))
            {
                // Logging
                Debug.WriteLine($"#{_stateManager.Moves.Count() + 1}: {Util.GetPiece(move.Moved)} from {move.From} to {move.To} - {Util.GetPiece(move.Captured)}");
                // Push latest move to Move stack
                _stateManager.Moves.Push(move);
                _stateManager.ChessState = ChessState.Default;

                if (!captured.IsNull())
                    _players[captured.Colour].RemovePiece(captured);

                SwitchTurn();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Dictionary<int, SpriteFont> fonts = _assetManager.Fonts;
            Rectangle rect;
            Rectangle screen = new(0, 0, Constants.WIN_WIDTH, Constants.WIN_HEIGHT);

            // Draw background
            spriteBatch.Draw(_assetManager.LoadTexture("Background"), screen, Color.White);
            spriteBatch.Draw(_assetManager.Tiles[TileType.Paused], screen, Color.White);

            if (_stateManager.GameState == GameState.Playing)
            {
                // Draw board
                for (int rank = 0; rank < Constants.SQUARE_NUM; rank++)
                {
                    for (int file = 0; file < Constants.SQUARE_NUM; file++)
                    {
                        Texture2D tile = (rank + file) % 2 == 0 ? _assetManager.Tiles[TileType.White] : _assetManager.Tiles[TileType.Black];
                        rect = _assetManager.LoadSquare(rank, file, Constants.SQUARE_SIZE, Constants.SQUARE_SIZE);
                        spriteBatch.Draw(tile, rect, Color.White);

                        string rankStr = new Square(rank, file).ToRank();
                        string fileStr = new Square(rank, file).ToFile();

                        // Draw square ranks and files
                        if (file == 7)
                            spriteBatch.DrawString(fonts[2], rankStr, new Vector2(rect.Right - 5, rect.Bottom - 5) - fonts[2].MeasureString(rankStr), Color.Black);

                        if (rank == 0)
                            spriteBatch.DrawString(fonts[2], fileStr, new Vector2(rect.Left + 5, rect.Top + 5), Color.Black);
                    }
                }

                // Draw selected square
                if (!_stateManager.SelectedSquare.IsNull())
                {
                    rect = _assetManager.LoadSquare(_stateManager.SelectedSquare.Rank, _stateManager.SelectedSquare.File,
                        Constants.SQUARE_SIZE, Constants.SQUARE_SIZE);
                    spriteBatch.Draw(_assetManager.Tiles[TileType.Selected], rect, Color.White * 0.8f);

                    // Draw allowed moves
                    foreach (var move in _stateManager.AllowedMoves)
                    {
                        rect = _assetManager.LoadSquare(move.To.Rank, move.To.File,
                            Constants.SQUARE_SIZE, Constants.SQUARE_SIZE);
                        spriteBatch.Draw(_assetManager.Tiles[TileType.Allowed], rect, Color.White * 0.8f);
                    }

                    // Draw disallowed moves
                    foreach (var move in _stateManager.DisallowedMoves)
                    {
                        rect = _assetManager.LoadSquare(move.To.Rank, move.To.File,
                            Constants.SQUARE_SIZE, Constants.SQUARE_SIZE);
                        spriteBatch.Draw(_assetManager.Tiles[TileType.Disallowed], rect, Color.White * 0.8f);
                    }
                }

                // Draw checked king
                if (_stateManager.ChessState == ChessState.WhiteCheck || _stateManager.ChessState == ChessState.BlackCheck)
                {
                    PieceColour colour = _stateManager.ChessState == ChessState.WhiteCheck ? PieceColour.White : PieceColour.Black;
                    Square kingPos = MoveValidator.LocateKing(_board, colour);

                    rect = _assetManager.LoadSquare(kingPos.Rank, kingPos.File,
                        Constants.SQUARE_SIZE, Constants.SQUARE_SIZE);
                    spriteBatch.Draw(_assetManager.Tiles[TileType.Danger], rect, Color.White);
                }

                // Draw pieces
                foreach (var square in _board.Squares)
                {
                    if (square.IsOccupied())
                    {
                        int rank = square.Rank * Constants.SQUARE_SIZE + (Constants.SQUARE_SIZE - Constants.PIECE_SIZE) / 2 + Constants.WIN_PADDING;
                        int file = square.File * Constants.SQUARE_SIZE + (Constants.SQUARE_SIZE - Constants.PIECE_SIZE) / 2 + Constants.WIN_PADDING;

                        rect = new(rank, file, Constants.PIECE_SIZE, Constants.PIECE_SIZE);
                        spriteBatch.Draw(_assetManager.Pieces[(int)square.Piece.Colour * (int)square.Piece.Type], rect, Color.White);
                    }
                }

                // Draw side menu
                // spriteBatch.Draw(_assetManager.LoadColoredTexture(Color.Gray),
                //    _assetManager.LoadRectangle(Constants.SQUARE_NUM * Constants.SQUARE_SIZE + Constants.WIN_PADDING, 0, Constants.MENU_SIZE, Constants.BOARD_SIZE, true, false), Color.White);


                // Track AI calculation time
                calculationTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (calculationTime > 0.5 && !_stateManager.IsPlayerTurn())
                {
                    spriteBatch.Draw(_assetManager.Tiles[TileType.Paused], screen, Color.White * 0.5f);

                    string str = "Calculating...";
                    Vector2 pos = _assetManager.LoadString(fonts[4], str, screen);

                    spriteBatch.DrawString(fonts[4], str, pos, Color.White);
                }
            }

            /*
            if (_stateManager.GameState == GameState.Paused)
            {
                string str = "Press Enter to resume, Escape to exit";
                Vector2 pos = _assetManager.LoadString(fonts[4], str, screen);

                spriteBatch.DrawString(fonts[4], str, pos, Color.Khaki);
            }
            
            if (_stateManager.GameState == GameState.Start || _stateManager.GameState == GameState.End)
            {
                Vector2 pos;

                string head, info, play;
                if (_stateManager.GameState == GameState.Start)
                {
                    head = "Check Mate";
                    info = $"P:     {(_stateManager.SinglePlayer ? "Single Player" : "Two Players")}"
                        + (_stateManager.SinglePlayer
                            ? $"\nS:     Play as {_stateManager.PlayerColour}"
                            + $"\nA:     Strategy: {_stateManager.Strategy}"
                            + $"\n1-5:   Difficulty: {_stateManager.Depth}"
                            : "")
                        + "\n"
                        + "\nR:     Reset"
                        + "\nU:     Undo";
                    play = "Press Enter to play";
                }
                else
                {
                    head = $"{Util.GetOpponent(_current.Colour)} Wins!";
                    info = "";
                    play = "Press Space to replay";
                }

                pos = _assetManager.LoadString(fonts[5], head, new Rectangle(0, 150, screen.Width, screen.Height), centerY: false);
                spriteBatch.DrawString(fonts[5], head, pos, Color.Gold);

                pos = _assetManager.LoadString(fonts[3], info, new Rectangle(200, 250, screen.Width, screen.Height), false, false);
                spriteBatch.DrawString(fonts[3], info, pos, Color.White);

                pos = _assetManager.LoadString(fonts[4], play, new Rectangle(0, 600, screen.Width, screen.Height), centerY: false);
                spriteBatch.DrawString(fonts[4], play, pos, Color.Khaki);
            }
            */
        }

        private void SwitchTurn()
        {
            _stateManager.CurrentPlayer = Util.GetOpponent(_stateManager.CurrentPlayer);
            _stateManager.Clear();

            _current = _players[_stateManager.CurrentPlayer];

            if (!_stateManager.IsPlayerTurn())
                calculationTime = 0;
        }
    }
}
