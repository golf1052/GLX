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
        static float _gameSpeed;
        public static float gameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                _gameSpeed = value;
            }
        }
        GraphicsDeviceManager graphics;
        public static int windowWidth;
        public static int windowHeight;

        List<GameTimeWrapper> gameTimes;

        public World(GraphicsDeviceManager graphics)
        {
            gameSpeed = 1.0f;
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
