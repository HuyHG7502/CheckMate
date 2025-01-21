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
        public void Draw(SpriteBatch spriteBatch, AssetManager assetManager);
        public void Update(GameTime gameTime);
    }
}
