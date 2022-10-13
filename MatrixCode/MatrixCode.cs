using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixCode
{
    public class MatrixCodeChar : IDisposable
    {
        public Vector2 pos { get; private set; }
        public Vector2 vel { get; private set; }
        private string strArray = "!\"#$%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_abcdefghijklmnopqrstuvwxyz{|}~­­­­;";
        private string displayChar = "";
        private double updater = 0.0;
        private const double updateFrequency = 250.0;
        private double opacityUpdater = 0.0;
        private const double opacityUpdateFrequency = 1.0;
        private float opacity = 1.0f;
        private float opacityChange = 0.0075f;

        public bool Disposing = false;

        private List<MatrixCodeChar>? prevCharList;
        private int prevCharPos = 1;
        private float FONT_SIZE = 28.0f;

        public MatrixCodeChar()
        {
            if (Engine.g_Window == null) return;
            if (Engine._rand == null) return;
            float range = ((FONT_SIZE / 2.0f) + 1.0f) * (Engine._rand.Next(0, Convert.ToInt32(Graphics.AdapterBounds.X / ((FONT_SIZE / 2.0f) + 1.0f))));
            pos = new Vector2(range, Engine._rand.Next(-200, -100));
            vel = new Vector2(0, Engine._rand.Next(2, 3) + (float)Engine._rand.NextDouble());
            displayChar = Convert.ToString(strArray[Engine._rand.Next(0, strArray.Length - 1)]);
            prevCharList = new List<MatrixCodeChar>();
            opacityChange = (Engine._rand.Next(1, 25) / 1000.0f);
            FONT_SIZE = 28.0f - Convert.ToInt32(opacityChange * 1000.0f);
        }

        public MatrixCodeChar(Vector2 _pos, Vector2 _vel, string _dispChar, int _prevCharPos, float opacity, float _opacityChange)
        {
            this.pos = _pos;
            this.vel = _vel;
            this.displayChar = _dispChar;
            this.prevCharList = null;
            this.prevCharPos = _prevCharPos;
            this.opacity = opacity;
            this.opacityChange = _opacityChange;
            FONT_SIZE = 28.0f - Convert.ToInt32(opacityChange * 1000.0f);
        }

        public MatrixCodeChar Clone()
        {
            return new MatrixCodeChar(this.pos, this.vel, this.displayChar, prevCharPos, opacity, opacityChange);
        }

        public void Update(GameTime gameTime)
        {
            if (this.Disposing) return;
            if (Engine.g_Window == null) return;
            if(pos.Y > Engine.g_Window.ClientBounds.Height + 200)
            {
                this.Dispose();
                this.Disposing = true;
            }
            if(gameTime.TotalGameTime.TotalMilliseconds - updater >= updateFrequency && prevCharList != null)
            {
                updater = gameTime.TotalGameTime.TotalMilliseconds;
                if (Engine._rand == null) return;
                for(int i=prevCharList.Count-1; i >= 0; i--)
                {
                    if (prevCharList[i].opacity <= 0.0f) prevCharList.RemoveAt(i);
                }

                prevCharList.Insert(0, Clone());
                prevCharList.First().prevCharPos++;
                for (int i = 0; i < prevCharList.Count; i++)
                {
                    MatrixCodeChar c = prevCharList[i];
                    c.pos = new Vector2(c.pos.X, c.pos.Y - ((FONT_SIZE - 4.0f) * (c.prevCharPos - 1)));
                }
                displayChar = Convert.ToString(strArray[Engine._rand.Next(0, strArray.Length - 1)]);
            }
            if(gameTime.TotalGameTime.TotalMilliseconds - opacityUpdater >= opacityUpdateFrequency && prevCharList != null)
            {
                opacityUpdater = gameTime.TotalGameTime.TotalMilliseconds;
                for (int i = 0; i < prevCharList.Count; i++)
                {
                    MatrixCodeChar c = prevCharList[i];
                    c.opacity -= opacityChange;
                }
            }
            this.pos += this.vel;
            if (prevCharList == null) return;
            foreach(MatrixCodeChar mcc in prevCharList)
            {
                mcc.pos += mcc.vel;
            }
        }

        public void Draw(GameTime gameTime, Graphics _graphics)
        {
            if (this.Disposing) return;
            if (Engine.g_Window == null) return;
                        _graphics.DrawString(displayChar, pos.X, pos.Y, Color.LightGreen, FONT_SIZE, Graphics.FONT_TYPE.MATRIX_KANJI, true);

            if (prevCharList == null) return;
            foreach(MatrixCodeChar mcc in prevCharList)
            {
                _graphics.DrawString(mcc.displayChar, mcc.pos.X, mcc.pos.Y, Color.Green * mcc.opacity, FONT_SIZE, Graphics.FONT_TYPE.MATRIX_KANJI);
            }
        }

        public void Dispose()
        {
            if(prevCharList != null) prevCharList.Clear();
            prevCharList = null;

        }
    }
}
