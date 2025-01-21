using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

using SpriteFontPlus;
using System.IO;
using System.Diagnostics;

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

                Pieces.Add((int)pieceType, _content.Load<Texture2D>($"w{Util.TypeMap[pieceType]}"));
                Pieces.Add(-(int)pieceType, _content.Load<Texture2D>($"b{Util.TypeMap[pieceType]}"));
            }
        }

        // Load Tile textures from Assets directory
        private void LoadTileTextures()
        {
            Tiles = new Dictionary<TileType, Texture2D>
            {
                { TileType.White, LoadColoredTexture(Color.Ivory) },
                { TileType.Black, LoadColoredTexture(Color.Tan) },
                { TileType.Danger, LoadColoredTexture(Color.Crimson) },
                { TileType.Selected, LoadColoredTexture(Color.Gold) },
                { TileType.Allowed, LoadColoredTexture(Color.YellowGreen) },
                { TileType.Disallowed, LoadColoredTexture(Color.IndianRed) },
                { TileType.Paused, LoadColoredTexture(Color.Black, 0.6f) },
            };
        }

        private void LoadFonts()
        {
            Fonts = new Dictionary<int, SpriteFont>();
            foreach (int i in Enumerable.Range(1, 10))
            {
                var font = TtfFontBaker.Bake(File.ReadAllBytes(Path.Combine("Assets/Font.ttf")),
                    i * 10, 1024, 1024,
                    [
                        CharacterRange.BasicLatin,
                        CharacterRange.Latin1Supplement,
                        CharacterRange.LatinExtendedA,
                        CharacterRange.Cyrillic
                    ]
                );
                SpriteFont spriteFont = font.CreateSpriteFont(_graphics);
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
        public Rectangle LoadRectangle(int x, int y, int width, int height)
            => new(x, y, width, height);

        public Rectangle LoadSquare(int x, int y, int width, int height)
            => new(x * width + Constants.WIN_PADDING, y * height + Constants.WIN_PADDING, width, height);

        // Load Vector2 for rendering centred strings
        public Vector2 LoadString(SpriteFont font, string text, Rectangle rect, bool centerX = true, bool centerY = true)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new(rect.X, rect.Y);
            if (centerX) pos = pos + new Vector2((rect.Width - size.X) / 2, 0);
            if (centerY) pos = pos + new Vector2(0, (rect.Height - size.Y) / 2);

            return pos;
        }
    }
}
