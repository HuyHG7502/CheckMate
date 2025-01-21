using CheckMate.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CheckMate.UI
{
    public class Button : IDrawable
    {
        private MouseState _current, _prev;

        public bool IsClicked { get; set; }
        public bool IsDrawable { get; set; } = true;
        public string Text { get; set; }

        public SpriteFont Font { get; set; }
        public Texture2D Texture { get; set; }
        public Color PenColour { get; set; }
        public Rectangle Rect { get; set; }

        public Action Action { get; set; }
        public static Texture2D Base { get; set; }
        public static Texture2D Highlight { get; set; }

        public void Draw(SpriteBatch spriteBatch, AssetManager assetManager)
        {
            if (IsDrawable)
            {
                if (IsClicked)
                    spriteBatch.Draw(assetManager.LoadColoredTexture(Color.Wheat), new Rectangle(Rect.X - 4, Rect.Y - 4, Rect.Width + 8, Rect.Height + 8), Color.White * 0.8f);

                spriteBatch.Draw(Texture ?? Base, Rect, Color.White);
                if (!string.IsNullOrEmpty(Text))
                    spriteBatch.DrawString(Font, Text, assetManager.LoadString(Font, Text, Rect), PenColour);

                if (Highlight != null && Rect.Contains(Mouse.GetState().Position))
                    spriteBatch.Draw(Highlight, Rect, Color.White * 0.25f);
            }
        }

        public void Update(GameTime gameTime)
        {
            _prev = _current;
            _current = Mouse.GetState();

            if (Action != null
                && IsDrawable
                && Rect.Contains(_current.Position)
                && Util.IsMouseClicked(_current, _prev))
                Action();
        }

    }
}
