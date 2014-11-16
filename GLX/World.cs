using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class World
    {
        GraphicsDeviceManager graphics;
        public static int windowWidth;
        public static int windowHeight;
        public static Random random = new Random();

        List<GameTimeWrapper> gameTimes;

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;
            gameTimes = new List<GameTimeWrapper>();
        }

        public void AddTime(GameTimeWrapper time)
        {
            gameTimes.Add(time);
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameTimeWrapper time in gameTimes)
            {
                time.Update(gameTime);
            }
        }
    }
}
