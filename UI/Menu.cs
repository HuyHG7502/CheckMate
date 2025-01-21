using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CheckMate.UI
{
    public class Menu
    {
        readonly StateManager _stateManager;
        readonly AssetManager _assetManager;
        readonly GameManager _gameManager;

        readonly Action _exitAction;

        List<IDrawable> setup = [];

        List<IDrawable> main = [];
        List<IDrawable> paused = [];
        List<IDrawable> end = [];

        MouseState _previous = Mouse.GetState();

        readonly Dictionary<GameState, List<IDrawable>> menus;

        readonly Dictionary<int, AIDepth> depths = new()
        {
            { 1, AIDepth.Basic },
            { 2, AIDepth.Easy },
            { 3, AIDepth.Medium },
            { 4, AIDepth.Hard },
            { 5, AIDepth.Expert },
        };

        Rectangle button;

        public Menu(GameManager gameManager, StateManager stateManager, AssetManager assetManager, Action exitAction)
        {
            _stateManager = stateManager;
            _assetManager = assetManager;
            _gameManager = gameManager;

            Button.Base = _assetManager.LoadColoredTexture(Color.Tan);
            Button.Highlight = _assetManager.LoadColoredTexture(Color.White);

            _exitAction = exitAction;

            InitStartMenu();
            InitPausedMenu();
            InitEndMenu();

            menus = new()
            {
                [GameState.Start] = main,
                [GameState.Paused] = paused,
                [GameState.End] = end,
            };
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_stateManager.GameState == GameState.Playing) return;

            foreach (var ui in menus[_stateManager.GameState])
            {
                // Toggle visibility of Single-Player-only setups
                foreach (var i in setup)
                    i.IsDrawable = _stateManager.SinglePlayer ? true : false;

                ui.Draw(spriteBatch, _assetManager);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_stateManager.GameState == GameState.Playing) return;
           
            foreach (var ui in menus[_stateManager.GameState])
                ui.Update(gameTime);        }

        private void InitStartMenu()
        {
            Label label = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 60, 100, 120, 64),
                Text = "Check Mate",
                PenColour = Color.Gold,
                Font = _assetManager.Fonts[6]
            };
            main.Add(label);

            Button singlePlayer = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - Constants.WIN_PADDING - 160, label.Rect.Bottom + 48, 160, 160),
                Texture = _assetManager.LoadTexture("white"),
                IsClicked = _stateManager.SinglePlayer == true
            };
            Label single = new()
            {
                Rect = new(singlePlayer.Rect.X, singlePlayer.Rect.Bottom + 16, singlePlayer.Rect.Width, 20),
                Text = "Single Player",
                PenColour = singlePlayer.IsClicked ? Color.Wheat : Color.Gray,
                Font = _assetManager.Fonts[2]
            };

            Button twoPlayer = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 + Constants.WIN_PADDING, singlePlayer.Rect.Y, 160, 160),
                Texture = _assetManager.LoadTexture("two"),
                IsClicked = _stateManager.SinglePlayer == false
            };
            Label two = new()
            {
                Rect = new(twoPlayer.Rect.X, twoPlayer.Rect.Bottom + 16, twoPlayer.Rect.Width, 20),
                Text = "Two Players",
                PenColour = twoPlayer.IsClicked ? Color.Wheat : Color.Gray,
                Font = _assetManager.Fonts[2]
            };

            singlePlayer.Action = () =>
            {
                _stateManager.SinglePlayer = true;
                singlePlayer.IsClicked = true;
                twoPlayer.IsClicked = false;
                
                single.PenColour = Color.Wheat;
                two.PenColour = Color.Gray;
            };

            twoPlayer.Action = () =>
            {
                _stateManager.SinglePlayer = false;
                singlePlayer.IsClicked = false;
                twoPlayer.IsClicked = true;

                single.PenColour = Color.Gray;
                two.PenColour = Color.Wheat;
            };

            main.Add(singlePlayer);
            main.Add(single);
            main.Add(twoPlayer);
            main.Add(two);

            Label you = new()
            {
                Rect = new(singlePlayer.Rect.X, singlePlayer.Rect.Bottom + 64, 160, 40),
                Text = "Play as",
                PenColour = Color.Ivory,
                Font = _assetManager.Fonts[3]
            };
            setup.Add(you);

            Label opp = new()
            {
                Rect = new(twoPlayer.Rect.X, singlePlayer.Rect.Bottom + 64, 160, 40),
                Text = "Algorithm",
                PenColour = Color.Ivory,
                Font = _assetManager.Fonts[3]
            };
            setup.Add(opp);

            Button side = new()
            {
                Rect = new(singlePlayer.Rect.X, you.Rect.Bottom, 160, 160),
                Texture = _assetManager.LoadTexture("white"),
            };
            side.Action = () =>
            {
                _stateManager.PlayerColour = _stateManager.PlayerColour == PieceColour.White ? PieceColour.Black : PieceColour.White;
                side.Texture = _stateManager.PlayerColour == PieceColour.White ? _assetManager.LoadTexture("white") : _assetManager.LoadTexture("black");
            };
            setup.Add(side);

            Button ai = new()
            {
                Rect = new(twoPlayer.Rect.X, opp.Rect.Bottom + 24, 160, 48),
                Text = _stateManager.Strategy.ToString(),
                Font = _assetManager.Fonts[3],
                PenColour = Color.Black,
            };
            ai.Action = () =>
            {
                _stateManager.Strategy = Util.GetNextEnumValue(_stateManager.Strategy);
                ai.Text = _stateManager.Strategy.ToString();
            };
            setup.Add(ai);

            Button depth = new()
            {
                Rect = new(twoPlayer.Rect.X, ai.Rect.Bottom + 24, 160, 48),
                Text = _stateManager.Depth.ToString(),
                Font = _assetManager.Fonts[3],
                PenColour = Color.Black,
            };
            depth.Action = () =>
            {
                _stateManager.Depth = Util.GetNextEnumValue(_stateManager.Depth);
                depth.Text = _stateManager.Depth.ToString();
            };
            setup.Add(depth);

            Button start = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 200, side.Rect.Bottom + 48, 400, 60),
                Text = "All Set",
                Font = _assetManager.Fonts[4],
                Texture = _assetManager.LoadColoredTexture(Color.Gold),
                PenColour = Color.Black,
                Action = () =>
                {
                    _stateManager.GameState = GameState.Playing;
                    _gameManager.Init();

                    Util.AwaitMouseClick();
                }
            };
            main.Add(start);

            Button exit = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 200, start.Rect.Bottom + 24, 400, 48),
                Text = "Exit",
                Font = _assetManager.Fonts[3],
                Texture = _assetManager.LoadColoredTexture(Color.Gray),
                PenColour = Color.Ivory,
                Action = _exitAction
            };
            main.Add(exit);
            main.AddRange(setup);
        }
    
        private void InitPausedMenu()
        {
            Label label = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 60, Constants.WIN_HEIGHT / 4, 120, 48),
                Text = "Paused",
                PenColour = Color.Gold,
                Font = _assetManager.Fonts[5]
            };
            paused.Add(label);

            Button resume = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, label.Rect.Bottom + 64, 200, 48),
                Text = "Resume",
                PenColour = Color.Black,
                Font = _assetManager.Fonts[3],
                Action = () =>
                {
                    _stateManager.GameState = GameState.Playing;
                }
            };
            paused.Add(resume);

            Button reset = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, resume.Rect.Bottom + 24, 200, 48),
                Text = "Reset",
                PenColour = Color.Black,
                Font = _assetManager.Fonts[3],
                Action = () =>
                {
                    _gameManager.Init();
                    _stateManager.GameState = GameState.Playing;
                }
            };
            paused.Add(reset);

            Button forfeit = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, reset.Rect.Bottom + 24, 200, 48),
                Text = "Forfeit",
                PenColour = Color.Black,
                Font = _assetManager.Fonts[3],
                Action = () =>
                {
                    _gameManager.Init();
                    _stateManager.GameState = GameState.Start;
                }
            };
            paused.Add(forfeit);

            Button exit = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, forfeit.Rect.Bottom + 64, 200, 48),
                Text = "Exit",
                Font = _assetManager.Fonts[3],
                Texture = _assetManager.LoadColoredTexture(Color.Gray),
                PenColour = Color.Ivory,
                Action = _exitAction
            };
            paused.Add(exit);
        }

        private void InitEndMenu()
        {
            Label label = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 60, Constants.WIN_HEIGHT / 4, 120, 80),
                Text = $"{Util.GetOpponent(_stateManager.CurrentPlayer)} Wins!",
                PenColour = Color.Gold,
                Font = _assetManager.Fonts[5]
            };
            end.Add(label);

            Button reset = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, Constants.WIN_HEIGHT / 2 - 48, 200, 48),
                Text = "Return",
                PenColour = Color.Black,
                Font = _assetManager.Fonts[3],
                Action = () =>
                {
                    _gameManager.Init();
                    _stateManager.GameState = GameState.Start;
                }
            };
            end.Add(reset);

            Button exit = new()
            {
                Rect = new(Constants.WIN_WIDTH / 2 - 100, Constants.WIN_HEIGHT / 2 + 24, 200, 48),
                Text = "Exit",
                Font = _assetManager.Fonts[3],
                Texture = _assetManager.LoadColoredTexture(Color.Gray),
                PenColour = Color.Ivory,
                Action = _exitAction
            };
            end.Add(exit);
        }
    }
}
