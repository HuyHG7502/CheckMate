using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CheckMate.Config
{
    public class Theme
    {
        public Color White { get; set; } = Colour.Karry;
        public Color Black { get; set; } = Colour.Buccaneer;
        public Color Danger { get; set; } = Colour.Persian;
        public Color Selected { get; set; } = Colour.Mustard;
        public Color Allowed { get; set; } = Colour.Apple;
        public Color Disallowed { get; set; } = Colour.Salmon;
        public Color InnerBorder { get; set; } = Colour.Windy;
        public Color OuterBorder { get; set; } = Colour.Vulcan;
        public Color Background { get; set; } = Colour.Dorado;
        public Color LightButton { get; set; } = Colour.Bone;
        public Color DarkButton { get; set; } = Colour.Fossil;
        public Color Paused { get; set; } = new Color(Color.Black, 0.6f);

        public Dictionary<TileType, Color> ToTileColorMap()
        {
            var map = new Dictionary<TileType, Color>();
            var props = typeof(Theme).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(Color) && Enum.TryParse<TileType>(prop.Name, out var TileType))
                {
                    var value = (Color)prop.GetValue(this);
                    map[TileType] = value;
                }
            }

            return map;
        }
    }
}
