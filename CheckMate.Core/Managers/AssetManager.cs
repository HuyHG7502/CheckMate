using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using SpriteFontPlus;
using CheckMate.Entities;
using CheckMate.Config;
using CheckMate.Helpers;

namespace CheckMate.Managers
{
    public class AssetManager
    {
        private readonly ContentManager _content;
        private readonly GraphicsDevice _graphics;

        public PieceSet CurrentSet { get; private set; } = PieceSet.dubrovny;
        public Dictionary<PieceSet, Dictionary<int, Texture2D>> PieceSets { get; private set; } = new();
        public Dictionary<FontStyle, Dictionary<int, SpriteFont>> Fonts { get; private set; } = new();
        public Dictionary<TileType, Texture2D> Tiles { get; private set; } = new();
        public Dictionary<int, Texture2D> Pieces { get; private set; } = new();

        public Theme Theme { get; private set; } = new Theme();

        public AssetManager(ContentManager content, GraphicsDevice graphics)
        {
            _content = content;
            _graphics = graphics;

            LoadPieceSet(PieceSet.dubrovny);
            LoadTiles();
        }

        // Fonts
        public SpriteFont Font(int size, FontStyle style = FontStyle.Regular)
        {
            if (!Fonts.TryGetValue(style, out var sizedFonts))
                Fonts[style] = sizedFonts = new Dictionary<int, SpriteFont>();

            if (!sizedFonts.TryGetValue(size, out var font))
                sizedFonts[size] = font = LoadFont(style, size);

            return font;
        }

        private SpriteFont LoadFont(FontStyle style, int size)
        {
            string path = style switch
            {
                FontStyle.Bold => "Assets/fonts/CabalBold.ttf",
                _ => "Assets/fonts/Cabal.ttf"
            };

            var baked = TtfFontBaker.Bake(File.ReadAllBytes(Path.Combine(path)),
                size * 10, 1024, 1024,
                [
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.LatinExtendedA,
                    CharacterRange.Cyrillic
                ]
            );

            return baked.CreateSpriteFont(_graphics);
        }

        // Tiles
        public TileType LoadTile(PieceColour colour)
            => colour == PieceColour.White ? TileType.White : TileType.Black;

        public Texture2D LoadColoredTexture(Color color, float alpha = 1f)
        {
            Texture2D texture = new Texture2D(_graphics, 1, 1);
            texture.SetData([new Color(color, alpha)]);

            return texture;
        }
        
        private void LoadTiles()
        {
            Tiles = Theme.ToTileColorMap().ToDictionary(
                entry => entry.Key,
                entry => LoadColoredTexture(entry.Value)
            );
        }

        // Pieces
        public void LoadPieceSet(PieceSet name)
        {
            if (!PieceSets.ContainsKey(name))
                LoadPieceTextures(name);

            Pieces = PieceSets[name];
            CurrentSet = name;
        }

        private void LoadPieceTextures(PieceSet set)
        {
            var dict = new Dictionary<int, Texture2D>();
            foreach (PieceType type in Enum.GetValues(typeof(PieceType)))
            {
                if (type == PieceType.Null) continue;

                string whitePath = $"pieces/{set}/{GameUtil.ColourMap[PieceColour.White]}{GameUtil.TypeMap[type]}";
                string blackPath = $"pieces/{set}/{GameUtil.ColourMap[PieceColour.Black]}{GameUtil.TypeMap[type]}";

                dict[(int)type] = _content.Load<Texture2D>(whitePath);
                dict[-(int)type] = _content.Load<Texture2D>(blackPath);
            }

            PieceSets[set] = dict;
        }

        // Load miscellaneous Content textures by name
        public Texture2D LoadTexture(string name)
            => _content.Load<Texture2D>(name);

        // Geometry Helpers
        // Load Rectangle for rendering textures
        public Rectangle LoadRectangle(int x, int y, int width, int height, int offsetX = 0, int offsetY = 0)
            => new(x + offsetX, y + offsetY, width, height);

        // Load Square
        public Rectangle LoadSquare(int x, int y, int size, int offsetX, int offsetY)
            => new(x * size + offsetX, y * size + offsetY, size, size);

        public Rectangle LoadSquare(Square square, int size, int offsetX, int offsetY)
            => LoadSquare(square.Rank, square.File, size, offsetX, offsetY);

        // Load Board Square with default Board offset
        public Rectangle LoadSquare(int x, int y, int size = Layout.SquareSize)
            => LoadSquare(x, y, size, Layout.BoardOffsetX, Layout.BoardOffsetY);

        public Rectangle LoadSquare(Square square, int size = Layout.SquareSize)
            => LoadSquare(square.Rank, square.File, size);

        // Load Piece in Square
        public Rectangle LoadPiece(Rectangle square, int innerSize = Layout.PieceSize)
        {
            int offset = (square.Width - innerSize) / 2;
            return new(square.X + offset, square.Y + offset, innerSize, innerSize);
        }

        public Rectangle LoadPiece(int x, int y, int outerSize = Layout.SquareSize, int innerSize = Layout.PieceSize)
        {
            Rectangle sqr = LoadSquare(x, y, outerSize);
            int offset = (outerSize - innerSize) / 2;
            return new(sqr.X + offset, sqr.Y + offset, innerSize, innerSize);
        }

        public Rectangle LoadPiece(Square square, int outerSize = Layout.SquareSize, int innerSize = Layout.PieceSize)
            => LoadPiece(square.Rank, square.File, outerSize, innerSize);

        // Load Vector2 for rendering centred strings
        public Vector2 LoadString(SpriteFont font, string text, Rectangle rect, int offsetX = -1, int offsetY = -1)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = new(rect.X, rect.Y);

            pos.X += (offsetX < 0) ? (rect.Width - size.X) / 2 : offsetX;
            pos.Y += (offsetY < 0) ? (rect.Height - size.Y) / 2 : offsetY;

            return pos;
        }
    }
}
