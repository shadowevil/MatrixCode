using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;

namespace MatrixCode
{
    public static class SpriteFonts
    {
        public const int MAX_FONT_SIZE = 72;
        public static Dictionary<KeyValuePair<float, bool>, SpriteFontBase> font = new Dictionary<KeyValuePair<float, bool>, SpriteFontBase>();
        public static Dictionary<KeyValuePair<float, bool>, SpriteFontBase> normalFont = new Dictionary<KeyValuePair<float, bool>, SpriteFontBase>();

        public static void LoadFonts(ContentManager Content)
        {
            string ContentPath = Directory.GetCurrentDirectory() + "\\Data\\";
            font = new Dictionary<KeyValuePair<float, bool>, SpriteFontBase>();
            FontSystem _fs_b = new FontSystem();
            FontSystem _fs = new FontSystem();
            FontSystem fs = new FontSystem();
            fs.AddFont(File.ReadAllBytes(ContentPath + "Font\\kanji.ttf"));
            _fs.AddFont(File.ReadAllBytes(ContentPath + "Font\\centurygothic.ttf"));
            _fs_b.AddFont(File.ReadAllBytes(ContentPath + "Font\\centurygothic_b.ttf"));
            for (int i = 0; i < MAX_FONT_SIZE + 1; i++)
            {
                font.Add(new KeyValuePair<float, bool>(i, false), fs.GetFont(i));
                font.Add(new KeyValuePair<float, bool>(i, true), fs.GetFont(i));

                normalFont.Add(new KeyValuePair<float, bool>(i, false), _fs.GetFont(i));
                normalFont.Add(new KeyValuePair<float, bool>(i, true), _fs_b.GetFont(i));
            }
        }
    }
}
