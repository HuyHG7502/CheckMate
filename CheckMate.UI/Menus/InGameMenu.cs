using CheckMate.Config;
using CheckMate.Helpers;
using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CheckMate.UI.Menus
{
    public class InGameMenu : Menu
    {
        private readonly GameController _game;

        // Timer label
        private Label _timer;
        private Label _state;
        private Label _player;
        private Label _current;
        private Button _aiBtn;
        private Button _depthBtn;
        private Button _pauseBtn;

        private readonly List<IDrawable> _singleOnly = new();

        public override GameState State => GameState.Playing;

        public InGameMenu(GameController game)
        {
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (StateManager.SinglePlayer)
                AddRange(_singleOnly);
            else
                RemoveRange(_singleOnly);

            _pauseBtn.Text = StateManager.GameState == GameState.Paused ? "Resume" : "Pause";
            _depthBtn.Text = $"{StateManager.Depth.ToString()} Level";
            _aiBtn.Text = $"{StateManager.Strategy.ToString()} Algorithm";
            _state.Text = StateManager.GameState == GameState.Paused ? "Paused" : $"Check Mate!\n\n{GameUtil.GetOpponent(StateManager.CurrentPlayer)} Wins";
            _state.IsDrawable =  StateManager.GameState == GameState.Paused || StateManager.GameState == GameState.End;

            if (StateManager.GameState == GameState.Playing)
            {
                _current.Texture = AssetManager.Tiles[AssetManager.LoadTile(StateManager.CurrentPlayer)];
                
                int totalSeconds = (int) StateManager.ElapsedTime;
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;

                _timer.Text = $"{minutes:00}:{seconds:00}";
            }
        }

        public override void Draw(SpriteBatch spriteBatch, AssetManager assetManager)
        {
            base.Draw(spriteBatch, assetManager);
        }

        public override void Init()
        {
            base.Init();

            _elements.Clear();
            
            // Top Left: AI Settings
            InitAI();
            // Top Right: Current Player
            InitHUD();

            // Bottom Left: Gameplay Controls
            InitControls();
            // Bottom Right: Timer and Move History
            InitProgress();

            InitOverlay();
        }

        private void InitHUD()
        {
            var currentLabel = new Label
            {
                Rect = new Rectangle(
                    Layout.WinWidth - Layout.ButtonWidth - Layout.ButtonMargin,
                    Layout.ButtonMargin,
                    Layout.ButtonWidth,
                    Layout.ButtonHeight),
                Text = "Current Player",
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
                Texture = AssetManager.Tiles[TileType.DarkButton],
                OffsetX = 8,
            };

            var currentTile = new Label
            {
                Rect = new Rectangle(
                    currentLabel.Rect.Right - 72,
                    currentLabel.Rect.Top + 8,
                    64, 32),
                Texture = AssetManager.Tiles[AssetManager.LoadTile(StateManager.CurrentPlayer)]
            };

            _current = currentTile;

            AddRange([currentLabel, currentTile]);

            var playerLabel = new Label
            {
                Rect = LayoutUtil.Below(currentLabel.Rect, Layout.ButtonMargin),
                Text = "Player Colour",
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
                Texture = AssetManager.Tiles[TileType.DarkButton],
                OffsetX = 8,
            };

            var playerTile = new Label
            {
                Rect = new Rectangle(
                    playerLabel.Rect.Right - 72,
                    playerLabel.Rect.Top + 8,
                    64, 32),
                Texture = AssetManager.Tiles[AssetManager.LoadTile(StateManager.PlayerColour)]
            };

            _player = playerTile;

            _singleOnly.AddRange([playerLabel, playerTile]);
        }

        private void InitAI()
        {
            var aiBtn = new Button
            {
                Rect = new Rectangle(
                        Layout.ButtonMargin, Layout.ButtonMargin,
                        Layout.ButtonWidth, Layout.ButtonHeight),
                Text = $"{StateManager.Strategy.ToString()} Algorithm",
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
                OffsetX = 8,
            };

            aiBtn.Action = () =>
            {
                StateManager.Strategy = GameUtil.GetNextEnumValue(StateManager.Strategy);
                _game.SetStrategy(StateManager.Strategy);
                aiBtn.Text = $"{StateManager.Strategy.ToString()} Algorithm";
            };

            var depthBtn = new Button
            {
                Rect = LayoutUtil.Below(aiBtn.Rect, Layout.ButtonMargin),
                Text = $"{StateManager.Depth.ToString()} Level",
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
                OffsetX = 8,
            };

            depthBtn.Action = () =>
            {
                StateManager.Depth = GameUtil.GetNextEnumValue(StateManager.Depth);
                depthBtn.Text = $"{StateManager.Depth.ToString()} Level";
            };

            _aiBtn = aiBtn;
            _depthBtn = depthBtn;

            _singleOnly.AddRange([aiBtn, depthBtn]);
        }

        private void InitControls()
        {
            int m = Layout.ButtonMargin;
            int w = Layout.ButtonWidth;
            int h = Layout.ButtonHeight;
            int x = Layout.ButtonMargin;
            int y = Layout.WinHeight - m - h;

            Button AddButton(string text, Action action, bool enabled = true)
            {
                var btn = new Button
                {
                    Rect = new Rectangle(x, y, w, h),
                    Text = text,
                    Font = AssetManager.Font(2),
                    PenColour = enabled ? Color.Black : Colour.Bone,
                    Texture = AssetManager.LoadColoredTexture(enabled ? Colour.Fossil : Colour.Slate),
                    Action = enabled ? action : null,
                };
                y -= (m + h);

                return btn;
            }

            // Start from Bottom Up
            var exit = AddButton("Exit", StateManager.Exit);

            var forfeit = AddButton("Forfeit", () =>
            {
                StateManager.Init();
                StateManager.GameState = GameState.Start;
            });

            var restart = AddButton("Restart", () =>
            {
                _game.Init();
                StateManager.GameState = GameState.Playing;
            });

            var pause = AddButton("Pause", () => { });
            pause.Action = () =>
            {
                StateManager.GameState = StateManager.GameState == GameState.Paused
                    ? GameState.Playing
                    : GameState.Paused;
                pause.Text = StateManager.GameState == GameState.Paused ? "Resume" : "Pause";
            };

            var save = AddButton("Save (Coming Soon)", () => { }, false);
            var undo = AddButton("Undo Move", () => _game.Undo());

            AddRange([exit, forfeit, restart, pause, save, undo]);

            _pauseBtn = pause;
        }

        private void InitProgress()
        {
            int m = Layout.ButtonMargin;
            int w = Layout.ButtonWidth;
            int h = Layout.BoardSize;
            int x = Layout.WinWidth - m - w;
            int y = Layout.WinHeight - m - h;

            // Start from Bottom Up
            var moves = new Label
            {
                Rect = new Rectangle(x, y, w, h),
                Texture = AssetManager.Tiles[TileType.LightButton],
            };

            h = Layout.ButtonHeight;
            y -= (m + h);

            _timer = new Label
            {
                Rect = new Rectangle(x, y, w, h),
                Text = "00:00",
                Font = AssetManager.Font(3),
                PenColour = Color.Black,
                Texture = AssetManager.Tiles[TileType.LightButton],
            };

            AddRange([moves, _timer]);
        }

        private void InitOverlay()
        {
            Rectangle board = new Rectangle(
                    Layout.BoardOffsetX, Layout.BoardOffsetY,
                    Layout.BoardSize, Layout.BoardSize);

            Texture2D overlay = AssetManager.LoadColoredTexture(Color.Black, 0.5f);

            _state = new Label
            {
                Rect = board,
                Font = AssetManager.Font(5),
                PenColour = Color.Gold,
                Texture = overlay,
            };

            Add(_state);
        }
    }
}
