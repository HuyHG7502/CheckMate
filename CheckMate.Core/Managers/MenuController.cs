using CheckMate.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CheckMate.Managers
{
    public class MenuController
    {
        private readonly Dictionary<GameState, Menu> _menus = new();
        private readonly StateManager _state;
        private readonly AssetManager _asset;

        public MenuController(StateManager state, AssetManager asset)
        {
            _state = state;
            _asset = asset;
        }

        public void Register(Menu menu)
        {
            menu.StateManager = _state;
            menu.AssetManager = _asset;

            _menus[menu.State] = menu;
        }

        public void Update(GameTime gameTime)
        {
            if (_menus.TryGetValue(_state.GameState, out var menu))
                if (menu.IsEmpty()) menu.Init();

            if (_state.GameState == GameState.Start)
                _menus[GameState.Start].Update(gameTime);
            else
                _menus[GameState.Playing].Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_state.GameState == GameState.Start && _menus.TryGetValue(GameState.Start, out var menu))
                menu.Draw(spriteBatch, _asset);
            else
                _menus[GameState.Playing].Draw(spriteBatch, _asset);
        }

        public Menu GetMenu(GameState state)
            => _menus.TryGetValue(state, out var menu) ? menu : null;
    }
}
