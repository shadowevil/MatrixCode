using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatrixCode
{
    public class Engine : Game
    {
        public static WindowClass? g_Window { get; private set; } = null;
        public static Graphics? g_Graphics { get; private set; } = null;
        public static Random? _rand = null;

        public List<MatrixCodeChar> matrixStuff;

        private bool enabled = false;
        private int amount = 1;

        private double updater = 0.0;
        private const double updaterFrequency = 15.0;

        public Engine()
        {
            Engine t = this;
            g_Window = new WindowClass(ref t, "Ghetto Matrix", 0, 0, 800, 600, false, true);
            g_Graphics = g_Window._graphics;
            g_Window.SetPosition(WindowClass.WindowPosition.CENTER);
            IsFixedTimeStep = false;

            g_Window._window.KeyDown += _window_KeyDown;
            g_Window._window.KeyUp += _window_KeyUp;

            SpriteFonts.LoadFonts(this.Content);
            _rand = new Random(Guid.NewGuid().GetHashCode());
            matrixStuff = new List<MatrixCodeChar>();
        }

        private void _window_KeyUp(object? sender, InputKeyEventArgs e)
        {
            if(e.Key == Keys.Escape)
            {
                this.Exit();
            }
            if(e.Key == Keys.Space)
            {
                enabled = !enabled;
            }
            if (e.Key == Keys.Up)
            {
                if (amount <= 10)
                {
                    amount++;
                }
            }
            if (e.Key == Keys.Down)
            {
                if(amount > 1)
                {
                    amount--;
                }
            }
            if(e.Key == Keys.Enter)
            {
                if (g_Window == null) return;
                g_Window.ToggleFullscreen();
            }
        }

        private void _window_KeyDown(object? sender, InputKeyEventArgs e)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (g_Window == null) return;
            if (g_Graphics == null) return;
            if (!enabled) return;

            if (gameTime.TotalGameTime.TotalMilliseconds - updater >= updaterFrequency)
            {
                updater = gameTime.TotalGameTime.TotalMilliseconds;
                for (int i = 0; i < amount; i++)
                {
                    matrixStuff.Add(new MatrixCodeChar());
                }

                for (int i = matrixStuff.Count - 1; i >= 0; i--)
                {
                    MatrixCodeChar mcc = matrixStuff[i];
                    if (mcc.Disposing)
                    {
                        matrixStuff.RemoveAt(i);
                        continue;
                    }
                    mcc.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (g_Window == null) return;
            if (g_Graphics == null) return;

            g_Graphics._gd.Clear(Color.Black);
            g_Graphics._batch.Begin(blendState: BlendState.AlphaBlend);

            foreach (MatrixCodeChar mcc in matrixStuff)
            {
                mcc.Draw(gameTime, g_Graphics);
            }

            g_Graphics.DrawString(matrixStuff.Count.ToString(), 0, 0, Color.White, 14.0f, Graphics.FONT_TYPE.NORMAL_FONT, true);
            g_Graphics.DrawString(amount.ToString(), 0, 16, Color.White, 14.0f, Graphics.FONT_TYPE.NORMAL_FONT, true);
            g_Graphics.DrawString("Press Enter to " + (g_Window.isFullscreen ? "Exit fullscreen mode" : "Enter fullscreen mode"), 0, 32, Color.White, 14.0f, Graphics.FONT_TYPE.NORMAL_FONT, true);
            g_Graphics.DrawString("Press Escape to Exit the Matrix", 0, 48, Color.White, 14.0f, Graphics.FONT_TYPE.NORMAL_FONT, true);

            g_Graphics._batch.End();

            base.Draw(gameTime);
        }
    }

    public class Graphics
    {
        public enum FONT_TYPE
        {
            MATRIX_KANJI = 0,
            NORMAL_FONT = 1
        }

        public Game g_Game;
        public GraphicsDeviceManager _gdm { get; private set; }
        public GraphicsDevice _gd { get; private set; }
        public SpriteBatch _batch { get; private set; }
        public static Point BackbufferSize { get; private set; }
        public static Point AdapterBounds { get; private set; }

        public Graphics(ref Engine _game)
        {
            g_Game = _game;

            _gdm = new GraphicsDeviceManager(_game);
            _gdm.GraphicsProfile = GraphicsProfile.HiDef;
            _gdm.SynchronizeWithVerticalRetrace = false;
            _gdm.PreferMultiSampling = true;
            _gdm.ApplyChanges();

            _gd = _gdm.GraphicsDevice;
            _gd.PresentationParameters.MultiSampleCount = 8;
            _gd.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
            _gd.BlendState = BlendState.AlphaBlend;

            AdapterBounds = new Point(_gdm.GraphicsDevice.Adapter.CurrentDisplayMode.Width, _gdm.GraphicsDevice.Adapter.CurrentDisplayMode.Height);

            _batch = new SpriteBatch(_gd);
        }

        public void SetBackbufferSize(int width, int height)
        {
            _gdm.PreferredBackBufferWidth = width;
            _gdm.PreferredBackBufferHeight = height;
            _gdm.ApplyChanges();
        }

        public void DrawString(string text, float x, float y, Color c, float fontSize, FONT_TYPE ft = FONT_TYPE.NORMAL_FONT, bool bold = false)
        {
            SpriteFontBase? _f = null;
            if (ft == FONT_TYPE.MATRIX_KANJI)
            {
                SpriteFonts.font.TryGetValue(new KeyValuePair<float, bool>(fontSize, bold), out _f);
            }
            if(ft == FONT_TYPE.NORMAL_FONT)
            {
                SpriteFonts.normalFont.TryGetValue(new KeyValuePair<float, bool>(fontSize, bold), out _f);
            }
            if (_f == null) return;
            _batch.DrawString(_f, text, new Vector2(x, y), c);
        }

        public Vector2 MeasureText(string text, float fontSize, FONT_TYPE ft = FONT_TYPE.NORMAL_FONT, bool bold = false)
        {
            SpriteFontBase? _f = null;
            if (ft == FONT_TYPE.MATRIX_KANJI)
            {
                SpriteFonts.font.TryGetValue(new KeyValuePair<float, bool>(fontSize, bold), out _f);
            }
            if (ft == FONT_TYPE.NORMAL_FONT)
            {
                SpriteFonts.normalFont.TryGetValue(new KeyValuePair<float, bool>(fontSize, bold), out _f);
            }
            if (_f == null) return Vector2.Zero;
            return _f.MeasureString(text);
        }
    }

    public class WindowClass
    {
        public enum WindowPosition
        {
            CENTER = 0,
            TOPLEFT = 1
        }

        public Graphics _graphics;
        public GameWindow _window;
        public string Title { get; set; }
        public Rectangle ClientBounds { get; private set; }
        public Point Location { get; private set; }
        public bool isFullscreen { get; private set; } = false;
        public bool isBorderless { get; private set; } = false;

        public WindowClass(ref Engine _game, string _title, int s_locationX, int s_locationY, int _width, int _height, bool _isFullscreen = false, bool _isBorderless = false)
        {
            _graphics = new Graphics(ref _game);

            Title = _title;
            if (_isFullscreen) ClientBounds = new Rectangle(0, 0, _graphics._gd.Adapter.CurrentDisplayMode.Width, _graphics._gd.Adapter.CurrentDisplayMode.Height);
            else ClientBounds = new Rectangle(s_locationX, s_locationY, _width, _height);
            isFullscreen = _isFullscreen;
            isBorderless = _isBorderless;
            _window = _game.Window;

            _window.Title = Title;
            _window.Position = new Point(ClientBounds.X, ClientBounds.Y);
            _window.IsBorderless = isBorderless;
            _graphics.SetBackbufferSize(ClientBounds.Width, ClientBounds.Height);
            _graphics._gdm.IsFullScreen = isFullscreen;
        }

        public void SetPosition(int x, int y)
        {
            ClientBounds = new Rectangle(x, y, ClientBounds.Width, ClientBounds.Height);
            Location = ClientBounds.Location;
            _window.Position = ClientBounds.Location;
            _graphics.SetBackbufferSize(ClientBounds.Width, ClientBounds.Height);
        }

        public void SetPosition(WindowPosition _pos)
        {
            switch(_pos)
            {
                case WindowPosition.CENTER:
                    SetPosition((Graphics.AdapterBounds.X - _window.ClientBounds.Width) / 2,
                                (Graphics.AdapterBounds.Y - _window.ClientBounds.Height) / 2);
                    break;
                case WindowPosition.TOPLEFT:
                    SetPosition(0, 0);
                    break;
            }
        }

        public void ToggleFullscreen()
        {
            isFullscreen = !isFullscreen;
            _graphics._gdm.ToggleFullScreen();
            if (!isFullscreen)
            {
                ClientBounds = new Rectangle(0, 0, 800, 600);
                _graphics.SetBackbufferSize(ClientBounds.Width, ClientBounds.Height);
                SetPosition(WindowPosition.CENTER);
            }
            else
            {
                ClientBounds = new Rectangle(0, 0, Graphics.AdapterBounds.X, Graphics.AdapterBounds.Y);
                _graphics.SetBackbufferSize(ClientBounds.Width, ClientBounds.Height);
            }
        }

        public void SetBounds(int width, int height)
        {
            ClientBounds = new Rectangle(ClientBounds.X, ClientBounds.Y, width, height);
            _graphics.SetBackbufferSize(width, height);
        }
    }
}
