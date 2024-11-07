using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

using SpriteFontPlus;
using System.IO;

namespace CheckMate.Managers
{
    public class AssetManager
    {
        private ContentManager _content;
        private GraphicsDevice _graphics;

        public AssetManager(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            LoadPieceTextures();
            LoadTileTextures();
            LoadFonts();
        }

        public Dictionary<int, SpriteFont> Fonts { get; private set; }
        public Dictionary<int, Texture2D> Pieces { get; private set; }
        public Dictionary<TileType, Texture2D> Tiles { get; private set; }

        // Load Piece textures from Assets directory
        private void LoadPieceTextures()
        {
            Pieces = new Dictionary<int, Texture2D>();
            var pieceTypes = Enum.GetValues(typeof(PieceType)).Cast<PieceType>().ToArray();
            foreach (PieceType pieceType in pieceTypes)
            {
                if (pieceType == PieceType.Null) continue;

                Pieces.Add((int)pieceType, _content.Load<Texture2D>($"White{pieceType.ToString()}"));
                Pieces.Add(-(int)pieceType, _content.Load<Texture2D>($"Black{pieceType.ToString()}"));
            }
        }

        // Load Tile textures from Assets directory
        private void LoadTileTextures()
        {
            Tiles = new Dictionary<TileType, Texture2D>
            {
                { TileType.White, LoadColoredTexture(Color.Beige) },
                { TileType.Black, LoadColoredTexture(Color.Peru) },
                { TileType.Danger, LoadColoredTexture(Color.Crimson) },
                { TileType.Selected, LoadColoredTexture(Color.Gold) },
                { TileType.Allowed, LoadColoredTexture(Color.Olive) },
                { TileType.Disallowed, LoadColoredTexture(Color.Maroon) },
                { TileType.Paused, LoadColoredTexture(Color.Black, 0.5f) },
            };
        }

        private void LoadFonts()
        {
            Fonts = new Dictionary<int, SpriteFont>();
            foreach (int i in Enumerable.Range(2, 5))
            {
                var font = TtfFontBaker.Bake(File.ReadAllBytes(Path.Combine("Assets/Font.ttf")),
                    i * 10, 1024, 1024,
                    new[]
                    {
                        CharacterRange.BasicLatin,
                        CharacterRange.Latin1Supplement,
                        CharacterRange.LatinExtendedA,
                        CharacterRange.Cyrillic
                    }
                );
                SpriteFont spriteFont = font.CreateSpriteFont(_graphics);
                spriteFont.LineSpacing = (int)(spriteFont.LineSpacing * 1.5);
                Fonts.Add(i, spriteFont);
            }
        }

        public Texture2D LoadColoredTexture(Color color, float alpha = 1f)
        {
            Texture2D texture = new Texture2D(_graphics, 1, 1);
            texture.SetData(new[] { new Color(color, alpha) });

            return texture;
        }

        // Load miscellaneous Content textures by name
        public Texture2D LoadTexture(string name)
            => _content.Load<Texture2D>(name);

        // Load Rectangle for rendering textures
        public Rectangle LoadRectangle(int x, int y, int width, int height, bool padding = true)
        {
            return padding
                ? new Rectangle(x * width + Constants.WIN_PADDING, y * height + Constants.WIN_PADDING, width, height)
                : new Rectangle(x * width, y * height, width, height);
        }

        // Load Vector2 for rendering strings
        public Vector2 LoadString(out SpriteFont font, int key, string text, int x = 0, int y = 0, bool centerX = true, bool centerY = true)
        {
            font = Fonts[key];
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new();
            pos.X = centerX ? (Constants.WIN_SIZE - size.X) / 2 : x;
            pos.Y = centerY ? (Constants.WIN_SIZE - size.Y) / 2 : y;

            return pos;
        }
    }
}
