using Microsoft.Xna.Framework;

namespace MatrixCode
{
    static class Program
    {
        static void Main()
        {
            using(Game game = new Engine())
            {
                game.Run();
            }
        }
    }
}
