using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    /// <summary>
    /// Helps draw a line on the screen.
    /// </summary>
    public class Line : SpriteBase
    {
        private Texture2D pixel;

        /// <summary>
        /// The first point of the line.
        /// </summary>
        public Vector2 point1;

        /// <summary>
        /// The second point of the line.
        /// </summary>
        public Vector2 point2;

        /// <summary>
        /// The thickness of the line.
        /// </summary>
        public float thickness;

        /// <summary>
        /// How this line is defined.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// point1 = point, point2 = point.
            /// </summary>
            Point,

            /// <summary>
            /// point1 = origin, point2 = vector.
            /// </summary>
            Vector
        }

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="graphics">The graphics device manager.</param>
        public Line(GraphicsDeviceManager graphics)
        {
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] {Color.White});
            point1 = Vector2.Zero;
            point2 = Vector2.Zero;
            thickness = 1;
        }

        /// <summary>
        /// Defines a line
        /// </summary>
        /// <param name="graphics">The graphics device manager</param>
        /// <param name="type">How you are defining the second point, point or vector</param>
        /// <param name="p1">The starting point</param>
        /// <param name="p2">The ending point if using Point or the direction the line should
        /// go in if using Vector.</param>
        /// <param name="thickness">The thickness of the line</param>
        public Line(GraphicsDeviceManager graphics, Type type, Vector2 p1, Vector2 p2, float thickness)
        {
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
            point1 = p1;
            if (type == Type.Point)
            {
                point2 = p2;
            }
            else if (type == Type.Vector)
            {
                p2 = Vector2.Normalize(p2);
                p2 *= 2000;
                point2 = point1 + p2;
            }
            this.thickness = thickness;
        }

        /// <summary>
        /// Updates the line.
        /// </summary>
        public override void Update()
        {
            point1 += velocity;
            point2 += velocity;
        }

        /// <summary>
        /// Updates the line.
        /// </summary>
        /// <param name="gameTime">The game time this line is in.</param>
        public void Update(GameTimeWrapper gameTime)
        {
            point1 += velocity * (float)gameTime.GameSpeed;
            point2 += velocity * (float)gameTime.GameSpeed;
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="spriteBatch">A sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);
            spriteBatch.Draw(pixel, point1, null, color, rotation, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }
    }
}
