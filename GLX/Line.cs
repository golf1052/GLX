using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Line : SpriteBase
    {
        Texture2D pixel;
        public Vector2 point1;
        public Vector2 point2;
        public float width;

        public Line(GraphicsDeviceManager graphics)
        {
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] {Color.White});
        }

        public Line(GraphicsDeviceManager graphics, Vector2 p1, Vector2 p2, float width)
        {
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            point1 = p1;
            point2 = p2;
            this.width = width;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);
            spriteBatch.Draw(pixel, point1, null, color, rotation, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
        }
    }
}
