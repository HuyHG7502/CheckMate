using CheckMate.Config;
using CheckMate.Helpers;
using CheckMate.Managers;
using CheckMate.UI.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CheckMate
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly StateManager _state;
        
        private SpriteBatch _spriteBatch;
        private GameController _gameController;
        private MenuController _menuController;

        Task _task;
        KeyboardState _previousKey;

        Dictionary<Keys, double> _previousTime = new();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferMultiSampling = true,
                IsFullScreen = false,
            };

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            Content.RootDirectory = "Assets";
            IsMouseVisible = true;

            _task = Task.CompletedTask;
            _state = new StateManager();
            _previousKey = new KeyboardState();

            _state.Exit = () => Exit();
        }

        protected override void Initialize()
        {
            Window.Title = "Check Mate";

            // TODO: Add your initialization logic here
            // Setup defaul resolution for the project
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.IsBorderless = false;
            Window.Position = Point.Zero;

            Layout.WinWidth = _graphics.PreferredBackBufferWidth;
            Layout.WinHeight = _graphics.PreferredBackBufferHeight;
            Layout.BoardOffsetX = (Layout.WinWidth - Layout.BoardSize) / 2;
            Layout.BoardOffsetY = (Layout.WinHeight - Layout.BoardSize) / 2;
            
            AssetManager _asset = new AssetManager(Content, _graphics.GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameController = new GameController(_state, _asset);
            _menuController = new MenuController(_state, _asset);

            _menuController.Register(new StartMenu(_gameController));
            _menuController.Register(new InGameMenu(_gameController));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override async void Update(GameTime gameTime)
        {       
            if (IsActive)
            {
                CheckInput(gameTime);

                _menuController.Update(gameTime);

                if (_state.GameState == GameState.Playing && _task.IsCompleted)
                {
                    _task = _gameController.Update(gameTime);
                    await _task;
                }
            }

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Colour.Vulcan);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            _gameController.Draw(_spriteBatch, gameTime);
            _menuController.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void CheckInput(GameTime gameTime)
        {
            // Numeric keys dictionary for AI depth/difficulty
            Dictionary<Keys, AIDepth> depths = new()
            {
                { Keys.D1, AIDepth.Basic },
                { Keys.D2, AIDepth.Easy },
                { Keys.D3, AIDepth.Medium },
                { Keys.D4, AIDepth.Hard },
                { Keys.D5, AIDepth.Expert },
            };

            KeyboardState currentKey = Keyboard.GetState();
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            // Add cooldown time to key presses
            bool IsKeyAllowed(Keys key)
            {
                if (!_previousTime.ContainsKey(key))
                {
                    _previousTime[key] = currentTime;
                    return true;
                }

                if (currentTime - _previousTime[key] > Constants.KEY_COOLDOWN)
                {
                    _previousTime[key] = currentTime;
                    return true;
                }

                return false;
            }

            /*
            // On Start screen
            if (_state.GameState == GameState.Start)
            {
                // P key to toggle Single/Two-Player mode
                if (IsKeyAllowed(Keys.P) && GameUtil.IsKeyPressed(Keys.P, currentKey, _previousKey))
                    _state.SinglePlayer = !_state.SinglePlayer;

                // S key to toggle Sides
                if (IsKeyAllowed(Keys.S) && GameUtil.IsKeyPressed(Keys.S, currentKey, _previousKey))
                    _state.PlayerColour = _state.PlayerColour == PieceColour.White ? PieceColour.Black : PieceColour.White;

                // A key to toggle AI Strategy
                if (IsKeyAllowed(Keys.A) && GameUtil.IsKeyPressed(Keys.A, currentKey, _previousKey))
                    _state.Strategy = GameUtil.GetNextEnumValue(_state.Strategy);

                // Numeric keys to toggle AI Depths
                foreach (var depth in depths)
                    if (IsKeyAllowed(depth.Key) && GameUtil.IsKeyPressed(depth.Key, currentKey, _previousKey))
                        _state.Depth = depth.Value;
            }
            */

            // Escape key to pause or exit game
            if (IsKeyAllowed(Keys.Escape) && GameUtil.IsKeyPressed(Keys.Escape, currentKey, _previousKey))
            {
                switch (_state.GameState)
                {
                    case GameState.Paused:
                        _state.Init();
                        _state.GameState = GameState.Start;
                        break;
                    case GameState.Playing:
                        _state.GameState = GameState.Paused;
                        break;
                    default:
                        Exit();
                        break;
                }
            }

            // Space key to pause or resume game
            if (IsKeyAllowed(Keys.Space) && GameUtil.IsKeyPressed(Keys.Space, currentKey, _previousKey))
            {
                switch (_state.GameState)
                {
                    case GameState.End:
                        _state.Init();
                        _state.GameState = GameState.Start;
                        break;
                    case GameState.Paused:
                        _state.GameState = GameState.Playing;
                        break;
                    case GameState.Playing:
                        _state.GameState = GameState.Paused;
                        break;
                }
            }

            // Enter key to play game
            if (IsKeyAllowed(Keys.Enter) && GameUtil.IsKeyPressed(Keys.Enter, currentKey, _previousKey))
            {
                switch (_state.GameState)
                {
                    case GameState.Start:
                        _state.GameState = GameState.Playing;
                        _gameController.Init();
                        break;
                    case GameState.Paused:
                        _state.GameState = GameState.Playing;
                        break;
                }
            }

            // U key to undo a move
            if (IsKeyAllowed(Keys.U) && GameUtil.IsKeyPressed(Keys.U, currentKey, _previousKey))
                _gameController.Undo();

            // R key to reset game
            if (IsKeyAllowed(Keys.R) && GameUtil.IsKeyPressed(Keys.R, currentKey, _previousKey))
                _gameController.Init();

            _previousKey = currentKey;
        }
    }
}