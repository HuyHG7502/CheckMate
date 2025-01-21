using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CheckMate.UI
{
    public class Label : IDrawable
    {
        public bool IsDrawable { get; set; } = true;
        public Texture2D Texture { get; set; }
        public SpriteFont Font { get; set; }
        public Color PenColour { get; set; }
        public Rectangle Rect { get; set; }
        public string Text { get; set; }

        public void Draw(SpriteBatch spriteBatch, AssetManager assetManager)
        {
            if (IsDrawable)
            {
                if (Texture != null)
                    spriteBatch.Draw(Texture, Rect, Color.White);

                if (Text != null)
                    spriteBatch.DrawString(Font, Text, assetManager.LoadString(Font, Text, Rect), PenColour);
            }
        }

        public void Update(GameTime gameTime)
        {
            return;
        }
    }
}
