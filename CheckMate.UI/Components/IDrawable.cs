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
    public interface IDrawable
    {
        public bool IsDrawable { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public string Text { get; set; }
        public Texture2D Texture { get; set; }
        public SpriteFont Font { get; set; }
        public Color PenColour { get; set; }
        public Rectangle Rect { get; set; }

        public void Draw(SpriteBatch spriteBatch, AssetManager assetManager);
        public void Update(GameTime gameTime);
    }
}
