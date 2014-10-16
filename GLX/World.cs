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

        public World(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;
        }
    }
}
