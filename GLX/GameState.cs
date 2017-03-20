using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class GameState
    {
        public string name;
        public List<GameTimeWrapper> gameTimes;
        public List<Action> drawMethods;

        public GameState(string name, GraphicsDeviceManager graphics)
        {
            this.name = name;
            gameTimes = new List<GameTimeWrapper>();
            drawMethods = new List<Action>();
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
                if (time.NormalUpdate)
                {
                    time.Update(gameTime);
                }
                else
                {
                    time.UpdateByIncrement(gameTime);
                }
            }
        }

        public void AddDraw(Action drawMethod)
        {
            drawMethods.Add(drawMethod);
        }
    }
}
