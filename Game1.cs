using CheckMate.Managers;
using CheckMate.UI;
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
        private GameManager _gameManager;
        private Menu _menu;

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
        }

        protected override void Initialize()
        {
            Window.Title = "Check Mate";

            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = Constants.WIN_WIDTH;
            _graphics.PreferredBackBufferHeight = Constants.WIN_HEIGHT;

            _graphics.ApplyChanges();

            AssetManager _asset = new AssetManager(Content, _graphics.GraphicsDevice);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameManager = new GameManager(_state, _asset);
            _menu = new Menu(_gameManager, _state, _asset, () => Exit());

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
                _menu.Update(gameTime);

                if (_state.GameState == GameState.Playing && _task.IsCompleted)
                {
                    _task = _gameManager.Update(gameTime);
                    await _task;
                }
            }

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            _gameManager.Draw(_spriteBatch, gameTime);
            _menu.Draw(_spriteBatch, gameTime);

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
                if (IsKeyAllowed(Keys.P) && Util.IsKeyPressed(Keys.P, currentKey, _previousKey))
                    _state.SinglePlayer = !_state.SinglePlayer;

                // S key to toggle Sides
                if (IsKeyAllowed(Keys.S) && Util.IsKeyPressed(Keys.S, currentKey, _previousKey))
                    _state.PlayerColour = _state.PlayerColour == PieceColour.White ? PieceColour.Black : PieceColour.White;

                // A key to toggle AI Strategy
                if (IsKeyAllowed(Keys.A) && Util.IsKeyPressed(Keys.A, currentKey, _previousKey))
                    _state.Strategy = Util.GetNextEnumValue(_state.Strategy);

                // Numeric keys to toggle AI Depths
                foreach (var depth in depths)
                    if (IsKeyAllowed(depth.Key) && Util.IsKeyPressed(depth.Key, currentKey, _previousKey))
                        _state.Depth = depth.Value;
            }
            */

            // Escape key to pause or exit game
            if (IsKeyAllowed(Keys.Escape) && Util.IsKeyPressed(Keys.Escape, currentKey, _previousKey))
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
            if (IsKeyAllowed(Keys.Space) && Util.IsKeyPressed(Keys.Space, currentKey, _previousKey))
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
            if (IsKeyAllowed(Keys.Enter) && Util.IsKeyPressed(Keys.Enter, currentKey, _previousKey))
            {
                switch (_state.GameState)
                {
                    case GameState.Start:
                        _state.GameState = GameState.Playing;
                        _gameManager.Init();
                        break;
                    case GameState.Paused:
                        _state.GameState = GameState.Playing;
                        break;
                }
            }

            // U key to undo a move
            if (IsKeyAllowed(Keys.U) && Util.IsKeyPressed(Keys.U, currentKey, _previousKey))
                _gameManager.Undo();

            // R key to reset game
            if (IsKeyAllowed(Keys.R) && Util.IsKeyPressed(Keys.R, currentKey, _previousKey))
                _gameManager.Init();

            _previousKey = currentKey;
        }
    }
}