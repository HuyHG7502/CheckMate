using CheckMate.Config;
using CheckMate.Helpers;
using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.UI.Menus
{
    public class StartMenu : Menu
    {
        private readonly GameController _game;

        private readonly List<IDrawable> _setup = new();

        private Label _title;
        private Button _singleBtn;
        private Button _twoBtn;

        public override GameState State => GameState.Start;

        public StartMenu(GameController game)
        {
            _game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Only add these if Single Player mode is selected
            if (StateManager.SinglePlayer)
                AddRange(_setup);
            else
                RemoveRange(_setup);

            base.Update(gameTime);
        }

        public override void Init()
        {
            base.Init();

            _elements.Clear();

            // Title
            var title = new Label
            {
                Rect = LayoutUtil.Center(400, 64, 0, -Layout.WinHeight / 3),
                Text = "Check Mate",
                Font = AssetManager.Font(6, FontStyle.Bold),
                PenColour = Colour.Mustard
            };
            
            _title = title;

            Add(title);

            InitPlayModes();
            InitSetup();
            InitControls();
        }

        private void InitPlayModes()
        {
            var offset = _title.Rect.Right - 160;

            // Single / Two Player Buttons
            var singleBtn = new Button
            {
                Rect = LayoutUtil.Below(_title.Rect, 64, 160, 160),
                Texture = AssetManager.LoadTexture("as-white"),
                IsClicked = StateManager.SinglePlayer,
            };

            var singleLabel = new Label
            {
                Rect = LayoutUtil.Below(singleBtn.Rect, 20, -1, 20),
                Text = "Single Player",
                Font = AssetManager.Font(2),
                PenColour = singleBtn.IsClicked ? Colour.Karry : Colour.Fossil
            };

            var twoBtn = new Button
            {
                Rect = LayoutUtil.Below(_title.Rect.WithX(offset), 64, 160, 160),
                Texture = AssetManager.LoadTexture("as-two"),
                IsClicked = !StateManager.SinglePlayer,
            };

            var twoLabel = new Label
            {
                Rect = LayoutUtil.Below(twoBtn.Rect, 20, -1, 20),
                Text = "Two Players",
                Font = AssetManager.Font(2),
                PenColour = twoBtn.IsClicked ? Colour.Karry : Colour.Fossil
            };

            singleBtn.Action = () =>
            {
                StateManager.SinglePlayer = true;
                singleBtn.IsClicked = true;
                twoBtn.IsClicked = false;

                singleLabel.PenColour = Colour.Karry;
                twoLabel.PenColour = Colour.Fossil;
            };

            twoBtn.Action = () =>
            {
                StateManager.SinglePlayer = false;
                singleBtn.IsClicked = false;
                twoBtn.IsClicked = true;

                singleLabel.PenColour = Colour.Fossil;
                twoLabel.PenColour = Colour.Karry;
            };

            AddRange([singleBtn, singleLabel, twoBtn, twoLabel]);

            _singleBtn = singleBtn;
            _twoBtn = twoBtn;
        }
    
        private void InitSetup()
        {
            var playAs = new Label
            {
                Rect = LayoutUtil.Below(_singleBtn.Rect, 80, -1, 20),
                Text = "Play as",
                Font = AssetManager.Font(3),
                PenColour = Colour.Bone,
            };

            var pieceBtn = new Button
            {
                Rect = LayoutUtil.Below(playAs.Rect, 20, 160, 160),
                Texture = AssetManager.LoadTexture(StateManager.PlayerColour == PieceColour.White ? "as-white" : "as-black"),
            };

            pieceBtn.Action = () =>
            {
                StateManager.PlayerColour = GameUtil.GetOpponent(StateManager.PlayerColour);
                pieceBtn.Texture = AssetManager.LoadTexture(StateManager.PlayerColour == PieceColour.White ? "as-white" : "as-black");
            };

            var aiLabel = new Label
            {
                Rect = LayoutUtil.Below(_twoBtn.Rect, 80, -1, 20),
                Text = "AI Strategy",
                Font = AssetManager.Font(3),
                PenColour = Colour.Bone,
            };

            var aiBtn = new Button
            {
                Rect = LayoutUtil.Below(aiLabel.Rect, 40, 160, 48),
                Text = StateManager.Strategy.ToString(),
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
            };

            aiBtn.Action = () =>
            {
                StateManager.Strategy = GameUtil.GetNextEnumValue(StateManager.Strategy);
                aiBtn.Text = StateManager.Strategy.ToString();
            };

            var depthBtn = new Button
            {
                Rect = LayoutUtil.Below(aiBtn.Rect, 22, 160, 48),
                Text = StateManager.Depth.ToString(),
                Font = AssetManager.Font(2),
                PenColour = Color.Black,
            };

            depthBtn.Action = () =>
            {
                StateManager.Depth = GameUtil.GetNextEnumValue(StateManager.Depth);
                depthBtn.Text = StateManager.Depth.ToString();
            };

            _setup.AddRange([playAs, pieceBtn, aiLabel, aiBtn, depthBtn]);
        }
        
        private void InitControls()
        {
            int offset = Layout.WinHeight * 3 / 4 - _title.Rect.Bottom;

            var startBtn = new Button
            {
                Rect = LayoutUtil.BelowCenter(_title.Rect, offset, 400, 48),
                Text = "Start New Game",
                Font = AssetManager.Font(3),
                Texture = AssetManager.LoadColoredTexture(Colour.Mustard),
                PenColour = Color.Black,
                Action = () =>
                {
                    StateManager.GameState = GameState.Playing;
                    _game.Init();
                    GameUtil.AwaitMouseClick();
                }
            };

            var resumeBtn = new Button
            {
                Rect = LayoutUtil.Below(startBtn.Rect, 24, 400, 40),
                Text = "Resume (Coming Soon)",
                Font = AssetManager.Font(2),
                Texture = AssetManager.LoadColoredTexture(Colour.Fossil),
                PenColour = Color.Black,
                Action = () => { }
            };

            var exitBtn = new Button
            {
                Rect = LayoutUtil.Below(resumeBtn.Rect, 24, 400, 40),
                Text = "Exit",
                Font = AssetManager.Font(2),
                Texture = AssetManager.LoadColoredTexture(Colour.Buccaneer),
                PenColour = Colour.Bone,
                Action = StateManager.Exit
            };

            AddRange([startBtn, resumeBtn, exitBtn]);
        }
    }
}
