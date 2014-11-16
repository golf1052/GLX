using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GLX
{
    public class Polygon
    {
        Vector2 _pos;
        public Vector2 pos
        {
            get { return _pos; }
            set
            {
                Vector2 change = new Vector2(value.X - _pos.X, value.Y - _pos.Y);
                _pos = value;
                foreach (Line line in sides)
                {
                    line.point1 += change;
                    line.point2 += change;
                }
            }
        }

        public List<Line> sides;
        GraphicsDeviceManager graphics;

        public Polygon(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            sides = new List<Line>();
        }

        /// <summary>
        /// Adds a side using the specified positions
        /// </summary>
        /// <param name="p1">Position 1</param>
        /// <param name="p2">Position 2</param>
        public void AddSide(Vector2 p1, Vector2 p2)
        {
            Line tmp = new Line(graphics, Line.Type.Point, p1, p2, 1);
            _pos = p1;
            sides.Add(tmp);
        }

        /// <summary>
        /// Adds a side using the last side defined and a specified position
        /// </summary>
        /// <param name="pos">Position 2</param>
        public void AddSide(Vector2 pos)
        {
            if (sides.Count != 0)
            {
                Line tmp = new Line(graphics, Line.Type.Point, sides[sides.Count - 1].point2, pos, 1);
                sides.Add(tmp);
            }
            else
            {
                throw new Exception("The polygon has no sides defined.");
            }
        }

        /// <summary>
        /// Adds a side connecting the last side defined and the first side defined
        /// </summary>
        public void AddSide()
        {
            if (sides.Count != 0)
            {
                Line tmp = new Line(graphics, Line.Type.Point, sides[sides.Count - 1].point2, sides[0].point1, 1);
                sides.Add(tmp);
            }
            else
            {
                throw new Exception("The polygon has no sides defined.");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Line line in sides)
            {
                line.Draw(spriteBatch);
            }
        }
    }
}
