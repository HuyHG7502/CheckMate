using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CheckMate.UI
{
    public class Label : IDrawable
    {
        public bool IsDrawable { get; set; } = true;
        public int OffsetX { get; set; } = -1;
        public int OffsetY { get; set; } = -1;
        public string Text { get; set; }

        public Texture2D Texture { get; set; }
        public SpriteFont Font { get; set; }
        public Color PenColour { get; set; }
        public Rectangle Rect { get; set; }

        public void Draw(SpriteBatch spriteBatch, AssetManager assetManager)
        {
            if (IsDrawable)
            {
                if (Texture != null)
                    spriteBatch.Draw(Texture, Rect, Color.White);

                if (Text != null)
                    spriteBatch.DrawString(Font, Text, assetManager.LoadString(Font, Text, Rect, OffsetX, OffsetY), PenColour);
            }
        }

        public void Update(GameTime gameTime)
        {
            return;
        }
    }
}
