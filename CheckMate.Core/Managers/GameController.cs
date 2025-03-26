using CheckMate.Entities;
using CheckMate.Helpers;
using CheckMate.Interfaces;
using CheckMate.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace CheckMate.Managers
{
    public class GameController : IObserver
    {
        readonly StateManager _stateManager;
        readonly AssetManager _assetManager;

        private Board _board;

        private Player _current;
        private Dictionary<PieceColour, Player> _players;

        private Task<Move> _task;

        double calculationTime;

        public GameController(StateManager stateManager, AssetManager assetManager)
        {
            _assetManager = assetManager;
            _stateManager = stateManager;

            Init();
        }

        public void Update(Object obj1, Object obj2)
        {
            if (obj1 is not Piece && obj2 is not Piece) return;

            Piece oldPiece = (Piece)obj1;
            Piece newPiece = (Piece)obj2;

            _players[newPiece.Colour].RemovePiece(oldPiece);
            _players[newPiece.Colour].AddPiece(newPiece);
        }

        // Init method
        // - Initialises the board
        // - Initialises the players according to StateManager
        public void Init()
        {
            _stateManager.Init();

            _board = new Board();
            _board.Attach(this);

            _players = new Dictionary<PieceColour, Player>()
            {
                { _stateManager.PlayerColour, new HumanPlayer(_stateManager.PlayerColour, _board) },
                {  GameUtil.GetOpponent(_stateManager.PlayerColour),
                    _stateManager.SinglePlayer
                    ? new AIPlayer(GameUtil.GetOpponent(_stateManager.PlayerColour), _board, _stateManager.Strategy, _stateManager.Depth)
                    : new HumanPlayer(GameUtil.GetOpponent(_stateManager.PlayerColour), _board) }
            };

            // Attach player observers
            foreach (Player player in _players.Values)
            {
                _board.Attach(player);
            }

            // Set current player to White
            _current = _players[_stateManager.CurrentPlayer];
            foreach (Square sqr in _board.Squares)
                if (sqr.IsOccupied())
                    // Add pieces to the respective players
                    if (sqr.Piece.IsMine(_stateManager.CurrentPlayer))
                        _players[_current.Colour].AddPiece(sqr.Piece);
                    else
                        _players[GameUtil.GetOpponent(_current.Colour)].AddPiece(sqr.Piece);

            // Action invocation from Board to update Piece list on Pawn Promotion
            _board.OnPiecePromoted = (pawn, queen) =>
            {
                _players[queen.Colour].RemovePiece(pawn);
                _players[queen.Colour].AddPiece(queen);
            };

            // Action invocation from Board to update Piece list on undoing Pawn Promotion
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
            {
                _players[last.Captured.Colour].AddPiece(last.Captured);
                _players[last.Captured.Colour].CapturedPieces.Remove(last.Captured);
            }

            SwitchTurn();

            // If Human vs AI
            // Undo one more time
            if (_stateManager.SinglePlayer && _stateManager.Moves.Count > 0)
            {
                last = _stateManager.Moves.Pop();

                _board.UndoMove(last);
                if (!last.Captured.IsNull())
                {
                    _players[last.Captured.Colour].AddPiece(last.Captured);
                    _players[last.Captured.Colour].CapturedPieces.Remove(last.Captured);
                }

                SwitchTurn();
            }
        }

        public async Task Update(GameTime gameTime)
        {
            if (_stateManager.GameState == GameState.Paused || _stateManager.GameState == GameState.End)
                return;

            _stateManager.ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            // End game upon Checkmate
            if (MoveValidator.DetectCheckmate(_board, _stateManager.CurrentPlayer))
            {
                _stateManager.GameState = GameState.End;
                return;
            }

            if (MoveValidator.DetectCheck(_board, _stateManager.CurrentPlayer))
                _stateManager.ChessState = _stateManager.CurrentPlayer == PieceColour.White ? ChessState.WhiteCheck : ChessState.BlackCheck;
            else
                _stateManager.ChessState = _stateManager.ChessState == ChessState.Opening ? ChessState.Opening : ChessState.Default;

            _task = _current.MakeMove(gameTime, _stateManager);
            Move move = await _task;

            if (_board.MakeMove(move, out Piece captured))
            {
                // Logging
                Debug.WriteLine($"#{_stateManager.Moves.Count() + 1}: {move.Moved.ToPieceNotation()} from {move.From} to {move.To} - {move.Captured.ToPieceNotation()}");
                // Push latest move to Move stack
                _stateManager.Moves.Push(move);
                _stateManager.ChessState = ChessState.Default;

                if (!captured.IsNull())
                {
                    _players[captured.Colour].RemovePiece(captured);
                    _players[captured.Colour].CapturedPieces.Add(captured);
                }

                SwitchTurn();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            BoardRenderer boardRenderer = new(_board, _players, _assetManager, _stateManager);
            boardRenderer.Draw(spriteBatch, gameTime);
        }

        private void SwitchTurn()
        {
            _stateManager.CurrentPlayer = GameUtil.GetOpponent(_stateManager.CurrentPlayer);
            _stateManager.Clear();

            _current = _players[_stateManager.CurrentPlayer];

            if (!_stateManager.IsPlayerTurn())
                calculationTime = 0;
        }

        public void SetStrategy(AIStrategy strategy)
        {
            if (_stateManager.SinglePlayer && _players.Values.FirstOrDefault(p => p is AIPlayer) is AIPlayer aiPlayer)
                aiPlayer.SetStrategy(strategy);
        }
    }
}
