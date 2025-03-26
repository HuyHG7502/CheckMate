using CheckMate.Config;
using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.UI
{
    public abstract class Menu
    {
        public abstract GameState State { get; }
        public AssetManager AssetManager { get; set; }
        public StateManager StateManager { get; set; }

        protected List<IDrawable> _elements = new();

        public virtual void Init()
        {
            Button.Base = AssetManager.LoadColoredTexture(Colour.Fossil);
            Button.Highlight = AssetManager.LoadColoredTexture(Colour.Karry);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsEmpty()) return;

            foreach (var element in _elements)
                element.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, AssetManager assetManager)
        {
            if (IsEmpty()) return;

            foreach (var element in _elements)
                if (element.IsDrawable)
                    element.Draw(spriteBatch, assetManager);
        }

        public void Add(IDrawable element)
            => _elements.Add(element);

        public void AddRange(IEnumerable<IDrawable> elements)
            => _elements.AddRange(elements);

        public void Remove(IDrawable element)
            => _elements.Remove(element);

        public void RemoveRange(IEnumerable<IDrawable> elements)
            => _elements.RemoveAll(elements.Contains);

        public void Clear()
            => _elements.Clear();

        public bool IsEmpty()
            => _elements.Count == 0;
    }
}
